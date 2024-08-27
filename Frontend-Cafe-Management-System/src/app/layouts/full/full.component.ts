import { MediaMatcher } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, OnDestroy, AfterViewInit } from '@angular/core';

/**
 * @title Responsive sidenav
 * The FullComponent manages a full-page layout with a responsive sidebar.
 * It listens for screen size changes to adapt the layout accordingly.
 */
@Component({
  selector: 'app-full-layout', // Defines the HTML tag for this component
  templateUrl: 'full.component.html', // Links to the component's HTML template
  styleUrls: [] // Currently, no specific styles are associated with this component
})
export class FullComponent implements OnDestroy, AfterViewInit {
  mobileQuery: MediaQueryList; // Tracks whether the screen size matches mobile view (width >= 768px)

  private _mobileQueryListener: () => void; // Listener for changes in screen size

  /**
   * Constructor function that initializes the FullComponent.
   * It sets up a listener for screen size changes to manage responsive layout behavior.
   *
   * @param changeDetectorRef - Service to trigger change detection in Angular when the screen size changes.
   * @param media - Service to match the media query for responsive design.
   */
  constructor(
    changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher
  ) {
    // Initialize the media query for detecting mobile view (width >= 768px)
    this.mobileQuery = media.matchMedia('(min-width: 768px)');
    // Set up a listener to trigger change detection when the media query matches or changes
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

  /**
   * Lifecycle hook that runs after the component's view has been fully initialized.
   * This can be used for additional initialization tasks or DOM manipulation.
   */
  ngAfterViewInit() { }
}
