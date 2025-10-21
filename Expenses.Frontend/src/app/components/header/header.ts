import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {Router, RouterLink} from "@angular/router";
import {NgOptimizedImage} from '@angular/common';
import {AuthService} from '../../services/auth-service';
import {Subject, takeUntil} from 'rxjs';


@Component({
  selector: 'app-header',
  imports: [RouterLink, NgOptimizedImage],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class Header implements OnInit, OnDestroy {

  private destroySubject = new Subject();
  isLoggedIn: boolean = false;
  loggedInUserEmail: string | null = null;

  constructor(private authService: AuthService, private router: Router) {
    this.authService.authStatus$
      .pipe(takeUntil(this.destroySubject))
      .subscribe(result => {
        this.isLoggedIn = result;
        this.loggedInUserEmail = result ? this.authService.getUserEmail() : null;
      });
  }

  // Method to handle user logout
  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Initialize the component and set the initial login status
  ngOnInit(): void {
    this.isLoggedIn = this.authService.isAuthenticated();
  }

  // Clean up subscriptions to prevent memory leaks
  ngOnDestroy(): void {
    this.destroySubject.next(true);
    this.destroySubject.complete();
  }
}
