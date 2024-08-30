import { Routes } from '@angular/router';
import {ManageCategoryComponent} from "./manage-category/manage-category.component";
import {RouteGuardService} from "../services/route-guard.service";
import {ManageProductComponent} from "./manage-product/manage-product.component";
import {ManageOrderComponent} from "./manage-order/manage-order.component";
import {ViewBillComponent} from "./view-bill/view-bill.component";
import {ManageUserComponent} from "./manage-user/manage-user.component";

/**
 * Defines the routes for the Material module, which includes various management and viewing components.
 * Each route is protected by a route guard that ensures only users with the correct roles can access them.
 */
export const MaterialRoutes: Routes = [
  {
    path: 'category',
    component: ManageCategoryComponent,
    canActivate: [RouteGuardService],
    data: {
      expectedRole: ['Admin'] // Categories tab viewable only to admins
    }
  },
  {
    path: 'product',
    component: ManageProductComponent,
    canActivate: [RouteGuardService],
    data: {
      expectedRole: ['Admin'] // Products tab viewable only to admins
    }
  },
  {
    path: 'order',
    component: ManageOrderComponent,
    canActivate: [RouteGuardService],
    data: {
      expectedRole: ['Admin', 'User'] // Orders tab viewable to both admins and users
    },
  },
  {
    path: 'bill',
    component: ViewBillComponent,
    canActivate: [RouteGuardService],
    data: {
      expectedRole: ['Admin', 'User'] // Bills tab viewable to both admins and users
    }
  },
  {
    path: 'user',
    component: ManageUserComponent,
    canActivate: [RouteGuardService],
    data: {
      expectedRole: ['Admin'] // Users tab viewable only to admins
    }
  }
];
