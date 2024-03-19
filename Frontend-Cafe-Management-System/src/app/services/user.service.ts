import { Injectable } from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient, HttpHeaders} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  url = environment.apiUrl;
  constructor(private httpClient: HttpClient) { }
  signup(data: any){
    return this.httpClient.post(
      this.url + '/user/signup',
      data, // what comes from the backend
      {headers: new HttpHeaders().set('Content-Type', 'application/json')} // ensures that we get JSON
    );
  }

  forgotPassword(data:any){
    return this.httpClient.post(
      this.url + "user/forgotPassword",
      data,
      {headers: new HttpHeaders().set('Content-Type', 'application/json')}
    );
  }

}
