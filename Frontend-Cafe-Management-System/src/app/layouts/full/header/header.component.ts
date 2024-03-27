import { Component } from '@angular/core';
import {Router} from "@angular/router";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import {ConfirmationComponent} from "../../../material-component/dialog/confirmation/confirmation.component";
import {ChangePasswordComponent} from "../../../material-component/dialog/change-password/change-password.component";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: []
})

// exist as a nested sidebar on the top-right profile icon
export class AppHeaderComponent {
  role:any;

  constructor(
    private router:Router,
    private dialog:MatDialog
    ) { }

  logout(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data = {
      message: 'Logout',
      confirmation:true
    };
    // inject confirmation dialog to the header
    const dialogRef = this.dialog.open(ConfirmationComponent, dialogConfig);
    const sub = dialogRef.componentInstance.onEmitStatusChange.subscribe((response) => {
      dialogRef.close();
      localStorage.clear(); // clear e.g., token
      this.router.navigate(['/']);  // navigate to the startup/login page
    })
  }

  changePassword(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width="550px";
    this.dialog.open(ChangePasswordComponent, dialogConfig);
  }
}
