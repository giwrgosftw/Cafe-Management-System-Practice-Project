import { Injectable } from '@angular/core';
import {Router} from "@angular/router";

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * The 'providedIn: root' option means this service is available application-wide and will be a singleton.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {

  /**
   * Constructor function that initializes the AuthService.
   *
   * @param router - The Angular Router service used for navigating between routes.
   */
  constructor(private router: Router) { }

  /**
   * Checks if the user is authenticated by verifying the existence of a token in localStorage.
   * If the token is not found, the user is redirected to the main page.
   *
   * @returns boolean - Returns `true` if a token exists in localStorage, otherwise returns `false`.
   */
  public isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    if(!token) {
      this.router.navigate(['/']);  // Navigate back to the main page if the token is not present
      return false;
    }else {
      return true;
    }
  }
}
