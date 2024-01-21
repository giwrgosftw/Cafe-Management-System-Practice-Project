import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {UserService} from "../services/user.service";
import {SnackbarService} from "../services/snackbar.service";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {MatDialogRef} from "@angular/material/dialog";
import {GlobalConstants} from "../shared/global-constants";

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
  password = true;
  confirmPassword = true;
  signupForm:any = FormGroup;
  responseMessage:any;

  // Include the extra components required
  constructor(
    private formBuilder:FormBuilder,
    private router:Router,
    private userService:UserService,
    private snackbarService:SnackbarService,
    private ngxService:NgxUiLoaderService,
    public dialogRef:MatDialogRef<SignupComponent> // load the component in a Dialog window
  ) { }

  // First-thing automatically happens when page loads
  ngOnInit(): void {
    this.signupForm = this.formBuilder.group(
{
              name:[null, [Validators.required, Validators.pattern(GlobalConstants.nameRegex)]], // initialized with null since the user will give the input
              email:[null, [Validators.required, Validators.pattern(GlobalConstants.emailRegex)]],
              contactNumber:[null, [Validators.required, Validators.pattern(GlobalConstants.contactNumberRegex)]],
              password: [null, [Validators.required]],
              confirmPassword: [null, [Validators.required]]
            }
    )
  }

  // Validate password and confirmPassword
  validatePasswordSubmit(){
    return this.signupForm.controls['password'].value != this.signupForm.controls['confirmPassword'].value;
  }

  // What happens when the SingUp submit button pressed
  handleSingUpSubmitButton(){
    this.ngxService.start(); // start the load spinner (icon that indicate that the application is busy)
    var formData = this.signupForm.value; // get all the form field values at once (as an object)
    // Get all the data from the given user inputs of the GUI form
    var data = {
      name: formData.name,
      email: formData.email,
      contactNumber: formData.contactNumber,
      password: formData.password
    }

    // Hit the API Backend response by calling our TS Service and pass the above data (input from User)
    this.userService.signup(data).subscribe((response:any) => { // whenever we get a response from API, we are going getting from there
      // If Success (implement Signup logic)
      this.ngxService.stop(); // since we get a response from the API, we have to stop the loader spinner (the app is not busy anymore)
      this.dialogRef.close(); // close the window since submit as well
      this.responseMessage = response?.message; // get the response message from the backend
      this.snackbarService.openSnackBar(this.responseMessage, ""); // display that message through the snackbar ("" = no action button will be displayed)
      this.router.navigate(['/']); // where navigate after we press the submit button
    },
      // If Fail (display the according error message)
      (error)=>{
        this.ngxService.stop();
          if(error.error?.message){
            this.responseMessage = error.error?.message;
          }
          else{
            this.responseMessage = GlobalConstants.genericError;
          }
          this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error);
      }
    )
  }
}
