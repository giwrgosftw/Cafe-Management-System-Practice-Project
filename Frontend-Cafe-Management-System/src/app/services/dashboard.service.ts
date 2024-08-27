import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * The 'providedIn: root' option means this service is available application-wide and will be a singleton.
 */
@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  url = environment.apiUrl;  // Base URL for API endpoints

  /**
   * Constructor function that initializes the DashboardService.
   *
   * @param httpClient - The Angular HttpClient service used for making HTTP requests.
   */
  constructor(private httpClient: HttpClient) { }

  /**
   * Fetches dashboard details by sending a GET request to the server.
   *
   * @returns Observable<any> - An observable that contains the server's response with dashboard details.
   */
  getDetails() {
    return this.httpClient.get(this.url + "/dashboard/details");
  }
}
