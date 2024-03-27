import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../../services/user.service";
import {MatDialogRef} from "@angular/material/dialog";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {SnackbarService} from "../../../services/snackbar.service";
import {GlobalConstants} from "../../../shared/global-constants";

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {

  oldPassword = true;
  newPassword = true;
  confirmPassword = true;
  changePasswordForm:any = FormGroup;
  responseMessage:any;
  constructor(
    private formBuilder:FormBuilder,
    private userService:UserService,
    private dialogRef: MatDialogRef<ChangePasswordComponent>,
    private ngxService:NgxUiLoaderService,
    private snackbarService:SnackbarService
  ) { }

  ngOnInit(): void {
    // Assign validators for the password fields
    this.changePasswordForm = this.formBuilder.group({
      oldPassword:[null, Validators.required],
      newPassword:[null, Validators.required],
      confirmPassword:[null, Validators.required]
    })
  }

  // Confirm that the new password was entered twice correctly
  validateSubmit(){
    if(this.changePasswordForm.controls['newPassword'].value != this.changePasswordForm.controls['confirmPassword']){
      return true;
    } else{
      return false;
    }
  }

  // Assign the according password variables to the according fields
  // and call the API function
  handlePasswordChangeSubmit(){
    this.ngxService.start(); // start the loader
    let formData = this.changePasswordForm.value; // access the form's fields
    let data = {  // assign the according password variables to the according fields
      oldPassword: formData.oldPassword,
      newPassword: formData.newPassword,
      confirmPassword: formData.confirmPassword
    }
    // apply the API call
    this.userService.changePassword(data).subscribe(
      (response: any) => {
        this.ngxService.stop(); // stop loader if odne
        this.responseMessage = response?.message;
        this.dialogRef.close(); // close the dialog window as well
        this.snackbarService.openSnackBar(this.responseMessage, "success"); // show success message
      },
      (error) => {
        console.log(error);
        this.ngxService.stop(); // stop loader since error
        if (error.error?.message) {
          this.responseMessage = error.error?.message;
        } else {
          this.responseMessage = GlobalConstants.genericError;
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    );

  }
}
