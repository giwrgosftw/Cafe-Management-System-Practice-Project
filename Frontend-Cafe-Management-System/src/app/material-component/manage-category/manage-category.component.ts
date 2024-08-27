import { Component, OnInit } from '@angular/core';
import {CategoryService} from "../../services/category.service";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import {SnackbarService} from "../../services/snackbar.service";
import {Router} from "@angular/router";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {MatTableDataSource} from "@angular/material/table";
import {GlobalConstants} from "../../shared/global-constants";
import {CategoryComponent} from "../dialog/category/category.component";

@Component({
  selector: 'app-manage-category', // The custom HTML tag used for this component
  templateUrl: './manage-category.component.html', // The HTML layout for this component
  styleUrls: ['./manage-category.component.scss'] // The styles applied to this component
})
export class ManageCategoryComponent implements OnInit {

  // Columns to be displayed in the category management table
  displayedColumns: string[] = ['name', 'edit'];
  dataSource: any; // Data source for the table, holding the category data
  responseMessage: any; // Stores messages (e.g., success or error) to display to the user

  /**
   * Constructor function that initializes the ManageCategoryComponent.
   * Injects necessary services for category management, dialog control, notifications, navigation, and loading.
   *
   * @param categoryService - Service to manage category-related operations like fetching categories.
   * @param ngxService - Service for showing and hiding a loading indicator.
   * @param dialog - Service for opening dialogs, such as the add/edit category dialog.
   * @param snackbarService - Service for displaying messages to the user via a snackbar.
   * @param router - Service for navigation and URL routing within the app.
   */
  constructor(
    private categoryService: CategoryService,
    private ngxService: NgxUiLoaderService,
    private dialog: MatDialog,
    private snackbarService: SnackbarService,
    private router: Router
  ) { }

  /**
   * Fetches the table data (categories) from the backend and populates the data source for the table.
   * Stops the loading indicator when data fetching is complete or if an error occurs.
   */
  tableData() {
    this.categoryService.getCategories().subscribe((response: any) => {
      this.ngxService.stop(); // Stop the loading indicator once data is fetched
      this.dataSource = new MatTableDataSource(response); // Assign the fetched data to the table's data source
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loader if an error occurs
      if (error.error?.message) {
        this.responseMessage = error.error?.message; // Set specific error message if available
      } else {
        this.responseMessage = GlobalConstants.genericError; // Set a generic error message if no specific message is available
      }
      console.log(this.responseMessage);
      this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error); // Display the error message in a snackbar
    })
  }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * Starts the loading indicator and fetches the table data (categories).
   */
  ngOnInit(): void {
    this.ngxService.start(); // Start the loading indicator
    this.tableData(); // Fetch the table data (categories) from the backend
  }

  /**
   * A search function for the table. Filters the table data based on the user's input.
   * It ignores case differences by converting the input to lowercase before filtering.
   *
   * @param event - The keyboard event triggered by the user typing in the search input.
   */
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase(); // Apply the filter to the table data
  }

  /**
   * Handles the action of adding a new category. Opens the add category dialog and subscribes to changes.
   * When the dialog is closed, it refreshes the table data to include the newly added category.
   */
  handleAddAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      action: 'Add'
    };
    dialogConfig.width = "850px"; // Set the width of the dialog
    const dialogRef = this.dialog.open(CategoryComponent, dialogConfig); // Open the add category dialog

    // Close the dialog if the user navigates to a different route
    this.router.events.subscribe(() => {
      dialogRef.close();
    });

    // Subscribe to the event emitted when a new category is added, and refresh the table data
    const sub = dialogRef.componentInstance.onAddCategory.subscribe(() => {
      this.tableData();
    });
  }

  /**
   * Handles the action of editing an existing category. Opens the edit category dialog with pre-filled data.
   * When the dialog is closed, it refreshes the table data to reflect the changes made.
   *
   * @param values - The category data passed to the dialog for editing.
   */
  handleEditAction(values: any) { // Expecting some data (category to edit)
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      action: 'Edit',
      data: values // Pass the category data to the dialog for editing
    };
    dialogConfig.width = "850px"; // Set the width of the dialog
    const dialogRef = this.dialog.open(CategoryComponent, dialogConfig); // Open the edit category dialog

    // Close the dialog if the user navigates to a different route
    this.router.events.subscribe(() => {
      dialogRef.close();
    });

    // Subscribe to the event emitted when the category is edited, and refresh the table data
    const sub = dialogRef.componentInstance.onAddCategory.subscribe(() => {
      this.tableData();
    });
  }
}
