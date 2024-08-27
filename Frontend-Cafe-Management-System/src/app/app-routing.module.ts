import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { FullComponent } from './layouts/full/full.component';
import {RouteGuardService} from "./services/route-guard.service";

// Define the application routes
const routes: Routes = [
  {
    path: '',
    component: HomeComponent // Default route loads the HomeComponent
  },
  {
    path: 'cafe',
    component: FullComponent, // FullComponent serves as the layout for all child routes under 'cafe'
    children: [
      {
        path: '',
        redirectTo: '/cafe/dashboard', // Redirect to dashboard when path is empty
        pathMatch: 'full',
      },
      {
        path: '',
        loadChildren: () => import('./material-component/material.module').then(m => m.MaterialComponentsModule),
        canActivate: [RouteGuardService], // Protect this route with the RouteGuardService
        data: {
          expectedRole: ['admin', 'user']  // Allow access only to users with 'admin' or 'user' roles
        }
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./dashboard/dashboard.module').then(m => m.DashboardModule),
        canActivate: [RouteGuardService], // Protect this route with the RouteGuardService
        data: {
          expectedRole: ['admin', 'user']  // Allow access only to users with 'admin' or 'user' roles
        }
      }
    ]
  },
  {
    path: '**',
    component: HomeComponent // Wildcard route to catch any undefined routes and redirect to HomeComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)], // Configure the router at the application's root level
  exports: [RouterModule] // Export RouterModule so it can be used throughout the app
})
export class AppRoutingModule { }
