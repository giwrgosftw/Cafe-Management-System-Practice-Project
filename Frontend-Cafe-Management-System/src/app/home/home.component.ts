import { Component, OnInit } from '@angular/core';
import {MatDialog, MatDialogConfig} from '@angular/material/dialog';
import {SignupComponent} from '../signup/signup.component';
import {ForgotPasswordComponent} from "../forgot-password/forgot-password.component";
import {LoginComponent} from "../login/login.component";
import {UserService} from "../services/user.service";
import {Router} from "@angular/router";

@Component({
  selector: 'app-home', // Defines the HTML tag for this component
  templateUrl: './home.component.html', // Links to the component's HTML template
  styleUrls: ['./home.component.scss'] // Links to the component's CSS styles
})
export class HomeComponent implements OnInit {

  /**
   * Constructor function that initializes the HomeComponent.
   * Injects services for dialog handling, user service operations, and routing.
   *
   * @param dialog - Service to open Angular Material dialogs.
   * @param userServices - Service to manage user-related operations, such as token validation.
   * @param router - Service to handle routing within the application.
   */
  constructor(
    private dialog: MatDialog,
    private userServices: UserService,
    private router: Router
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It checks if a valid user token exists; if it does, it navigates to the dashboard.
   */
  ngOnInit(): void {
    // Check if the user token is valid when the home page is loaded
    this.userServices.checkToken().subscribe(
      (response: any) => {
        // If the token is valid, navigate to the dashboard
        this.router.navigate(['cafe/dashboard']);
      },
      (error: any) => {
        console.log(error); // Log any errors to the console
      }
    );
  }

  /**
   * Handles the action of opening the signup dialog.
   * Configures the dialog's width and opens the SignupComponent inside it.
   */
  handleSignupAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '550px'; // Set the width of the signup dialog
    this.dialog.open(SignupComponent, dialogConfig); // Open the signup dialog
  }

  /**
   * Handles the action of opening the forgot password dialog.
   * Configures the dialog's width and opens the ForgotPasswordComponent inside it.
   */
  handleForgotPasswordAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '500px'; // Set the width of the forgot password dialog
    this.dialog.open(ForgotPasswordComponent, dialogConfig); // Open the forgot password dialog
  }

  /**
   * Handles the action of opening the login dialog.
   * Configures the dialog's width and opens the LoginComponent inside it.
   */
  handleLoginAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '550px'; // Set the width of the login dialog
    this.dialog.open(LoginComponent, dialogConfig); // Open the login dialog
  }
}
