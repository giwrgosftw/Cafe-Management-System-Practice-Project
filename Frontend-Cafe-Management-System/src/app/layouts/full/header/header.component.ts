import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import {ConfirmationComponent} from "../../../material-component/dialog/confirmation/confirmation.component";
import {ChangePasswordComponent} from "../../../material-component/dialog/change-password/change-password.component";

@Component({
  selector: 'app-header', // Defines the HTML tag for this component
  templateUrl: './header.component.html', // Links to the component's HTML template
  styleUrls: [] // Currently, no specific styles are associated with this component
})
/**
 * The AppHeaderComponent manages the header section of the application,
 * including handling user interactions from the profile icon in the top-right sidebar.
 */
export class AppHeaderComponent {
  role: any; // Holds the role of the user, which can be used for conditional rendering or logic

  /**
   * Constructor function that initializes the AppHeaderComponent.
   * Injects the Router for navigation and MatDialog for opening dialogs.
   *
   * @param router - Service to handle routing within the application.
   * @param dialog - Service to open Angular Material dialogs.
   */
  constructor(
    private router: Router,
    private dialog: MatDialog
  ) { }

  /**
   * Handles the logout action triggered from the profile menu.
   * Opens a confirmation dialog to confirm the logout action.
   * If confirmed, clears the local storage (e.g., token) and navigates to the startup/login page.
   */
  logout() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      message: 'Logout',
      confirmation: true
    };
    // Open the confirmation dialog
    const dialogRef = this.dialog.open(ConfirmationComponent, dialogConfig);
    // Subscribe to the confirmation response
    const sub = dialogRef.componentInstance.onEmitStatusChange.subscribe((response) => {
      dialogRef.close(); // Close the dialog after response
      localStorage.clear(); // Clear local storage (e.g., user token)
      this.router.navigate(['/']);  // Navigate to the startup/login page
    });
  }

  /**
   * Handles the change password action triggered from the profile menu.
   * Opens a dialog with the ChangePasswordComponent to allow users to update their password.
   */
  changePassword() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = "550px"; // Set the width of the change password dialog
    this.dialog.open(ChangePasswordComponent, dialogConfig); // Open the change password dialog
  }
}
