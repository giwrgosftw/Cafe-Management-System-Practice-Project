import { Component, OnInit } from '@angular/core';
import {MatDialog, MatDialogConfig} from '@angular/material/dialog';
import {SignupComponent} from '../signup/signup.component';
import {ForgotPasswordComponent} from "../forgot-password/forgot-password.component";
import {LoginComponent} from "../login/login.component";
import {UserService} from "../services/user.service";
import {Router} from "@angular/router";
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor(
    private dialog: MatDialog,
    private userServices:UserService,
    private router:Router
  ) { }
  ngOnInit(): void {
    // whenever we are at the homepage and a token exists,
    // our localstorage will hit the checkToken() API
    this.userServices.checkToken().subscribe((response:any) => {
      this.router.navigate(['cafe/dashboard']);
    }, (error:any) => {
        console.log(error);
    })
  }
  handleSignupAction(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '550px';
    this.dialog.open(SignupComponent, dialogConfig);
  }

  handleForgotPasswordAction(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '500px';
    this.dialog.open(ForgotPasswordComponent, dialogConfig);
  }

  handleLoginAction(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '550px';
    this.dialog.open(LoginComponent, dialogConfig);
  }

}
