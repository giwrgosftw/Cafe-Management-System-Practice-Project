import { ChangeDetectorRef, Component, OnDestroy } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import {jwtDecode} from "jwt-decode";
import {MenuItems} from "../../../shared/menu-items";

@Component({
  selector: 'app-sidebar', // Defines the HTML tag for this component
  templateUrl: './sidebar.component.html', // Links to the component's HTML template
  styleUrls: [] // Currently, no specific styles are associated with this component
})
/**
 * The AppSidebarComponent manages the sidebar of the application.
 * It handles responsive design for mobile devices and displays menu items based on the user's role.
 */
export class AppSidebarComponent implements OnDestroy {
  mobileQuery: MediaQueryList; // Tracks whether the screen size matches mobile view
  userRole: any; // Stores the role of the user, used to filter menu items
  token: any = localStorage.getItem('token');  // Retrieves the token from local storage for authentication
  tokenPayload: any; // Decoded token payload to access user information, such as role

  private _mobileQueryListener: () => void; // Listener for changes in screen size

  /**
   * Constructor function that initializes the AppSidebarComponent.
   * It decodes the user's token to extract their role, sets up a listener for screen size changes,
   * and initializes the list of menu items accessible from the sidebar.
   *
   * @param changeDetectorRef - Service to trigger change detection in Angular.
   * @param media - Service to match the media query for responsive design.
   * @param MenuItems - Service that provides access to the menu items to be displayed in the sidebar.
   */
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
    public MenuItems: MenuItems // Provide access to menu items in the sidebar
  ) {
    // Decode the token to get user information
    this.tokenPayload = jwtDecode(this.token);
    this.userRole = this.tokenPayload?.role; // Extract the user role from the token payload

    // Set up a media query listener for screen size changes
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  /**
   * Lifecycle hook that runs when the component is destroyed.
   * It removes the media query listener to prevent memory leaks.
   */
  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener); // Clean up the media query listener
  }
}
