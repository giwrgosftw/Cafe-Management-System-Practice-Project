import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class BillService {

  url = environment.apiUrl;
  constructor(private httpClient:HttpClient) { }

  generateReport(data:any){
    return this.httpClient.post(
      this.url + "/bill/generateBillReportPdf",
      data,
      {headers:new HttpHeaders().set('Content-Type', "application/json")}
    )
  }

  // @ts-ignore
  getPdf(data:any):Observable<Blob>{
    return this.httpClient.post(
      this.url + "/bill/getPdf",
      data,
      {responseType:'blob'})
  }

  getBills(){
    return this.httpClient.get(this.url + "/bill/GetAllBills");
  }
}
