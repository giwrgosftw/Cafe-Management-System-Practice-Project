import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders} from "@angular/common/http";

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * The 'providedIn: root' option means this service is available application-wide and will be a singleton.
 */
@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  url = environment.apiUrl;  // Base URL for API endpoints

  /**
   * Constructor function that initializes the CategoryService.
   *
   * @param httpClient - The Angular HttpClient service used for making HTTP requests.
   */
  constructor(private httpClient: HttpClient) { }

  /**
   * Adds a new category by sending a POST request to the server.
   *
   * @param data - The data to be sent in the body of the POST request.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  add(data: any) {
    return this.httpClient.post(
      this.url + "/category/add",
      data,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    );
  }

  /**
   * Updates an existing category by sending a POST request to the server.
   *
   * @param data - The data to be sent in the body of the POST request.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  update(data: any) {
    return this.httpClient.post(
      this.url + "/category/update",
      data,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    );
  }

  /**
   * Retrieves a list of all categories from the server.
   *
   * @returns Observable<any> - An observable that contains the list of categories.
   */
  getCategories() {
    return this.httpClient.get(this.url + "/category/get");
  }

  /**
   * Retrieves a filtered list of categories from the server.
   *
   * @returns Observable<any> - An observable that contains the filtered list of categories.
   */
  getFilteredCategories() {
    return this.httpClient.get(this.url + "/category/get?filterValue=true");
  }

}
