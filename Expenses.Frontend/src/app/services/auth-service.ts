import { LoginResult } from '../models/login-result';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { User } from '../models/user';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  // Dependency Injections using inject() function
  private readonly http = inject(HttpClient);
  private readonly apiAuthUrl = environment.apiAuthUrl;
  private readonly router = inject(Router);

  public tokenKey: string = "token";

  // check if the user is authenticated by checking if the token exists in local storage
  isAuthenticated(): boolean {
    return this.getToken() !== null; // Returns true if token exists, false otherwise
  }

  // Retrieve the token from local storage
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Authenticate the user and store the token
  login(credentials: User): Observable<LoginResult> {
    console.log('Login in the system...');
    const url = this.apiAuthUrl + '/Login';

    return this.http.post<LoginResult>(url, credentials).pipe(
      tap((loginResult) => {
        if (loginResult.success && loginResult.token) {
          console.log('Login accepted for: ', credentials.email);
          localStorage.setItem(this.tokenKey, loginResult.token);
        }
      })
    );
  }

  // Register by creating a new user and store the token
  register(credentials: User): Observable<any> {
    console.log('Registering user with credentials: ', credentials);

    const url = this.apiAuthUrl + '/Register';

    return this.http.post<any>(url, credentials).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        console.log('User accepted with credentials: ', credentials);
      })
    );
  }

  // Logout by clearing the token and navigate to the login page
  logout(): void {
    // Implement logout logic if needed, e.g., clear tokens, notify server, etc.
    console.log('User logged out...');
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }
}
