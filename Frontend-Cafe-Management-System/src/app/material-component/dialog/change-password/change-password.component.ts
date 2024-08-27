import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../../services/user.service";
import {MatDialogRef} from "@angular/material/dialog";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {SnackbarService} from "../../../services/snackbar.service";
import {GlobalConstants} from "../../../shared/global-constants";

@Component({
  selector: 'app-change-password', // Defines the HTML tag for this component
  templateUrl: './change-password.component.html', // Links to the component's HTML template
  styleUrls: ['./change-password.component.scss'] // Links to the component's CSS styles
})
export class ChangePasswordComponent implements OnInit {

  oldPassword = true; // Controls visibility of the old password field
  newPassword = true; // Controls visibility of the new password field
  confirmPassword = true; // Controls visibility of the confirm password field
  changePasswordForm:any = FormGroup; // Form group that manages the change password form controls
  responseMessage:any; // Stores messages (e.g., success or error) to display to the user

  /**
   * Constructor function that initializes the ChangePasswordComponent.
   * Injects necessary services for form building, user management, dialog control, loading spinner, and snackbar notifications.
   *
   * @param formBuilder - Service to handle reactive form creation.
   * @param userService - Service to manage user-related operations, specifically password change.
   * @param dialogRef - Reference to the dialog containing this component, used to close it when needed.
   * @param ngxService - Service to show or hide the loading spinner.
   * @param snackbarService - Service to display messages via a snackbar notification.
   */
  constructor(
    private formBuilder:FormBuilder,
    private userService:UserService,
    private dialogRef: MatDialogRef<ChangePasswordComponent>,
    private ngxService:NgxUiLoaderService,
    private snackbarService:SnackbarService
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It sets up the change password form with validation rules for the old password, new password, and confirm password fields.
   */
  ngOnInit(): void {
    // Assign validators for the password fields
    this.changePasswordForm = this.formBuilder.group({
      oldPassword:[null, Validators.required], // Validates that the old password field is required
      newPassword:[null, Validators.required], // Validates that the new password field is required
      confirmPassword:[null, Validators.required] // Validates that the confirm password field is required
    })
  }

  /**
   * Confirms that the new password was entered twice correctly.
   * Ensures that the new password and confirm password fields match.
   *
   * @returns boolean - Returns true if the passwords do not match, false otherwise.
   */
  validateSubmit(){
    if(this.changePasswordForm.controls['newPassword'].value != this.changePasswordForm.controls['confirmPassword'].value){
      return true;
    } else{
      return false;
    }
  }

  /**
   * Handles the password change submission.
   * It assigns the password variables to the corresponding fields, calls the API function to change the password,
   * and manages the response by displaying success or error messages.
   */
  handlePasswordChangeSubmit(){
    this.ngxService.start(); // Start the loading spinner
    let formData = this.changePasswordForm.value; // Access the form's fields
    let data = {  // Assign the corresponding password variables to the respective fields
      oldPassword: formData.oldPassword,
      newPassword: formData.newPassword,
      confirmPassword: formData.confirmPassword
    }
    // Apply the API call to change the password
    this.userService.changePassword(data).subscribe(
      (response: any) => {
        this.ngxService.stop(); // Stop the loading spinner once done
        this.responseMessage = response?.message;
        this.dialogRef.close(); // Close the dialog window
        this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
      },
      (error) => {
        console.log(error);
        this.ngxService.stop(); // Stop the loading spinner in case of an error
        if (error.error?.message) {
          this.responseMessage = error.error?.message; // Set specific error message if available
        } else {
          this.responseMessage = GlobalConstants.genericError; // Set a generic error message
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error); // Show error message
      }
    );
  }
}
