import {Component, OnInit} from '@angular/core';
import {NgxUiLoaderService} from "ngx-ui-loader";
import {UserService} from "../../services/user.service";
import {SnackbarService} from "../../services/snackbar.service";
import {GlobalConstants} from "../../shared/global-constants";
import {MatTableDataSource} from "@angular/material/table";

@Component({
  selector: 'app-manage-user', // The HTML tag for this component
  templateUrl: './manage-user.component.html', // Applied HTML template
  styleUrls: ['./manage-user.component.scss'] // Applied CSS styles
})
export class ManageUserComponent implements OnInit {

  displayedColumns: string[] = ['name', 'email', 'contactNumber', 'status']; // Columns displayed in the table
  dataSource: any; // Data source for the table
  responseMessage: any; // Stores messages to display to the user

  /**
   * Constructor function that initializes the ManageUserComponent.
   * Injects necessary services for user management, loading, and snackbar notifications.
   *
   * @param ngxService - Service to show and hide a loading indicator.
   * @param userService - Service to manage user-related operations.
   * @param snackbarService - Service to display messages via a snackbar notification.
   */
  constructor(
    private ngxService: NgxUiLoaderService,
    private userService: UserService,
    private snackbarService: SnackbarService
  ) { }

  /**
   * Lifecycle hook that is called after Angular has initialized all data-bound properties of a directive.
   * Starts the loading indicator and fetches the user data for the table.
   */
  ngOnInit(): void {
    this.ngxService.start(); // Start the loading indicator
    this.tableData(); // Fetch and load the table data
  }

  /**
   * Handles errors by logging them, setting an error message, and displaying it in a snackbar.
   *
   * @param error - The error object received from a failed HTTP request or other operation.
   */
  private handleError(error: any) {
    console.log(error); // Log the error for debugging purposes
    if (error.error?.message) {
      this.responseMessage = error.error?.message; // Use specific error message from server
    } else {
      this.responseMessage = GlobalConstants.genericError; // Use a generic error message if none is provided
    }
    this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error); // Show the error message in a snackbar
  }

  /**
   * Fetches the list of users from the backend and populates the table.
   * Stops the loading indicator once the data is loaded or if an error occurs.
   */
  tableData() {
    this.userService.getUsers().subscribe((response: any) => {
      this.ngxService.stop(); // Stop the loading indicator
      this.dataSource = new MatTableDataSource(response); // Set the data source for the table
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loading indicator in case of error
      this.handleError(error); // Handle the error
    });
  }

  /**
   * Filters the table data based on the user's input in the filter box.
   *
   * @param event - The keyup event from the filter input field.
   */
  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase(); // Apply the filter to the table data
  }

  /**
   * Handles the change of user status (activate/deactivate).
   *
   * @param status - The new status of the user (true for active, false for inactive).
   * @param id - The ID of the user whose status is being changed.
   */
  onChange(status: any, id: any) {
    this.ngxService.start(); // Start the loading indicator
    let data = {
      status: status.toString(),
      id: id
    };

    this.userService.update(data).subscribe((response: any) => {
      this.ngxService.stop(); // Stop the loading indicator
      this.responseMessage = response?.message; // Get the response message
      this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
    }, (error: any) => {
      this.ngxService.stop(); // Stop the loading indicator in case of error
      this.handleError(error); // Handle the error
    });
  }
}
