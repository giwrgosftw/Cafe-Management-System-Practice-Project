import { Component, OnInit } from '@angular/core';
import {BillService} from "../../services/bill.service";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import {SnackbarService} from "../../services/snackbar.service";
import {Router} from "@angular/router";
import {MatTableDataSource} from "@angular/material/table";
import {GlobalConstants} from "../../shared/global-constants";
import {ConfirmationComponent} from "../dialog/confirmation/confirmation.component";
import {ViewBillProductsComponent} from "../dialog/view-bill-products/view-bill-products.component";

@Component({
  selector: 'app-view-bill', // Defines the component's selector
  templateUrl: './view-bill.component.html', // Links to the HTML template
  styleUrls: ['./view-bill.component.scss'] // Links to the SCSS stylesheet
})
export class ViewBillComponent implements OnInit {

  displayedColumns: string[] = ['name', 'email', 'contactNumber', 'paymentMethod', 'total', 'view']; // Columns displayed in the table
  dataSource: any; // Data source for the table
  responseMessage: any; // Holds the response message for the snackbar

  constructor(
    private billService:BillService, // Service to manage bill operations
    private ngxService:NgxUiLoaderService, // Service to manage loading
    private dialog:MatDialog, // Service to manage dialog operations
    private snackbarService:SnackbarService, // Service to manage snackbar notifications
    private router: Router // Router service to manage navigation
  ) { }

  /**
   * Lifecycle hook that is called after Angular has initialized all data-bound properties.
   * This is used to start the loading indicator and fetch the table data.
   */
  ngOnInit(): void {
    setTimeout(() => {
      this.ngxService.start();
      this.tableData();
    });
  }

  /**
   * Handles any errors encountered during API calls by displaying an appropriate message
   * in the snackbar and logging the error to the console.
   *
   * @param error - The error object returned from the API call.
   */
  private handleError(error: any) {
    console.log(error);
    if (error.error?.message) {
      this.responseMessage = error.error?.message;
    } else {
      this.responseMessage = GlobalConstants.genericError;
    }
    this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
  }

  /**
   * Fetches bill data from the server and populates the dataSource for the table.
   * Stops the loading indicator once the data is loaded or if an error occurs.
   */
  tableData() {
    this.billService.getBills().subscribe((response: any) => {
      this.ngxService.stop();
      this.dataSource = new MatTableDataSource(response);
    }, (error:any)=>{
      this.ngxService.stop();
      this.handleError(error);
    })
  }

  /**
   * Applies a filter to the table based on the input provided by the user.
   * Converts the filter value to lowercase and trims any whitespace before filtering.
   *
   * @param event - The keyboard event that triggers the filter.
   */
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  /**
   * Opens a dialog to view the details of a selected bill.
   *
   * @param values - The bill data to be viewed.
   */
  handleViewAction(values: any) {
    // TODO: In the backend app, modify the 'billService.getPdf' API to 'FindBillProductsByBillId'
    //       because in the current page there are no the bill-product data to grap from the UI
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      data:values
    }
    dialogConfig.width = "100%";
    const dialogRef = this.dialog.open(ViewBillProductsComponent, dialogConfig);
    this.router.events.subscribe(()=>{
      dialogRef.close();
    })
  }

  /**
   * Opens a confirmation dialog before deleting a bill.
   *
   * @param values - The bill data to be deleted.
   */
  handleDeleteAction(values: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      message:'delete " ' + values.name + ' " bill?',
      confirmation:true
    };
    const dialogRef = this.dialog.open(ConfirmationComponent, dialogConfig);
    const sub = dialogRef.componentInstance.onEmitStatusChange.subscribe((response)=>{
      this.ngxService.start();
      this.deleteBill(values.uuid);
      dialogRef.close();
    })
  }

  /**
   * Deletes a bill by its ID and refreshes the table data.
   *
   * @param id - The ID of the bill to be deleted.
   */
  deleteBill(id: any) {
    this.billService.delete(id).subscribe((response: any) => {
      this.ngxService.stop();
      this.tableData();
      this.responseMessage = response?.message;
      this.snackbarService.openSnackBar(this.responseMessage, "success");
    }, (error:any)=>{
      this.ngxService.stop();
      this.handleError(error);
    })
  }

  /**
   * Initiates the download of a bill report in PDF format.
   *
   * @param values - The bill data to be downloaded.
   */
  downloadReportAction(values: any) {
    this.ngxService.start();
    // Constructing each Product object
    const products = Array.isArray(values.productDetails) ? values.productDetails.map((product: any) => ({
      id: product.id,
      name: product.name,
      categoryId: product.categoryId,
      description: product.description,
      price: product.price
    })) : [];
    // Constructing the Bill object
    let data = {
      Bill: {
        name: values.name,
        email: values.email,
        contactNumber: values.contactNumber,
        paymentMethod: values.paymentMethod,
        totalAmount: values.totalAmount
      },
      Products: products  // Using the validated products array
    };
    this.downloadFile(values.uuid, data);  // Call the method to download the file
  }

  /**
   * Downloads the PDF file for the bill.
   *
   * @param fileName - The name of the file to be downloaded.
   * @param data - The data to be included in the PDF file.
   */
  downloadFile(fileName: string, data: any) {
    // Call the getPdf method from the billService with the provided (document) data
    this.billService.getPdf(data).subscribe(
      (response) => {
        // Create a Blob object to hold the PDF data from the response
        const blob = new Blob([response], { type: 'application/pdf' });
        // Create a URL for the Blob to allow the browser to access the PDF data
        const url = window.URL.createObjectURL(blob);
        // Create an anchor (<a>) element to facilitate the download
        const anchor = document.createElement('a');
        anchor.href = url;              // Set the URL as the href of the anchor
        anchor.download = fileName + '.pdf';  // Set the download attribute with the desired file name
        anchor.click();                 // Programmatically click the anchor to trigger the download
        // Clean up by revoking the object URL to free up resources
        window.URL.revokeObjectURL(url);
        // Stop the loading spinner as the download is complete
        this.ngxService.stop();
      },
      (error: any) => {
        this.ngxService.stop();
        this.handleError(error);
      }
    );
  }

}
