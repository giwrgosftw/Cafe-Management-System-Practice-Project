import { Routes } from '@angular/router';

import { DashboardComponent } from './dashboard.component';


// DashboardComponent should be loaded when the path is ''
export const DashboardRoutes: Routes = [{
  path: '',
  component: DashboardComponent
}];
