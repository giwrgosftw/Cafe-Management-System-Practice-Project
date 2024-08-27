import { Component, OnInit, Inject } from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-view-bill-products', // The custom HTML tag used for this component
  templateUrl: './view-bill-products.component.html', // The HTML layout for this component
  styleUrls: ['./view-bill-products.component.scss'] // The styles applied to this component
})
export class ViewBillProductsComponent implements OnInit {

  // Columns to be displayed in the table
  displayedColumns: string[] = ['name', 'category', 'price', 'quantity', 'total'];
  dataSource: any; // The data source for the table, containing product details
  data: any; // Holds the complete data passed into the dialog

  /**
   * Constructor function that initializes the ViewBillProductsComponent.
   * Injects the dialog data and dialog reference for managing the dialog.
   *
   * @param dialogData - Data passed to the dialog, containing information about the bill.
   * @param dialogRef - Reference to the dialog containing this component, used to close it when needed.
   */
  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogData:any,
    public dialogRef:MatDialogRef<ViewBillProductsComponent>
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It extracts the data passed to the dialog and parses the product details to display in the table.
   */
  ngOnInit() {
    this.data = this.dialogData.data; // Extracts the main data object from the dialog data
    this.dataSource = JSON.parse(this.dialogData.data.productDetails); // Parses the product details from JSON to an array
    console.log(this.data);
  }

}
