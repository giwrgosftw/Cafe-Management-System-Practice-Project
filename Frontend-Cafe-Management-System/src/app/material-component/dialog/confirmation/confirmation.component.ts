/*
Show confirmation details to the user, and it includes functionality to emit
an event when a user takes action on the dialog
 */

import {Component, EventEmitter, Inject, inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-confirmation', // The custom HTML tag used for this component
  templateUrl: './confirmation.component.html', // The HTML layout for this component
  styleUrls: ['./confirmation.component.scss'] // The styles applied to this component
})

// ConfirmationComponent implements the OnInit lifecycle hook interface,
// which means it contains an ngOnInit() method that will be called once the component is initialized
export class ConfirmationComponent implements OnInit {

  // EventEmitter instance which will be used to emit events when the status changes
  onEmitStatusChange = new EventEmitter();
  details: any = {};

  // The constructor injects data passed from the Material dialog using MAT_DIALOG_DATA token.
  // This data is accessible via this.dialogData in the component
  constructor(@Inject(MAT_DIALOG_DATA) public dialogData: any) { }

  ngOnInit(): void {
    // If dialogData exists and has a 'confirmation' property, set 'details' to the passed data
    if (this.dialogData && this.dialogData.confirmation) {
      this.details = this.dialogData;
    }
  }

  // A method that will emit a custom event when the user performs an action that needs to
  // change a status. Components that listen for this event can react to status changes.
  // It's like it's announcing to any other related parts/component of the app that are listening:
  // "Hey, something just happened over here!"
  handleChangeAction() {
    this.onEmitStatusChange.emit();
  }
}
