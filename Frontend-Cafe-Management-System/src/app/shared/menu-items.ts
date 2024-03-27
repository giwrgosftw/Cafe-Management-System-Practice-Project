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
  {state: 'dashboard', name:'Dashboard', type:'link', icon:'dashboard', role:''} // role is empty so that to do not restrict any user
]

@Injectable()
export class MenuItems{
  getMenuItems():Menu[]{
    return MENUITEMS;
  }
}
