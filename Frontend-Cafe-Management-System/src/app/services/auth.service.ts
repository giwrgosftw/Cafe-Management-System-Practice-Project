import { Injectable } from '@angular/core';
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private router:Router) { }

  // Return true/false if localStorage has a token
  public isAuthenticated():boolean{
    const token = localStorage.getItem('token');
    if(!token){
      this.router.navigate(['/']);  // navigate back to main page
      return false;
    }else {
      return true;
    }
  }
}
