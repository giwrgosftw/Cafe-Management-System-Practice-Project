import { Injectable } from '@angular/core';
import {MatSnackBar} from "@angular/material/snack-bar";

@Injectable({
  providedIn: 'root'
})
export class SnackbarService {

  constructor(private snackBar: MatSnackBar) { }

  // A notification: e.g., display message response from the backend
  openSnackBar(message: string, action: string){
    // If there is an error, display an error notification
    if (action === 'error'){
      this.snackBar.open(
        message,
        '',
  {
        horizontalPosition: 'center',
        verticalPosition: 'top',
        duration: 2000, // 2 sec
        panelClass: ['black-snackbar'] // from style.css
        }
      );
    }
    else{
      this.snackBar.open(
        message,
        '',
        {
          horizontalPosition: 'center',
          verticalPosition: 'top',
          duration: 2000, // 2 sec
          panelClass: ['green-snackbar'] // from style.css
        }
      );
    }
  }

}
