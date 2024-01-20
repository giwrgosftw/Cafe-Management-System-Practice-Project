using Cafe_management_system_backend.MVC.Models.Dtos;
using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Security;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Collections.Generic;
using System;
using System.Linq;
using iText.Layout;
using Cafe_management_system_backend.MVC.Repositories;
using NLog;
using System.IO;
using Microsoft.IdentityModel.Tokens;

namespace Cafe_management_system_backend.MVC.Services.Facades
{
    public class BillProductFacadeServiceImpl : BillProductFacadeService
    {
        private readonly BillProductRepository billProductRepository;
        private readonly ProductService productService;
        private readonly BillService billService;
        private readonly CategoryService categoryService;
        private static Logger logger = LogManager.GetLogger("NLogger");

        public BillProductFacadeServiceImpl(BillProductRepository billProductRepository, ProductService productService, 
            BillService billService, CategoryService categoryService)
        {
            this.billProductRepository = billProductRepository;
            this.productService = productService;
            this.billService = billService;
            this.categoryService = categoryService;
        }

        /// <summary> Retrieves a list of BillProducts associated with a Bill based on the Bill's ID. </summary>
        /// <param name="billId"> The ID of the Bill. </param>
        /// <returns> A list of BillProducts associated with the specified Bill. </returns>
        /// <exception cref="KeyNotFoundException"> Thrown if no BillProducts are found for the given Bill ID. </exception>
        public List<BillProduct> FindBillProductsByBillId(int billId)
        {
            List<BillProduct> billProductsDB = billProductRepository.FindAllByBillId(billId);
            if (billProductsDB.IsNullOrEmpty())
            {
                logger.Error($"[BillProductService:FindBillProductsByBillId()] Fail: BillProducts with given billID was NOT found (billId: {billId})");
                throw new KeyNotFoundException($"BillProducts with given billID was NOT found (billId: {billId})");
            }
            return billProductsDB;
        }

        /// <summary> Deletes a Bill and its associated BillProducts from the system. </summary>
        /// <param name="billUuid"> UUID of the Bill to be deleted. </param>
        public void DeleteBillProductAndBill(string billUUid)
        {
            // Find
            Bill bill = billService.FindBillByUUID(billUUid);
            List<BillProduct> billProducts = FindBillProductsByBillId(bill.id);
            // Delete
            billProductRepository.Delete(billProducts); // Need to delete these first
            billService.DeleteBill(bill.uuid);
        }

        /// <summary>
        /// Generates a bill report, including creating a new Bill, updating the many-to-many relationship 
        /// with products, and generating a PDF document for the bill.
        /// </summary>
        /// <param name="principalProfile"> Principal profile containing user information. </param>
        /// <param name="generateBillReportDto"> DTO containing information for generating the bill report. </param>
        /// <returns> A tuple containing the generated Bill and the corresponding PDF document as a byte array. </returns>
        public (Bill, byte[]) GenerateBillReport(PrincipalProfile principalProfile, GenerateBillReportDto generateBillReportDto)
        {
            try
            {
                // Check if the given products exist (else it will throw exception error)
                generateBillReportDto.Products.All(product => productService.DoesProductExistById(product.id));
                // Initialise and Save new Bill 
                Bill bill = generateBillReportDto.Bill;
                bill.createdBy = principalProfile.Email;
                billService.AddBill(bill);
                // Update the many-to-many (BillProduct) table with given Bill and its products
                foreach (var productDB in generateBillReportDto.Products)
                {
                    var billProduct = new BillProduct { billId = bill.id, productId = productDB.id };
                    billProductRepository.Add(billProduct);
                }
                // Generate/Get the final PDF
                return (bill, GetBillReportPdf(generateBillReportDto));

            }
            catch (Exception ex)
            {
                logger.Error("[BillService:GenerateBillReport()] Exception: ", ex);
                throw;
            }

        }

        /// <summary> Generates a PDF report for a bill, including customer details, product table, and footer. </summary>
        /// <param name="generateBillReportDto"> The data transfer object containing information for generating the PDF report. </param>
        /// <returns> Byte array representing the PDF document. </returns>
        private byte[] GetBillReportPdf(GenerateBillReportDto generateBillReportDto)
        {
            List<Product> products = generateBillReportDto.Products;
            using (MemoryStream memoryStream = new MemoryStream())  // Temporary storage location for PDF content
            {
                Document document = InitializePdfDocument(memoryStream);
                AddDocumentSections(generateBillReportDto.Bill, products, document);
                document.Close();
                return memoryStream.ToArray();
            }
        }

        /// <summary> Initializes a PDF document for a Bill using the provided MemoryStream. </summary>
        /// <param name="bill"> The Bill object for which to create the PDF document. </param>
        /// <param name="memoryStream"> The MemoryStream to write the PDF content. </param>
        /// <returns> The initialized PDF Document. </returns>
        private Document InitializePdfDocument(MemoryStream memoryStream)
        {
            PdfWriter writer = new PdfWriter(memoryStream); // Responsible for writing the PDF content into the memory
            PdfDocument pdf = new PdfDocument(writer);  // Allow to add more content to the PDF
            return new Document(pdf);
        }

        /// <summary> Setup the sections of the PDF document, including title, customer details, product table, and footer. </summary>
        /// <param name="bill"> The Bill object containing information for the document. </param>
        /// <param name="products"> The list of Product objects to be included in the document. </param>
        /// <param name="document"> The PDF Document to which sections will be added. </param>
        private void AddDocumentSections(Bill bill, List<Product> products, Document document)
        {
            AddTitle(document); // The title of the Cafe store
            AddNewLine(document);  // A new line to create more space
            AddLineSeparator(document); // A black (actual) line separates the title with the rest parts
            AddCustomerDetails(document, bill); // Customer details coming from the Bill
            CreateTable(document, products); // Table of product values
            AddFooter(document, bill);  // Total amount and thanks message
        }

        /// <summary> Adds a bold and centered title to a PDF document. </summary>
        /// <param name="document"> The PDF Document to which the title will be added. </param>
        private void AddTitle(Document document)
        {
            Paragraph header = new Paragraph("Cafe Management System")
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(25);
            document.Add(header);
        }

        /// <summary> Adds a new line to the PDF document. </summary>
        /// <param name="document"> The PDF Document to which the new line will be added. </param>
        private void AddNewLine(Document document)
        {
            Paragraph newline = new Paragraph(new Text("\n"));
            document.Add(newline);
        }

        /// <summary> Adds a line separator (actual line) to the PDF document. </summary>
        /// <param name="document"> The PDF Document to which the line separator will be added. </param>
        private void AddLineSeparator(Document document)
        {
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);
        }

        /// <summary> Adds customer details to the PDF document. </summary>
        /// <param name="document"> The PDF Document to which customer details will be added. </param>
        /// <param name="bill"> The Bill object containing customer information. </param>
        private void AddCustomerDetails(Document document, Bill bill)
        {
            Paragraph customerDetails = new Paragraph(
                "Name: " + bill.name +
                "\nEmail: " + bill.email +
                "\nContact Number: " + bill.contactNumber +
                "\nPayment Method: " + bill.paymentMethod
            );
            document.Add(customerDetails);
        }

        /// <summary> Creates and adds a table to the PDF document, populated with product information. </summary>
        /// <param name="document"> The PDF Document to which the table will be added. </param>
        /// <param name="products"> The list of Product objects to populate the table. </param>
        private void CreateTable(Document document, List<Product> products)
        {
            // Initialize
            List<string> columnHeaderNames = new List<string> { "Name", "Description", "Category", "Price" }; // The list of columns titles
            Table table = new Table(columnHeaderNames.Count(), false);  // IMPORTANT: the number of columns matter in the display data order
            table.SetWidth(new UnitValue(UnitValue.PERCENT, 100)); // (Unit of measurement) width is set to 100%
            // Header of each Column
            foreach (string columnHeaderName in columnHeaderNames)
            {
                AddTableColumnCellHeader(table, columnHeaderName);
            }
            // For all the products in the bill
            foreach (Product product in products)
            {
                Category categoryDB = categoryService.FindCategoryByIdWithoutException(product?.categoryId);
                // The list of value parameters that will populate each cell
                List<string> tableRowProductValues = new List<string> {
                    product?.name ?? "", product?.description ?? "", categoryDB?.name ?? "", product?.price?.ToString() ?? "" };
                // Add value to each row
                foreach (string tableRowProductValue in tableRowProductValues)
                {
                    AddTableRow(table, tableRowProductValue);
                }

            }
            document.Add(table);
        }

        /// <summary> Adds a bold and centered header cell to a table in a PDF document (column title). </summary>
        /// <param name="table"> The PDF Table to which the header cell will be added. </param>
        /// <param name="columnHeaderName"> The name of the column for the header cell. </param>
        private void AddTableColumnCellHeader(Table table, string columnHeaderName)
        {
            Cell headerCell = new Cell(1,1)
                .Add(new Paragraph(columnHeaderName))
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBold();
            table.AddCell(headerCell);
        }

        /// <summary> Adds a centered cell with a value to the table's row in the PDF document. </summary>
        /// <param name="table"> The PDF Table to which the row cell will be added. </param>
        /// <param name="tableRowValue"> The value to be added to the row cell. </param>
        private void AddTableRow(Table table, string tableRowValue)
        {
            Cell rowCell = new Cell(1,1)   // The cell should occupy one row and one column in the table
                .SetTextAlignment(TextAlignment.CENTER)
                .Add(new Paragraph(tableRowValue));
            table.AddCell(rowCell);
        }

        /// <summary> Adds a footer to the PDF document with total amount and a thank you message. </summary>
        /// <param name="document"> The PDF Document to which the footer will be added. </param>
        /// <param name="bill"> The Bill object containing total amount information. </param>
        private void AddFooter(Document document, Bill bill)
        {
            Paragraph documentFooter = new Paragraph(
                "Total: " + bill.totalAmount +
                "\nThank you for visiting us!");
            document.Add(documentFooter);
        }

    }
}