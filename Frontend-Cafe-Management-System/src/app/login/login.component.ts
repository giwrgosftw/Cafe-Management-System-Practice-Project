import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../services/user.service";
import {MatDialogRef} from "@angular/material/dialog";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {SnackbarService} from "../services/snackbar.service";
import {Router} from "@angular/router";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  hide = true; // hide the login typed password
  loginForm:any = FormGroup;
  responseMessage:any;
  constructor(
    private formBuilder:FormBuilder,
    private router:Router,
    private userService:UserService,
    private dialogRef:MatDialogRef<LoginComponent>,
    private ngxService:NgxUiLoaderService,
    private snackbarService:SnackbarService
  ) { }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email:[null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]],
      password:[null, [Validators.required]]
    })
  }

  handleSubmit(){
    this.ngxService.start();
    const formData = this.loginForm.value;
    const data = {
      email: formData.email,
      password: formData.password
    }
    this.userService.login(data).subscribe((response:any)=>{
        this.ngxService.stop();
        this.dialogRef.close();
        // Store the token (coming from backend), required for any API call authentication
        localStorage.setItem('token', response.token);
        this.router.navigate(['/cafe/dashboard']);
    }, (error) => {
        if(error.error?.message){
          this.responseMessage = error.error?.message;
        }
        else{
          this.responseMessage = GlobalConstants.genericError;
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      });
  }

}
