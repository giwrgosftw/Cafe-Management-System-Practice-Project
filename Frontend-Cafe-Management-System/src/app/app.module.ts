import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './shared/material-module';
import { HomeComponent } from './home/home.component';
import { BestSellerComponent } from './best-seller/best-seller.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FlexLayoutModule } from '@angular/flex-layout';
import { SharedModule } from './shared/shared.module';
import { FullComponent } from './layouts/full/full.component';
import { AppHeaderComponent } from './layouts/full/header/header.component';
import { AppSidebarComponent } from './layouts/full/sidebar/sidebar.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import { SignupComponent } from './signup/signup.component';
import {NgxUiLoaderConfig, NgxUiLoaderModule, SPINNER} from "ngx-ui-loader";
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { LoginComponent } from './login/login.component';
import {TokenInterceptorInterceptor} from "./services/token-interceptor.interceptor";

// Configuration for the NgxUiLoader, a customizable loading indicator library
const ngxUiLoaderConfig: NgxUiLoaderConfig = {
  text: "Loading...", // Text displayed during loading
  textColor: "#FFFFFF", // White color for the text, ensuring high contrast on dark backgrounds
  textPosition: "center-center", // Position of the text in the center of the screen
  bgsColor: "#7b1fa2", // Background spinner color is purple, matching the primary theme of the app
  fgsColor: "#7b1fa2", // Foreground spinner color is also purple, maintaining consistency with the app's color scheme
  fgsType: SPINNER.squareJellyBox, // Type of spinner animation
  fgsSize: 100, // Size of the spinner
  hasProgressBar: false // Progress bar is disabled for this loader
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    BestSellerComponent,
    FullComponent,
    AppHeaderComponent,
    AppSidebarComponent,
    SignupComponent,
    ForgotPasswordComponent,
    LoginComponent
   ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    MaterialModule,
    FlexLayoutModule,
    SharedModule,
    HttpClientModule,
    NgxUiLoaderModule.forRoot(ngxUiLoaderConfig) // Import a loader including the above custom UiLoader configuration
  ],
  // Configure for the BEARER tokens
  providers: [HttpClientModule, {provide:HTTP_INTERCEPTORS, useClass:TokenInterceptorInterceptor, multi:true }],
  bootstrap: [AppComponent]  // Bootstraps the root component (AppComponent) when the application starts
})
export class AppModule { }
