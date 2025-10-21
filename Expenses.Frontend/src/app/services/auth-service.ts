import { LoginResult } from '../models/login-result';
import {HttpClient, HttpResponse} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from '../models/login-request';
import {BehaviorSubject, Observable, tap} from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // Dependency Injections using inject() function
  private readonly http = inject(HttpClient);
  private readonly apiAuthUrl = environment.apiAuthUrl;
  private readonly router = inject(Router);

  // Key to store the token in local storage
  private tokenKey: string = "token";
  // Public field to receive the email of the logged-in user
  private userEmail: string = 'userEmail';
  // BehaviorSubject to manage authentication status
  private _authStatus = new BehaviorSubject<boolean>(false);
  public authStatus$ = this._authStatus.asObservable();

  // check if the user is authenticated by checking if the token exists in local storage
  isAuthenticated(): boolean {
    return this.getToken() !== null; // Returns true if token exists, false otherwise
  }

  // get the user email
  getUserEmail(): string | null {
    return localStorage.getItem('userEmail');
  }
  // Retrieve the token from local storage
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Initialize the authentication status on service creation
  initializeAuthStatus(): void {
    const isAuth = this.isAuthenticated();
    this.setAuthStatus(isAuth);
  }

  // Authenticate the user and store the token
  login(credentials: LoginRequest): Observable<LoginResult> {
    console.log('Login in the system...');
    const url = this.apiAuthUrl + '/Login';

    return this.http.post<LoginResult>(url, credentials).pipe(
      tap((loginResult) => {
        if (loginResult.success && loginResult.token) {
          console.log('Login accepted for: ', credentials.email);
          localStorage.setItem(this.tokenKey, loginResult.token);
          localStorage.setItem(this.userEmail, credentials.email);
          this.setAuthStatus(true);
        }
      })
    );
  }

  // Register by creating a new user and store the token
  register(credentials: LoginRequest): Observable<any> {
    console.log('Registering user with credentials: ', credentials);

    const url = this.apiAuthUrl + '/Register';
    return this.http.post<any>(url, credentials).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        console.log('LoginRequest accepted with credentials: ', credentials);
      })
    );
  }

  // Logout by clearing the token and navigate to the login page
  logout(): void {
    // Implement logout logic if needed, e.g., clear tokens, notify server, etc.
    console.log('LoginRequest logged out...');
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userEmail);
    this.setAuthStatus(false);
    this.router.navigate(['/login']);
  }

  // Update the authentication status
  private setAuthStatus(isAuthenticated: boolean): void {
    this._authStatus.next(isAuthenticated);
  }
}
