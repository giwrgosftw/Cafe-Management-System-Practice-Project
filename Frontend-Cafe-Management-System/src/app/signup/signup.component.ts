import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {UserService} from "../services/user.service";
import {SnackbarService} from "../services/snackbar.service";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {MatDialogRef} from "@angular/material/dialog";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-signup', // The custom HTML tag used for this component
  templateUrl: './signup.component.html', // The HTML layout for this component
  styleUrls: ['./signup.component.scss'] // The styles applied to this component
})
export class SignupComponent implements OnInit {

  password = true; // Controls visibility of the password field
  confirmPassword = true; // Controls visibility of the 'confirm password' field
  signupForm: any = FormGroup; // FormGroup instance to handle the signup form-fields
  responseMessage: any; // Stores messages to display to the user

  /**
   * Constructor function to initialize the SignupComponent.
   * Injects necessary services for form building, routing, user management, snackbar notifications, and loading.
   *
   * @param formBuilder - Service to handle reactive form creation.
   * @param router - Service for navigation.
   * @param userService - Service to manage user-related operations like signup.
   * @param snackbarService - Service to display messages via a snackbar notification.
   * @param ngxService - Service to display and control loading.
   * @param dialogRef - Reference to the dialog/modal containing this component, used to close it when needed.
   */
  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private userService: UserService,
    private snackbarService: SnackbarService,
    private ngxService: NgxUiLoaderService,
    public dialogRef: MatDialogRef<SignupComponent> // Dialog reference to close the dialog when needed
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It sets up the signup form with initial values and validation rules.
   */
  ngOnInit(): void {
    // Initialize the signup form with fields and their validation rules
    this.signupForm = this.formBuilder.group({
      name: [null, [Validators.required, Validators.pattern(GlobalConstants.nameRegex)]],
      email: [null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]],
      contactNumber: [null, [Validators.required, Validators.pattern(GlobalConstants.contactNumberRegex)]],
      password: [null, [Validators.required]],
      confirmPassword: [null, [Validators.required]]
    });
  }

  /**
   * Validates that the password and confirm password fields match.
   * Returns true if they do not match, indicating an error.
   */
  validatePasswordSubmit() {
    return this.signupForm.controls['password'].value !== this.signupForm.controls['confirmPassword'].value;
  }

  /**
   * Handles the signup form submission.
   * It sends the user input to the backend for account creation, shows a loading spinner, and handles the response.
   */
  handleSingUpSubmitButton() {
    this.ngxService.start(); // Start the loading spinner when the submit button is clicked

    // Gather the form data into an object
    const formData = this.signupForm.value;
    const data = {
      name: formData.name,
      email: formData.email,
      contactNumber: formData.contactNumber,
      password: formData.password
    };

    // Send the signup request to the backend
    this.userService.signup(data).subscribe(
      (response: any) => {
        // Handle successful signup
        this.ngxService.stop(); // Stop the loading spinner
        this.dialogRef.close(); // Close the signup dialog
        this.responseMessage = response?.message; // Get the success message from the response
        this.snackbarService.openSnackBar(this.responseMessage, ""); // Show the success message in a snackbar
        this.router.navigate(['/']); // Navigate to the homepage
      },
      // Handle signup errors
      (error) => {
        this.ngxService.stop(); // Stop the loading spinner
        if (error.error?.message) {
          this.responseMessage = error.error?.message; // Set specific error message from backend
        } else {
          this.responseMessage = GlobalConstants.genericError; // Set a generic error message
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error); // Show the error message in a snackbar
      }
    );
  }
}
