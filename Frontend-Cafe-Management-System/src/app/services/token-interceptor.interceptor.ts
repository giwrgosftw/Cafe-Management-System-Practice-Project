import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor, HttpResponse
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {Router} from "@angular/router";
import {catchError} from "rxjs/operators";

@Injectable()
export class TokenInterceptorInterceptor implements HttpInterceptor {

  constructor(private router: Router) {
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Get the token from the local storage
    const token = localStorage.getItem('token');
    // Set up the token as a bearer token
    if (token) {
      request = request.clone({
        setHeaders: {Authorization: `Bearer ${token}`}
      });
    }

    return next.handle(request).pipe(
      catchError((err) => {
        if (err instanceof HttpResponse) {
          console.log(err.url);
          if (err.status === 401 || err.status === 403) {
            if (this.router.url === '/') {

            } else {
              localStorage.clear();
              this.router.navigate(['/'])
            }
          }
        }
        return throwError(err);
      })
    )
  }
}
