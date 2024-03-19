import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../services/user.service";
import {MatDialogRef} from "@angular/material/dialog";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {SnackbarService} from "../services/snackbar.service";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {

  forgotPasswordForm:any = FormGroup;
  responseMessage:any
  constructor(
    private formBuilder:FormBuilder,
    private userService:UserService,
    private dialogRef:MatDialogRef<ForgotPasswordComponent>,
    private ngxService:NgxUiLoaderService,
    private snackbarService:SnackbarService
    ) { }

  ngOnInit(): void {
    // Group fields so that to declare which one are required
    this.forgotPasswordForm = this.formBuilder.group({
      email:[null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]]
    });
  }

  handleSubmit(){
    this.ngxService.start(); // start the spinner
    const formData = this.forgotPasswordForm.value;
    const data = {
      email: formData.email
    };
    this.userService.forgotPassword(data).subscribe((response:any)=>{
        this.ngxService.stop(); // stop spinner since loaded data
        this.responseMessage = response?.message;
        this.dialogRef.close();
        this.snackbarService.openSnackBar(this.responseMessage, "");
      },(error)=>{
        this.ngxService.stop();
        if(error.error?.message){ // get the error message
          this.responseMessage = error.error?.message;
        }
        else {
          this.responseMessage = GlobalConstants.genericError;
        }
        // Show the error through snackbar
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      })
  }

}
