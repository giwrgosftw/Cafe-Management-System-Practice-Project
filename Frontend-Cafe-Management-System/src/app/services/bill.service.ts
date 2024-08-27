import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";

/**
 * @Injectable decorator marks this class as a service that can be injected into other components or services.
 * The 'providedIn: root' option means this service is available application-wide and will be a singleton.
 */
@Injectable({
  providedIn: 'root'
})
export class BillService {

  url = environment.apiUrl;  // Base URL for API endpoints

  /**
   * Constructor function that initializes the BillService.
   *
   * @param httpClient - The Angular HttpClient service used for making HTTP requests.
   */
  constructor(private httpClient: HttpClient) { }

  /**
   * Generates a bill report in PDF format by sending a POST request to the server.
   *
   * @param data - The data to be sent in the body of the POST request.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  generateReport(data: any) {
    return this.httpClient.post(
      this.url + "/bill/generateBillReportPdf",
      data,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    );
  }

  /**
   * Retrieves a PDF file from the server based on the provided data.
   *
   * @param data - The data to be sent in the body of the POST request.
   * @returns Observable<Blob> - An observable that emits the Blob containing the PDF file.
   */
  getPdf(data: any): Observable<Blob> {
    return this.httpClient.post(
      this.url + "/bill/getPdf",
      data,
      { responseType: 'blob' }
    );
  }

  /**
   * Retrieves a list of all bills from the server.
   *
   * @returns Observable<any> - An observable that contains the list of bills.
   */
  getBills() {
    return this.httpClient.get(this.url + "/bill/GetAllBills");
  }

  /**
   * Deletes a bill by sending a POST request to the server with the bill ID.
   *
   * @param id - The ID of the bill to be deleted.
   * @returns Observable<any> - An observable that contains the server's response.
   */
  delete(id: any) {
    return this.httpClient.post(
      this.url + "/bill/delete/" + id,
      { headers: new HttpHeaders().set('Content-Type', "application/json") }
    );
  }
}
