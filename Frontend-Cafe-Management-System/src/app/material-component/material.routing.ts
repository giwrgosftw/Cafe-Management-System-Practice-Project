import { Routes } from '@angular/router';
import { DashboardComponent } from '../dashboard/dashboard.component';
import {ManageCategoryComponent} from "./manage-category/manage-category.component";
import {RouteGuardService} from "../services/route-guard.service";
import {ManageProductComponent} from "./manage-product/manage-product.component";
import {ManageOrderComponent} from "./manage-order/manage-order.component";
import {ViewBillComponent} from "./view-bill/view-bill.component";


export const MaterialRoutes: Routes = [
  {
    path: 'category',
    component:ManageCategoryComponent,
    canActivate:[RouteGuardService],
    data:{
      expectedRole: ['admin'] // Categories tab viewable only to admins
    }
  },
  {
    path: 'product',
    component:ManageProductComponent,
    canActivate:[RouteGuardService],
    data:{
      expectedRole: ['admin']
    }
  },
  {
    path: 'order',
    component:ManageOrderComponent,
    canActivate:[RouteGuardService],
    data:{
      expectedRole: ['admin', 'user']
    },
  },
  {
    path: 'bill',
    component: ViewBillComponent,
    canActivate: [RouteGuardService],
    data: {
      expectedRole: ['admin', 'user']
    }
  }
];
