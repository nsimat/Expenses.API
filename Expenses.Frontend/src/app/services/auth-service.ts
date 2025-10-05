import { LoginResult } from './../models/login-result';
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
  private readonly http = inject(HttpClient);
  private readonly apiAuthUrl = environment.apiAuthUrl;
  private readonly router = inject(Router);

  public tokenKey: string = 'token';

  login(credentials: User): Observable<LoginResult> {
    console.log('Login in the system...');
    var url = this.apiAuthUrl + '/Login';

    return this.http.post<LoginResult>(url, credentials).pipe(
      tap((LoginResult) => {
        if (LoginResult.success && LoginResult.token) {
          console.log('Login accepted.');
          localStorage.setItem(this.tokenKey, LoginResult.token);
        }
      })
    );
  }

  register(credentials: User): Observable<any> {
    console.log('Registering user with credentials: ', credentials);

    var url = this.apiAuthUrl + '/Register';

    return this.http.post<any>(url, credentials).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        console.log('User accepted with credentials: ', credentials);
      })
    );
  }

  logout(): void {
    // Implement logout logic if needed, e.g., clear tokens, notify server, etc.
    console.log('User logged out...');
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }
}
