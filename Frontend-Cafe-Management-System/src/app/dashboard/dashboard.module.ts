import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { DashboardComponent } from './dashboard.component';
import { DashboardRoutes } from './dashboard.routing';
import { MaterialModule } from '../shared/material-module';

/**
 * @NgModule decorator marks this class as an Angular module.
 * This module is responsible for handling everything related to the dashboard feature.
 * It imports necessary modules and declares the components that are part of the dashboard.
 */
@NgModule({
  imports: [
    CommonModule,
    /**
     * CommonModule provides common Angular directives (like ngIf, ngFor) and pipes.
     * It is imported to make these common functionalities available within the dashboard module.
     */

    MaterialModule,
    /**
     * MaterialModule is a custom module that bundles Angular Material components.
     * It is imported to provide Material Design components (like buttons, cards, form fields) used in the dashboard.
     */

    FlexLayoutModule,
    /**
     * FlexLayoutModule provides layout utilities based on CSS flexbox.
     * It is used to create responsive, adaptive layouts for the dashboard UI, making it mobile-friendly.
     */

    RouterModule.forChild(DashboardRoutes)
    /**
     * RouterModule.forChild is used to define child routes specific to the Dashboard module.
     * This allows the dashboard to have its own routing configuration, separate from the rest of the application.
     */
  ],
  declarations: [
    DashboardComponent //Declared here so it can be used within this module, serving as the primary view for the dashboard.
  ]
})

export class DashboardModule { }
