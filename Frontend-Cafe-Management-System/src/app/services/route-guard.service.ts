import { Injectable } from '@angular/core';
import {AuthService} from "./auth.service";
import {ActivatedRouteSnapshot, Router} from "@angular/router";
import {SnackbarService} from "./snackbar.service";
import{jwtDecode} from "jwt-decode";
import {GlobalConstants} from "../shared/global-constants";

@Injectable({
  providedIn: 'root'
})
export class RouteGuardService {

  constructor(
    public auth:AuthService,
    public router:Router,
    private snackbarService:SnackbarService
  ) { }

  /* A few URLs that are only accessible by admins */
  canActivate(route:ActivatedRouteSnapshot):boolean{
    // Initialization
    let expectedRoleArray = route.data;
    expectedRoleArray = expectedRoleArray.expectedRole;
    const token:any = localStorage.getItem('token');  // get the user's token from the localStorage after login
    let tokenPayload:any; // to decode
    // Decode token
    try{
      tokenPayload = jwtDecode(token);  // 'solve' the user's token
    }catch(err){
      // means that the token is wrong, thus, clear the storage and send him back to the login page
      localStorage.clear();
      this.router.navigate(['/'])
    }

    // Check if the token has the appropriate role for access
    let expectedRole = '';
    // Find the role (that the token must have)
    for(let i=0; i< expectedRoleArray.length; i++){
      if(expectedRoleArray[i] == tokenPayload.role){
        expectedRole = tokenPayload.role;
      }
    }
    // Since found the role, check specific role scenarios
    if(tokenPayload.role == 'user' || tokenPayload.role == 'admin'){
      if(this.auth.isAuthenticated() && tokenPayload.role == expectedRole){
        return true;
      }
      // In case the user tries to access a feature in the dashboard that he is not allowed
      this.snackbarService.openSnackBar(GlobalConstants.unauthorized, GlobalConstants.error);
      this.router.navigate(['/cafe/dashboard']);
      return false;
    }
    else{
      // Else means that the user is not authorized, thus, throw error
      // and navigate back to the login page
      this.router.navigate(['/']);
      localStorage.clear();
      return false;
    }
  }
}
