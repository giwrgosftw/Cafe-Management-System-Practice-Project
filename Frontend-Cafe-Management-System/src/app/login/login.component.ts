import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../services/user.service";
import {MatDialogRef} from "@angular/material/dialog";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {SnackbarService} from "../services/snackbar.service";
import {Router} from "@angular/router";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-login', // Defines the HTML tag for this component
  templateUrl: './login.component.html', // Links to the component's HTML template
  styleUrls: ['./login.component.scss'] // Links to the component's CSS styles
})
export class LoginComponent implements OnInit {
  hide = true; // Controls the visibility of the password field (true = hidden)
  loginForm: any = FormGroup; // Form group that manages the login form controls/fields
  responseMessage: any; // Stores messages (e.g., error messages) to display to the user

  /**
   * Constructor function that initializes the LoginComponent.
   * Injects necessary services for form building, routing, user authentication,
   * dialog control, UI loading, and snackbar notifications.
   *
   * @param formBuilder - Service to handle reactive form creation.
   * @param router - Service to handle routing within the application.
   * @param userService - Service to manage user authentication.
   * @param dialogRef - Reference to the dialog containing this component, used to close it when needed.
   * @param ngxService - Service to show or hide the loading spinner.
   * @param snackbarService - Service to display messages via a snackbar notification.
   */
  constructor(
    private formBuilder:FormBuilder,
    private router:Router,
    private userService:UserService,
    private dialogRef:MatDialogRef<LoginComponent>,
    private ngxService:NgxUiLoaderService,
    private snackbarService:SnackbarService
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It sets up the login form with validation rules for the email and password fields.
   */
  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email:[null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]], // Validates email format
      password:[null, [Validators.required]] // Password field is required
    });
  }

  /**
   * Handles form submission when the user attempts to log in.
   * It sends the login data to the server and, if successful, stores the authentication token
   * and navigates to the dashboard. In case of an error, it displays an appropriate error message.
   */
  handleSubmit() {
    this.ngxService.start(); // Start the loading spinner
    const formData = this.loginForm.value;
    const data = {
      email: formData.email,
      password: formData.password
    };
    this.userService.login(data).subscribe(
      (response: any) => {
        this.ngxService.stop(); // Stop the loading spinner once the login is successful
        this.dialogRef.close(); // Close the login dialog
        // Store the token in local storage, which is required for authenticated API requests
        localStorage.setItem('token', response.token);
        this.router.navigate(['/cafe/dashboard']); // Navigate to the dashboard after successful login
      },
      (error) => {
        this.ngxService.stop(); // Stop the loading spinner in case of an error
        if (error.error?.message) {
          this.responseMessage = error.error?.message; // Set specific error message from server
        } else {
          this.responseMessage = GlobalConstants.genericError; // Set a generic error message
        }
        // Display the error message using a snackbar notification
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    );
  }
}
