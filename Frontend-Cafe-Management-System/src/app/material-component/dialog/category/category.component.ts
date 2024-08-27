import {Component, EventEmitter, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {CategoryService} from "../../../services/category.service";
import {SnackbarService} from "../../../services/snackbar.service";
import {GlobalConstants} from "../../../shared/global-constants";

@Component({
  selector: 'app-category', // Defines the HTML tag for this component
  templateUrl: './category.component.html', // Links to the component's HTML template
  styleUrls: ['./category.component.scss'] // Links to the component's CSS styles
})
export class CategoryComponent implements OnInit {

  onAddCategory = new EventEmitter(); // Emits an event when a category is successfully added
  onEditCategory = new EventEmitter(); // Emits an event when a category is successfully edited
  categoryForm:any = FormGroup; // Form group that manages the category form controls/fields
  dialogAction:any = "Add"; // Determines the action type (Add/Edit) based on dialog data
  action:any = "Add"; // Action label used in the UI (Add/Update)
  responseMessage:any; // Stores messages (e.g., success or error) to display to the user

  /**
   * Constructor function that initializes the CategoryComponent.
   * Injects necessary services for form building, category management, dialog control, and snackbar notifications.
   *
   * @param dialogData - Data passed to the dialog, used to determine if the action is Add or Edit.
   * @param formBuilder - Service to handle reactive form creation.
   * @param categoryService - Service to manage category-related operations like add and update.
   * @param dialogRef - Reference to the dialog containing this component, used to close it when needed.
   * @param snackbarService - Service to display messages via a snackbar notification.
   */
  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogData:any,
    private formBuilder:FormBuilder,
    private categoryService:CategoryService,
    public dialogRef: MatDialogRef<CategoryComponent>,
    private snackbarService: SnackbarService
  ) { }

  /**
   * Lifecycle hook that runs when the component is initialized.
   * It sets up the category form and checks if the dialog action is for editing an existing category.
   * If editing, it pre-fills the form with the existing category data.
   */
  ngOnInit(): void {
    this.categoryForm = this.formBuilder.group({
      name:[null, [Validators.required]] // Validates that the name field is required
    });
    if(this.dialogData.action === 'Edit'){
      this.dialogAction = "Edit";
      this.action = "Update";
      this.categoryForm.patchValue(this.dialogData.data); // Pre-fill the form with existing data
    }
  }

  /**
   * Handles adding a new category.
   * It sends the category data to the server, closes the dialog, and emits an event to refresh the category list.
   * In case of an error, it shows an appropriate error message.
   */
  add(){
    let formData = this.categoryForm.value;
    let data = {
      name:formData.name
    }

    this.categoryService.add(data).subscribe((response:any) => {
        this.dialogRef.close(); // Close the dialog after successful addition
        this.onAddCategory.emit(); // Emit event to refresh the category list
        this.responseMessage = response.message;
        this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
      },(error)=>{
        this.dialogRef.close(); // Close the dialog on error
        console.error(error);
        if(error.error?.message){
          this.responseMessage = error.error?.message; // Set specific error message from server
        }else{
          this.responseMessage = GlobalConstants.genericError; // Set a generic error message
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error); // Show error message
      }
    );
  }

  /**
   * Handles editing an existing category.
   * It sends the updated category data to the server, closes the dialog,
   * and emits an event to refresh the category list.
   * In case of an error, it shows an appropriate error message.
   */
  edit(){

    let formData = this.categoryForm.value;
    let data = {
      id:this.dialogData.data.id,
      name:formData.name
    }

    this.categoryService.update(data).subscribe((response:any) => {
        this.dialogRef.close(); // Close the dialog after successful update
        this.onAddCategory.emit(); // Emit event to refresh the category list
        this.responseMessage = response.message;
        this.snackbarService.openSnackBar(this.responseMessage, "success"); // Show success message
      },(error)=>{
        this.dialogRef.close(); // Close the dialog on error
        console.error(error);
        if(error.error?.message){
          this.responseMessage = error.error?.message; // Set specific error message from server
        }else{
          this.responseMessage = GlobalConstants.genericError; // Set a generic error message
        }
        this.snackbarService.openSnackBar(this.responseMessage, GlobalConstants.error); // Show error message
      }
    );

  }

  /**
   * Handles the form submission.
   * It determines whether the action is to add or edit a category and calls the appropriate method.
   */
  handleSubmit(){
    if(this.dialogAction === "Edit"){
      this.edit(); // Call the edit method if the action is Edit
    }
    else{
      this.add(); // Call the add method if the action is Add
    }
  }

}
