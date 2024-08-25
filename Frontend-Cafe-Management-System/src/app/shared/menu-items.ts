/* HERE WE ADD ALL THE TABS/SECTION THAT WILL EXIST ON THE SIDEBAR */

import {Injectable} from "@angular/core";

export interface Menu{
  state:string;
  name:string;
  type:string;
  icon:string;
  role:string;
}

const MENUITEMS = [
  // First-Top tab/section, dashboard tab
  {state: 'dashboard', name:'Dashboard', type:'link', icon:'dashboard', role:''}, // role is empty so that to do not restrict any user
  {state: 'category', name:'Manage Category', type:'link', icon:'category', role:'admin'},
  {state: 'product', name:'Manage Product', type:'link', icon:'inventory_2', role:'admin'},
  {state: 'order', name:'Manage Order', type:'link', icon:'shopping_cart', role:''}
]

@Injectable()
export class MenuItems{
  getMenuItems():Menu[]{
    return MENUITEMS;
  }
}
