import {Component, inject, OnDestroy, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {Router, RouterLink} from "@angular/router";
import {NgOptimizedImage} from '@angular/common';
import {AuthService} from '../../services/auth-service';
import {Subject, takeUntil} from 'rxjs';


@Component({
  selector: 'app-header',
  imports: [RouterLink, NgOptimizedImage],
  templateUrl: './header.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './header.css'
})
export class Header implements OnInit, OnDestroy {

  // Subject to manage the lifecycle of subscriptions and prevent memory leaks
  private destroySubject = new Subject();

  // Boolean flag to indicate if the user is logged in
  isLoggedIn: boolean = false;

  // Store the email of the logged-in user, initialized to null
  loggedInUserEmail: string | null = null;

  // Injecting AuthService and Router into the component
  /**
   * Constructor for initializing the class with dependencies and setting up subscriptions.
   *
   * @param {AuthService} authService - Service to handle authentication-related operations.
   * @param {Router} router - Router service to navigate between application routes.
   * @return {void} Initializes the component and sets up authentication status subscription.
   */
  constructor(private authService: AuthService, private router: Router) {
    this.authService.authStatus$
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
        this.loggedInUserEmail = result ? this.authService.getUserEmail() : null;
      });
  }

  // Method to handle user logout
  /**
   * Logs out the current user by invoking the logout method of the authentication service
   * and navigates the user to the login page.
   *
   * @return {void} No return value.
   */
  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Initialize the component and set the initial login status
  /**
   * Lifecycle hook that is called after the component's view has been initialized.
   * Initializes the component by determining the authentication status of the user.
   * Sets the `isLoggedIn` property based on the authentication status retrieved from the AuthService.
   *
   * @return {void} This method does not return any value.
   */
  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
  }

  // Clean up subscriptions to prevent memory leaks
  /**
   * A lifecycle hook that is called when the component is destroyed.
   * It completes and cleans up the `destroySubject` to prevent memory leaks.
   *
   * @return {void} No return value.
   */
  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }
}
