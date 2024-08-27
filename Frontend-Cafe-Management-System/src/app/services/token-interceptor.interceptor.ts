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

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * This service implements the HttpInterceptor interface to intercept HTTP requests and responses.
 */
@Injectable()
export class TokenInterceptorInterceptor implements HttpInterceptor {

  /**
   * Constructor function that initializes the TokenInterceptorInterceptor.
   *
   * @param router - The Angular Router service used for navigation.
   */
  constructor(private router: Router) {
  }

  /**
   * Intercepts HTTP requests to add an Authorization header with a Bearer token if available.
   * Also handles errors like 401 Unauthorized or 403 Forbidden by clearing local storage
   * and redirecting the user to the main page.
   *
   * @param request - The outgoing HTTP request to be intercepted.
   * @param next - The next handler in the HTTP request pipeline.
   * @returns Observable<HttpEvent<unknown>> - An observable that contains the HTTP event (request or response).
   */
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Get the token from local storage
    const token = localStorage.getItem('token');

    // If token exists, clone the request and set the Authorization header (set as Bearer)
    if (token) {
      request = request.clone({
        setHeaders: {Authorization: `Bearer ${token}`}
      });
    }

    // Handle the request and catch errors
    return next.handle(request).pipe(
      catchError((err) => {
        // If the error is an HttpResponse and status is 401 or 403, clear local storage and navigate to the main page
        if (err instanceof HttpResponse) {
          console.log(err.url);
          if (err.status === 401 || err.status === 403) {
            if (this.router.url === '/') {
              // Do nothing if already on the main page
            } else {
              localStorage.clear();
              this.router.navigate(['/']);
            }
          }
        }
        // Return the error to be handled by the calling code
        return throwError(err);
      })
    );
  }
}
