import { Component, AfterViewInit } from '@angular/core';
import {DashboardService} from "../services/dashboard.service";
import {SnackbarService} from "../services/snackbar.service";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {GlobalConstants} from "../shared/global-constants";
@Component({
	selector: 'app-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements AfterViewInit {

  responseMessage:any;
  data:any;
	ngAfterViewInit() { }

	constructor(private dashboardService:DashboardService,
              private ngxService:NgxUiLoaderService,
              private snackbarService:SnackbarService,
              ) {
    this.ngxService.start();
    this.dashboardData();
	}

  // Load dashboard (all the infos from the rest entities)
  dashboardData(){
    // while accessing the details
    this.dashboardService.getDetails().subscribe((response:any)=> {
      this.ngxService.start(); // start the loader
      this.data = response; // get the data response
    },(error:any)=>{
      // if something goes wrong
      this.ngxService.stop(); // stop the loader
      // Show the error
      console.log(error);
      if(error.error?.message){
        this.responseMessage = error.error?.message; // show specific error message
      }
      else{
        this.responseMessage = GlobalConstants.genericError; // show generic
      }
      this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error) // display the message using snackbar
    })
  }



}
