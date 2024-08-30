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

  canActivate(route:ActivatedRouteSnapshot): boolean {
    // Initialization
    const expectedRoleArray = route.data?.expectedRole || [];
    const token: string | null = localStorage.getItem('token');  // get the user's token from localStorage

    if (!token) {
      // No token found, navigate to login
      this.router.navigate(['/']);
      return false;
    }

    let tokenPayload: any;

    try {
      tokenPayload = jwtDecode(token);  // decode the user's token
    } catch (err) {
      // Invalid token, clear storage and navigate to login
      localStorage.clear();
      this.router.navigate(['/']);
      return false;
    }

    const userRole = tokenPayload.role?.toLowerCase();

    if (!userRole) {
      // If no role is found in the token, redirect to login
      this.snackbarService.openSnackBar(GlobalConstants.unauthorized, GlobalConstants.error);
      this.router.navigate(['/']);
      return false;
    }

    // Check if the token's role matches any of the expected roles
    const matchedRole = expectedRoleArray.find((role: string) => role.toLowerCase() === userRole);

    if (matchedRole && this.auth.isAuthenticated()) {
      // If role is matched and user is authenticated, allow access
      return true;
    } else {
      // Unauthorized access, redirect to dashboard
      this.snackbarService.openSnackBar(GlobalConstants.unauthorized, GlobalConstants.error);
      this.router.navigate(['/cafe/dashboard']);
      return false;
    }
  }
}
