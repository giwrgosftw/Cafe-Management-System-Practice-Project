import { Component, AfterViewInit } from '@angular/core';
import {DashboardService} from "../services/dashboard.service";
import {SnackbarService} from "../services/snackbar.service";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-dashboard', // This specifies the custom HTML tag for the component
  templateUrl: './dashboard.component.html', // The HTML template associated with this component
  styleUrls: ['./dashboard.component.scss'] // The styles associated with this component
})
export class DashboardComponent implements AfterViewInit {

  responseMessage: any; // Hold any error messages or other responses
  data: any; // Hold the data fetched from the server

  // Lifecycle hook that is called after Angular has fully initialize a component's view.
  ngAfterViewInit() { }

  /**
   * Constructor function that runs when the component is initialized.
   * It starts the loading indicator and initiates the data fetching process.
   *
   * @param dashboardService - Service for fetching dashboard data from the backend.
   * @param ngxService - Service for showing and hiding a loading indicator.
   * @param snackbarService - Service for displaying messages to the user via a snackbar.
   */
  constructor(
    private dashboardService: DashboardService,
    private ngxService: NgxUiLoaderService,
    private snackbarService: SnackbarService
  ) {
    this.ngxService.start(); // Start the loading indicator as soon as the component is initialized
    this.dashboardData(); // Fetch the dashboard data from the backend
  }

  /**
   * Fetches data for the dashboard from the backend service. It handles
   * the loading state, processes the response data, and manages any errors
   * that occur during the API call.
   *
   * The method starts the loading indicator when data fetching begins and stops it when
   * data fetching is complete or if an error occurs. The fetched data is stored in the
   * `data` variable, and any errors are handled by displaying an appropriate message in
   * a snackbar.
   */
  dashboardData() {
    this.dashboardService.getDetails().subscribe(
      (response: any) => {
        this.ngxService.stop(); // Start the loading indicator when data fetching starts
        this.data = response; // Store the fetched data in the `data` variable
      },
      (error: any) => {
        // Handle any errors that occur during the data fetching process
        this.ngxService.stop(); // Stop the loading indicator when an error occurs
        console.log(error); // Log the error to the console for debugging
        if (error.error?.message) {
          this.responseMessage = error.error?.message; // Set a specific error message if available
        } else {
          this.responseMessage = GlobalConstants.genericError; // Otherwise, set a generic error message
        }
        // Show the error message in a snackbar (a small popup notification)
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    );
  }
}
