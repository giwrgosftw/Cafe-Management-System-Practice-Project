import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../services/user.service";
import {MatDialogRef} from "@angular/material/dialog";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {SnackbarService} from "../services/snackbar.service";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-forgot-password', // Defines the HTML tag for this component
  templateUrl: './forgot-password.component.html', // Links to the component's HTML template
  styleUrls: ['./forgot-password.component.scss'] // Links to the component's CSS styles
})
export class ForgotPasswordComponent implements OnInit {

  forgotPasswordForm: any = FormGroup; // Form group to manage the forgot password form controls
  responseMessage: any; // Holds any success or error messages from the server

  /**
   * Constructor function that initializes the component.
   * It injects several services to handle form building, user operations, dialog control, UI loading, and snackbars.
   *
   * @param formBuilder - Service to handle reactive form creation.
   * @param userService - Service to handle user-related operations such as password reset.
   * @param dialogRef - Reference to the dialog containing this component, used to close it when needed.
   * @param ngxService - Service to show or hide the loading spinner.
   * @param snackbarService - Service to display messages via a snackbar notification.
   */
  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    private dialogRef: MatDialogRef<ForgotPasswordComponent>,
    private ngxService: NgxUiLoaderService,
    private snackbarService: SnackbarService
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It sets up the form with validation rules for the email field.
   */
  ngOnInit(): void {
    // Initialize the form with validation for the email field
    this.forgotPasswordForm = this.formBuilder.group({
      email: [null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]]
    });
  }

  /**
   * Handles form submission when the user submits their email for password recovery.
   * It sends the email to the server and displays a loading spinner during the process.
   * Upon success, it closes the dialog and shows a success message.
   * If an error occurs, it stops the spinner and shows an error message via snackbar.
   */
  handleSubmit() {
    this.ngxService.start(); // Start the loading spinner
    const formData = this.forgotPasswordForm.value;
    const data = {
      email: formData.email
    };
    // Call the forgotPassword method from userService to send the email data to the server..
    this.userService.forgotPassword(data).subscribe(
      (response: any) => {
        this.ngxService.stop(); // Stop spinner when data is loaded
        this.responseMessage = response?.message;
        this.dialogRef.close(); // Close the dialog after a successful operation
        this.snackbarService.openSnackBar(this.responseMessage, ""); // Show success message
      },
      (error) => {
        this.ngxService.stop(); // Stop the spinner on error
        if (error.error?.message) { // Get the error message if available
          this.responseMessage = error.error?.message;
        } else {
          this.responseMessage = GlobalConstants.genericError; // Fallback to a generic error message
        }
        // Show the error message via snackbar
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    );
  }
}
