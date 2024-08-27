import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {CategoryService} from "../../services/category.service";
import {ProductService} from "../../services/product.service";
import {SnackbarService} from "../../services/snackbar.service";
import {BillService} from "../../services/bill.service";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {GlobalConstants} from "../../shared/global-constants";
import {saveAs} from "file-saver";

@Component({
  selector: 'app-manage-order', // The HTML tag for this component
  templateUrl: './manage-order.component.html', // The applied HTML template
  styleUrls: ['./manage-order.component.scss'] // The applied CSS styles
})
export class ManageOrderComponent implements OnInit {

  displayedColumns: string[] = ['name', 'category', 'price', 'quantity', 'total', 'edit']; // Columns displayed in the table
  dataSource: any = []; // Data source for the table
  manageOrderForm: any = FormGroup; // Form group for managing the order form controls
  categories: any = []; // List of categories available for selection
  products: any = []; // List of products available for selection
  price: any; // Holds the price of the selected product
  totalAmount: number = 0; // The total amount for the order
  responseMessage: any; // Stores messages to display to the user

  /**
   * Constructor function that initializes the ManageOrderComponent.
   * Injects necessary services for form building, category and product management, snackbar notifications, bill generation, and loading.
   *
   * @param formBuilder - Service to handle reactive form creation.
   * @param categoryService - Service to manage category-related operations.
   * @param productService - Service to manage product-related operations.
   * @param snackbarService - Service to display messages via a snackbar notification.
   * @param billService - Service to handle bill generation and PDF downloading.
   * @param ngxService - Service to show and hide a loading indicator.
   */
  constructor(
    private formBuilder: FormBuilder,
    private categoryService: CategoryService,
    private productService: ProductService,
    private snackbarService: SnackbarService,
    private billService: BillService,
    private ngxService: NgxUiLoaderService
  ) { }

  /**
   * Lifecycle hook that is called after Angular has initialized all data-bound properties of a directive.
   * Initializes the form, starts the loading indicator, and fetches categories.
   */
  ngOnInit(): void {
    this.ngxService.start(); // Start the loading indicator
    this.getCategories(); // Fetch available categories
    this.manageOrderForm = this.formBuilder.group({
      name: [null, [Validators.required, Validators.pattern(GlobalConstants.nameRegex)]],
      email: [null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]],
      contactNumber: [null, [Validators.required, Validators.pattern(GlobalConstants.contactNumberRegex)]],
      paymentMethod: [null, [Validators.required]],
      product: [null, [Validators.required]],
      category: [null, [Validators.required]],
      quantity: [null, [Validators.required]],
      price: [null, [Validators.required]],
      total: [0, [Validators.required]]
    });
  }

  /**
   * Handles errors by logging them, setting an error message, and displaying it in a snackbar.
   *
   * @param error - The error object received from a failed HTTP request or other operation.
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
   * Fetches the list of categories from the backend.
   * Stops the loading indicator once the categories are retrieved or if an error occurs.
   */
  getCategories() {
    this.categoryService.getFilteredCategories().subscribe((response: any) => {
      this.ngxService.stop();
      this.categories = response;
    }, (error: any) => {
      this.ngxService.stop();
      this.handleError(error);
    });
  }

  /**
   * Fetches the list of products based on the selected category.
   * Resets the price, quantity, and total fields after fetching the products.
   *
   * @param value - The selected category.
   */
  getProductByCategory(value: any) {
    this.productService.getProductsByCategory(value.id).subscribe((response: any) => {
      this.products = response;
      this.manageOrderForm.controls['price'].setValue('');
      this.manageOrderForm.controls['quantity'].setValue('');
      this.manageOrderForm.controls['total'].setValue(0);
    }, (error: any) => {
      this.ngxService.stop();
      this.handleError(error);
    });
  }

  /**
   * Fetches the details of a selected product, including its price.
   * Sets the initial quantity to 1 and calculates the total.
   *
   * @param value - The selected product.
   */
  getProductDetails(value: any) {
    this.productService.getById(value.id).subscribe((response: any) => {
      this.price = response.price;
      this.manageOrderForm.controls['price'].setValue(response.price);
      this.manageOrderForm.controls['quantity'].setValue('1');
      this.manageOrderForm.controls['total'].setValue(this.price * 1);
    }, (error: any) => {
      this.ngxService.stop();
      this.handleError(error);
    });
  }

  /**
   * Updates the total price based on the quantity entered by the user.
   * Ensures that the quantity is a positive number, and if not, resets it to 1.
   *
   * @param value - The quantity value input by the user.
   */
  setQuantity(value: any) {
    // Retrieve the current value of the quantity control from the form
    let temp = this.manageOrderForm.controls['quantity'].value;

    // Check if the entered quantity is greater than 0
    if (temp > 0) {
      // Calculate and update the total based on the quantity and price
      this.manageOrderForm.controls['total'].setValue(
        this.manageOrderForm.controls['quantity'].value * this.manageOrderForm.controls['price'].value
      );
    } else if (temp != '') {
      // If the quantity is not valid (e.g., negative or zero), reset it to 1
      this.manageOrderForm.controls['quantity'].setValue('1');
      // Recalculate and update the total based on the reset quantity and price
      this.manageOrderForm.controls['total'].setValue(
        this.manageOrderForm.controls['quantity'].value * this.manageOrderForm.controls['price'].value
      );
    }
  }

  /**
   * Validates whether a product can be added to the order.
   * Returns true if the total is zero, null, or if the quantity is less than or equal to zero.
   */
  validateProductAdd() {
    if (
      this.manageOrderForm.controls['total'].value === 0 ||
      this.manageOrderForm.controls['total'].value === null ||
      this.manageOrderForm.controls['quantity'].value <= 0
    ) {
      return true;
    } else {
      return false;
    }
  }

  /**
   * Validates whether the form can be submitted.
   * Returns true if the total amount is zero or if any required fields are empty.
   */
  validateSubmit() {
    if (
      this.totalAmount === 0 ||
      this.manageOrderForm.controls['name'].value === null ||
      this.manageOrderForm.controls['email'].value === null ||
      this.manageOrderForm.controls['contactNumber'].value === null ||
      this.manageOrderForm.controls['paymentMethod'].value === null
    ) {
      return true;
    } else {
      return false;
    }
  }

  /**
   * Adds the selected product to the order if it hasn't been added already.
   * Updates the total amount and displays a success or error message accordingly.
   */
  add() {
    let formData = this.manageOrderForm.value;
    // Checks if the selected product already exists in the order (dataSource) by comparing product IDs
    let productName = this.dataSource.find((e: { id: number }) => e.id === formData.product.id);
    if (productName === undefined) {
      this.totalAmount = this.totalAmount + formData.total;
      this.dataSource.push({
        id: formData.product.id,
        name: formData.product.name,
        category: formData.category.name,
        quantity: formData.quantity,
        price: formData.price,
        total: formData.total
      });
      this.dataSource = [...this.dataSource]; // create a (shallow) copy
      this.snackbarService.openSnackBar(GlobalConstants.productAdded, "success");
    } else {
      this.snackbarService.openSnackBar(GlobalConstants.productExistsError, GlobalConstants.error);
    }
  }

  /**
   * Deletes a product from the order and updates the total amount.
   *
   * @param value - The index of the product in the dataSource array.
   * @param element - The product object that is being deleted.
   */
  handleDeleteAction(value: any, element: any) {
    this.totalAmount = this.totalAmount - element.total;
    this.dataSource.splice(value, 1);  // remove '1' element at index 'value'
    this.dataSource = [...this.dataSource]; // update the data source to reflect changes
  }

  /**
   * Submits the order by generating a bill.
   * Sends the order details to the server, downloads the bill PDF, and resets the form.
   */
  submitAction() {
    let formData = this.manageOrderForm.value;
    let data = {
      name: formData.name,
      email: formData.email,
      contactNumber: formData.contactNumber,
      paymentMethod: formData.paymentMethod,
      totalAmount: this.totalAmount.toString(),
      productDetails: JSON.stringify(this.dataSource)
    };

    this.ngxService.start(); // Start the loading indicator
    this.billService.generateReport(data).subscribe((response: any) => {
      this.downloadFile(response?.uuid); // Download the bill PDF
      this.dataSource = []; // Reset the order data
      this.totalAmount = 0; // Reset the total amount
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loading indicator
      this.handleError(error); // Handle any errors
    });
  }

  /**
   * Downloads the bill PDF using the file name provided by the server.
   *
   * @param fileName - The UUID of the file to be downloaded.
   */
  downloadFile(fileName: string) {
    let data = {
      uuid: fileName
    };

    this.billService.getPdf(data).subscribe((response: any) => {
      saveAs(response, fileName + '.pdf'); // Save the PDF file locally
      this.ngxService.stop(); // Stop the loading indicator
    });
  }
}
