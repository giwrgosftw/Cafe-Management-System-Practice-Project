import {Component, EventEmitter, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {ProductService} from "../../../services/product.service";
import {CategoryService} from "../../../services/category.service";
import {SnackbarService} from "../../../services/snackbar.service";
import {GlobalConstants} from "../../../shared/global-constants";

@Component({
  selector: 'app-product', // The custom HTML tag used for this component
  templateUrl: './product.component.html', // The HTML layout for this component
  styleUrls: ['./product.component.scss'] // The styles applied to this component
})
export class ProductComponent implements OnInit {

  onAddProduct = new EventEmitter(); // Emits an event when a product is successfully added
  onEditProduct = new EventEmitter(); // Emits an event when a product is successfully edited
  productForm:any = FormGroup; // Form group that manages the product form controls/fields
  dialogAction:any = "Add"; // Determines the action type (Add/Edit) based on dialog data
  action:any = "Add"; // Action label used in the UI (Add/Update)
  responseMessage:any; // Stores messages (e.g., success or error) to display to the user
  categories:any = []; // Holds the list of categories fetched from the server

  /**
   * Constructor function that initializes the ProductComponent.
   * Injects necessary services for form building, product management, dialog control, category management, and snackbar notifications.
   *
   * @param dialogData - Data passed to the dialog, used to determine if the action is Add or Edit.
   * @param formBuilder - Service to handle reactive form creation.
   * @param productService - Service to manage product-related operations like add and update.
   * @param dialogRef - Reference to the dialog containing this component, used to close it when needed.
   * @param categoryService - Service to fetch categories from the backend.
   * @param snackbarService - Service to display messages via a snackbar notification.
   */
  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogData:any,
    private formBuilder:FormBuilder,
    private productService:ProductService,
    public dialogRef:MatDialogRef<ProductComponent>,
    private categoryService:CategoryService,
    private snackbarService:SnackbarService,
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It sets up the product form with validation rules and checks if the dialog action is for editing an existing product.
   * If editing, it pre-fills the form with the existing product data. It also fetches the list of categories.
   */
  ngOnInit(): void {
    this.productForm = this.formBuilder.group({
      name:[null,[Validators.required, Validators.pattern(GlobalConstants.nameRegex)]], // Validates that the name field is required and follows a specific pattern
      categoryId:[null, [Validators.required]], // Validates that the category field is required
      price:[null, Validators.required], // Validates that the price field is required
      description:[null,Validators.required] // Validates that the description field is required
    });

    // Check if the action is Edit, and if so, set the form data and update labels
    if(this.dialogData.action === "Edit"){
      this.dialogAction = "Edit";
      this.action = "Update";
      this.productForm.patchValue(this.dialogData.data); // Pre-fill the form with existing data
    }
    this.getCategories(); // Fetch the list of categories from the server
  }

  /**
   * Fetches the list of categories from the backend service and assigns it to the categories array.
   * In case of an error, it handles the error by displaying an appropriate message.
   */
  getCategories(){
    this.categoryService.getCategories().subscribe((response:any)=>{
      this.categories = response;
    }, (error: any) => {
      this.handleError(error);
    });
  }

  /**
   * Handles the form submission.
   * It determines whether the action is to add or edit a product and calls the appropriate method.
   */
  handleSubmit() {
    if (this.dialogAction === "Edit") {
      this.edit(); // Call the edit method if the action is Edit
    } else {
      this.add(); // Call the add method if the action is Add
    }
  }

  /**
   * Handles adding a new product.
   * It sends the product data to the server, closes the dialog, and emits an event to refresh the product list.
   * In case of an error, it handles the error by displaying an appropriate message.
   */
  add() {
    let formData = this.productForm.value;
    let data = {
      name: formData.name,
      categoryId: formData.categoryId,
      price: formData.price,
      description: formData.description
    }

    this.productService.add(data).subscribe((response: any) => {
      this.dialogRef.close(); // Close the dialog after successful addition
      /*
      The current component (ProductComponent) who is subscribed to this event will receive the signal,
      triggering a function to refresh or update the list of products
      (e.g., by re-fetching the list from the server)
      */
      this.onAddProduct.emit(); // Emit event to refresh the product list
      this.responseMessage = response.message;
      this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
    }, (error) => {
      this.handleError(error); // Handle any errors that occur
    });
  }

  /**
   * Handles editing an existing product.
   * It sends the updated product data to the server, closes the dialog, and emits an event to refresh the product list.
   * In case of an error, it handles the error by displaying an appropriate message.
   */
  edit() {
    let formData = this.productForm.value;
    let data = {
      id: this.dialogData.data.id,
      name: formData.name,
      categoryId: formData.categoryId,
      price: formData.price,
      description: formData.description
    }

    this.productService.update(data).subscribe((response: any) => {
      this.dialogRef.close(); // Close the dialog after successful update
      this.onEditProduct.emit(); // Emit event to refresh the product list
      this.responseMessage = response.message;
      this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
    }, (error) => {
      this.handleError(error); // Handle any errors that occur
    });
  }

  /**
   * Handles errors that occur during API calls.
   * It logs the error to the console and displays an appropriate error message via a snackbar.
   *
   * @param error - The error object received from the API call.
   */
  private handleError(error: any) {
    console.log(error);
    if(error.error?.message){
      this.responseMessage = error.error?.message; // Set specific error message if available
    } else {
      this.responseMessage = GlobalConstants.genericError; // Set a generic error message if no specific message is available
    }
    this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error) // Display the error message in a snackbar
  }
}
