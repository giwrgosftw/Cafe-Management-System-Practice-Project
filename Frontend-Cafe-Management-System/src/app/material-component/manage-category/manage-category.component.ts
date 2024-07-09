import { Component, OnInit } from '@angular/core';
import {CategoryService} from "../../services/category.service";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";
import {SnackbarService} from "../../services/snackbar.service";
import {Router} from "@angular/router";
import {NgxUiLoaderService} from "ngx-ui-loader";
import {MatTableDataSource} from "@angular/material/table";
import {GlobalConstants} from "../../shared/global-constants";
import {CategoryComponent} from "../dialog/category/category.component";

@Component({
  selector: 'app-manage-category',
  templateUrl: './manage-category.component.html',
  styleUrls: ['./manage-category.component.scss']
})
export class ManageCategoryComponent implements OnInit {

  // display the columns of the manage-category sidebar's section
  displayedColumns: string[] = ['name', 'edit'];
  dataSource: any;
  responseMessage: any;

  constructor(
    private categoryService: CategoryService,
    private ngxService:NgxUiLoaderService,
    private dialog:MatDialog,
    private snackbarService:SnackbarService,
    private router: Router
  ) { }

  tableData(){
    this.categoryService.getCategories().subscribe((response:any)=>{
      this.ngxService.stop();
      this.dataSource = new MatTableDataSource(response);
    }, (error:any)=>{
        this.ngxService.stop() // stop the loader if an error presents (otherwise it will keep spinning)
        if(error.error?.message){
          this.responseMessage = error.error?.message;
        } else {
          this.responseMessage = GlobalConstants.genericError;
        }
        console.log(this.responseMessage)
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error)
      })
  }

  ngOnInit(): void {
    this.ngxService.start();
    this.tableData();
  }

  // A search function for the table. You type in some text (e.g., "apple"),
  // and it shows you the matching rows, ignoring case differences (only shows rows that have "apple")
  applyFilter(event:Event){
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();  // adapt case-sensitive for the match while searching
  }

  handleAddAction(){
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data ={
      action: 'Add'
    };
    dialogConfig.width = "850px";
    // Whenever the path change (e.g., navigate from one page to another),
    const dialogRef = this.dialog.open(CategoryComponent, dialogConfig);
    this.router.events.subscribe(()=>{
      // we need to close all the e.g., pop up windows left
      dialogRef.close()
    });
    const sub = dialogRef.componentInstance.onAddCategory.subscribe((response)=>{
      this.tableData()
    })
  }

  handleEditAction(values:any){  // expecting some data
    const dialogConfig = new MatDialogConfig();
    dialogConfig.data ={
      action: 'Edit',
      data:values
    };
    dialogConfig.width = "850px";
    // Whenever the path change (e.g., navigate from one page to another),
    const dialogRef = this.dialog.open(CategoryComponent, dialogConfig);
    this.router.events.subscribe(()=>{
      // we need to close all the e.g., pop up windows left
      dialogRef.close()
    });
    const sub = dialogRef.componentInstance.onAddCategory.subscribe((response)=>{
      this.tableData()
    })
  }
}
