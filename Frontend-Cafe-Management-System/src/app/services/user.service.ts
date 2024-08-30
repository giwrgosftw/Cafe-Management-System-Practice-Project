import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient, HttpHeaders} from '@angular/common/http';

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * This service handles all user-related operations, such as signup, login, password management, and user management.
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {

  url = environment.apiUrl; // The base URL for the API, taken from the environment.ts.

  /**
   * Constructor function that initializes the UserService.
   *
   * @param httpClient - The Angular HttpClient service used for making HTTP requests.
   */
  constructor(private httpClient: HttpClient) { }

  /**
   * Sends a signup request to the backend with user data.
   *
   * @param data - The user data to be sent to the backend.
   * @returns An observable for the HTTP POST request.
   */
  signup(data: any) {
    return this.httpClient.post(
      this.url + '/user/signup',
      data, // The data to be sent in the request body.
      { headers: new HttpHeaders().set('Content-Type', 'application/json') } // Ensure JSON content type.
    );
  }

  /**
   * Sends a forgot password request to the backend.
   *
   * @param data - The data (usually the user's email) to be sent to the backend.
   * @returns An observable for the HTTP POST request.
   */
  forgotPassword(data: any) {
    return this.httpClient.post(
      this.url + "/user/forgotPassword",
      data,
      { headers: new HttpHeaders().set('Content-Type', 'application/json') }
    );
  }

  /**
   * Sends a login request to the backend with user credentials.
   *
   * @param data - The user credentials (email and password) to be sent to the backend.
   * @returns An observable for the HTTP POST request.
   */
  login(data: any) {
    return this.httpClient.post(
      this.url + "/user/login",
      data,
      { headers: new HttpHeaders().set('Content-Type', 'application/json') }
    );
  }

  /**
   * Checks the user's token for validity, typically used to authorize access to certain features like the dashboard.
   *
   * @returns An observable for the HTTP GET request.
   */
  checkToken() {
    this.httpClient.get(this.url + "/user/checkToken");  // TODO: Implemented that API in the backend
  }

  /**
   * Sends a change password request to the backend with the user's old and new passwords.
   *
   * @param data - The password data to be sent to the backend.
   * @returns An observable for the HTTP POST request.
   */
  changePassword(data: any) {
    return this.httpClient.post(
      this.url + "/user/changePassword/",
      data,
      { headers: new HttpHeaders().set('Content-Type', 'application/json') }
    );
  }

  /**
   * Retrieves a list of all users from the backend.
   *
   * @returns An observable for the HTTP GET request.
   */
  getUsers() {
    return this.httpClient.get(this.url + "/user/getAllUsers");
  }

  /**
   * Sends an update request to the backend to update user information.
   *
   * @param data - The updated user data to be sent to the backend.
   * @returns An observable for the HTTP POST request.
   */
  update(data: any) {
    return this.httpClient.post(
      this.url + '/user/update',
      data,
      { headers: new HttpHeaders().set('Content-Type', 'application/json') }
    );
  }
}
