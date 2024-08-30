import { Component, OnInit } from '@angular/core';
import {NgxUiLoaderService} from "ngx-ui-loader";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import {SnackbarService} from "../../services/snackbar.service";
import {Router} from "@angular/router";
import {MatTableDataSource} from "@angular/material/table";
import {GlobalConstants} from "../../shared/global-constants";
import {ProductService} from "../../services/product.service";
import {ProductComponent} from "../dialog/product/product.component";
import {ConfirmationComponent} from "../dialog/confirmation/confirmation.component";

@Component({
  selector: 'app-manage-product', // The HTML tag for this component
  templateUrl: './manage-product.component.html', // The applied HTML template
  styleUrls: ['./manage-product.component.scss'] // The applied CSS styles
})
export class ManageProductComponent implements OnInit {

  displayedColumns: string[] = ['name', 'categoryName', 'description', 'price', 'edit']; // Columns displayed in the table
  dataSource: any; // Data source for the table
  responseMessage: any; // Stores messages to display to the user

  /**
   * Constructor function that initializes the ManageProductComponent.
   * Injects necessary services for product management, loading, dialog management, and routing.
   *
   * @param productService - Service to manage product-related operations.
   * @param ngxService - Service to show and hide a loading indicator.
   * @param dialog - Service to handle dialog windows.
   * @param snackbarService - Service to display messages via a snackbar notification.
   * @param router - Service to manage navigation between routes.
   */
  constructor(
    private productService: ProductService,
    private ngxService: NgxUiLoaderService,
    private dialog: MatDialog,
    private snackbarService: SnackbarService,
    private router: Router
  ) { }

  /**
   * Lifecycle hook that is called after Angular has initialized all data-bound properties of a directive.
   * Starts the loading indicator and fetches the product data for the table.
   */
  ngOnInit(): void {
    this.ngxService.start(); // Start the loading indicator
    this.tableData(); // Fetch and load the table data
  }

  /**
   * Fetches the list of products from the backend and populates the table.
   * Stops the loading indicator once the data is loaded or if an error occurs.
   */
  tableData() {
    this.productService.getProducts().subscribe((response: any) => {
      this.ngxService.stop(); // Stop the loading indicator
      this.dataSource = new MatTableDataSource(response); // Set the data source for the table
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loading indicator in case of error
      this.handleError(error); // Handle the error
    });
  }

  /**
   * Filters the table data based on the user's input in the filter box.
   * E.g., search filter
   * @param event - The keyup event from the filter input field.
   */
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase(); // Apply the filter to the table data
  }

  /**
   * Opens a dialog to add a new product. Closes any open dialogs when the route changes.
   */
  handleAddAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      action: 'Add' // Specifies that the action is to add a new product
    };
    dialogConfig.width = "850px"; // Set the width of the dialog
    const dialogRef = this.dialog.open(ProductComponent, dialogConfig); // Open the dialog
    this.router.events.subscribe(() => {
      dialogRef.close(); // Close the dialog on route change
    });

    const sub = dialogRef.componentInstance.onAddProduct.subscribe((response) => {
      this.tableData(); // Refresh the table data after adding a product
    });
  }

  /**
   * Opens a dialog to edit an existing product. Closes any open dialogs when the route changes.
   *
   * @param values - The product data to be edited.
   */
  handleEditAction(values: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      action: 'Edit', // Specifies that the action is to edit an existing product
      data: values
    };
    dialogConfig.width = "850px"; // Set the width of the dialog
    const dialogRef = this.dialog.open(ProductComponent, dialogConfig); // Open the dialog
    this.router.events.subscribe(() => {
      dialogRef.close(); // Close the dialog on route change
    });

    const sub = dialogRef.componentInstance.onEditProduct.subscribe((response) => {
      this.tableData(); // Refresh the table data after editing a product
    });
  }

  /**
   * Opens a confirmation dialog to delete a product. If confirmed, deletes the product.
   *
   * @param values - The product data to be deleted.
   */
  handleDeleteAction(values: any) {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      message: 'delete ' + values.name + ' product', // Confirmation message
      confirmation: true // Specifies that this is a confirmation dialog
    };
    // Open the confirmation dialog
    const dialogRef = this.dialog.open(ConfirmationComponent, dialogConfig);
    const sub = dialogRef.componentInstance.onEmitStatusChange.subscribe((response) => {
      this.ngxService.start(); // Start the loading indicator
      this.deleteProduct(values.id); // Delete the product if confirmed
      dialogRef.close(); // Close the confirmation dialog
    });
  }

  /**
   * Deletes a product by its ID. Refreshes the table data after deletion.
   *
   * @param id - The ID of the product to be deleted.
   */
  deleteProduct(id: any) {
    this.productService.delete(id).subscribe((response: any) => {
      this.ngxService.stop(); // Stop the loading indicator
      this.tableData(); // Refresh the table data after deletion
      this.responseMessage = response?.message; // Get the response message
      this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loading indicator in case of error
      this.handleError(error); // Handle the error
    });
  }

  /**
   * Handles the change of product status (activate/deactivate).
   *
   * @param status - The new status of the product (true for active, false for inactive).
   * @param id - The ID of the product whose status is being changed.
   */
  onChange(status: any, id: any) {
    this.ngxService.start(); // Start the loading indicator
    let data = {
      status: status.toString(), // Convert status to string
      id: id
    };

    this.productService.updateStatus(data).subscribe((response: any) => {
      this.ngxService.stop(); // Stop the loading indicator
      this.responseMessage = response?.message; // Get the response message
      this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loading indicator in case of error
      this.handleError(error); // Handle the error
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
}
