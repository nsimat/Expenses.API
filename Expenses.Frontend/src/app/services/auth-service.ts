import { LoginResult } from '../models/login-result';
import {HttpClient} from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from '../models/login-request';
import {BehaviorSubject, catchError, map, Observable, of, tap, throwError} from 'rxjs';
import { Router } from '@angular/router';
import {User} from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // Dependency Injections using inject() function
  private readonly http = inject(HttpClient);

  // Base URL for authentication-related API endpoints, sourced from environment configuration
  private readonly apiAuthUrl = environment.apiAuthUrl;

  // Router for navigation after login/logout actions
  private readonly router = inject(Router);

  // Key to store the token in local storage
  private tokenKey: string = "token";

  // Public field to receive the email of the logged-in user
  private userEmail: string = 'userEmail';

  // BehaviorSubject to manage authentication status
  private _authStatus = new BehaviorSubject<boolean>(false);

  // Observable to allow other components to subscribe to authentication status changes
  public authStatus$ = this._authStatus.asObservable();

  // check if the user is authenticated by checking if the token exists in local storage
  /**
   * Checks whether the user is authenticated by verifying the presence of a token.
   *
   * @return {boolean} True if a token exists, otherwise false.
   */
  isAuthenticated(): boolean {
    return this.getToken() !== null; // Returns true if token exists, false otherwise
  }

  // get the user email
  /**
   * Retrieves the user's email address from local storage.
   *
   * @return {string|null} The user's email address if found, or null if not present.
   */
  getUserEmail(): string | null {
    return localStorage.getItem('userEmail');
  }

  // Retrieve the token from local storage
  /**
   * Retrieves the token stored in the local storage.
   *
   * @return {string | null} The token as a string if it exists, or null if not found.
   */
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  // Initialize the authentication status on service creation
  /**
   * Initializes the authentication status by checking the user's authentication state
   * and updating the relevant status accordingly.
   *
   * @return {void} No return value.
   */
  initializeAuthStatus(): void {
    const isAuth = this.isAuthenticated();
    this.setAuthStatus(isAuth);
  }

  // Check if an email is already registered
  isEmailRegistered(email: string): Observable<boolean>{
    console.log(`Checking if ${email} is already registered...`);
    //let emailPattern = '^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$';

    const params = new Map();
    params.set('email', email);

    return this.http.get<boolean>(`${this.apiAuthUrl}/isEmailAlreadyTaken?email=`+ email)
      .pipe(
        map((isTaken) => {
          console.log(`Email:${email} is registered:`, isTaken);
          return isTaken;// Return the boolean value, specifically true if email is taken, false otherwise
        }),
        catchError(error => {
          console.error('Error checking email registration:', error);
          if(error.status === 404){
            // If the server returns 404, it means the email is not registered
            return of(false);// Emit 'false' if Email is not registered
          }
          return throwError(() => new Error(error));// Rethrow other status codes (errors) for further handling
        })
      );
  }

  // Get a user from his email
  /**
   * Fetches the user profile associated with the given email.
   *
   * @param {string} email - The email address of the user whose profile is to be retrieved.
   * @return {Observable<User>} An observable containing the user's profile data.
   */
  getUser(email: string): Observable<User>{
    return this.http.get<User>(`${this.apiAuthUrl}/profile?email=${email}`);
  }

  // Update the profile of a user
  /**
   * Updates the profile of an existing user.
   *
   * @param {string} id - The unique identifier of the user whose profile is being updated.
   * @param {User} user - An object containing the updated profile information of the user.
   * @return {Observable<User>} An observable that emits the updated user information upon success.
   */
  updateUserProfile(id: string, user: User): Observable<User>{
    return this.http.put<User>(`${this.apiAuthUrl}/updateProfile/${id}`, user);
  }

  // Authenticate the user and store the token
  /**
   * Authenticates a user by sending their credentials to the server.
   *
   * @param {LoginRequest} credentials - The login credentials containing the user's email and password.
   * @return {Observable<LoginResult>} An observable emitting the result of the login attempt, including success status
   * and token if successful.
   */
  login(credentials: LoginRequest): Observable<LoginResult> {
    console.log('Login in the system...');
    const url = this.apiAuthUrl + '/login';

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
  /**
   * Registers a new user using the provided credentials.
   *
   * @param {LoginRequest} credentials - The login credentials containing user information required for registration.
   * @return {Observable<any>} An observable that emits the server response upon successful registration.
   */
  register(credentials: LoginRequest): Observable<any> {
    console.log('Registering user with credentials: ', credentials);

    const url = this.apiAuthUrl + '/register';
    return this.http.post<any>(url, credentials).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        console.log('LoginRequest accepted with credentials: ', credentials);
      })
    );
  }

  // Logout by clearing the token and navigate to the login page
  /**
   * Logs out the currently authenticated user by clearing user data from local storage,
   * updating the authentication status, and navigating to the login page.
   *
   * @return {void} No return value.
   */
  logout(): void {
    // Implement logout logic if needed, e.g., clear tokens, notify server, etc.
    console.log('LoginRequest logged out...');
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userEmail);
    this.setAuthStatus(false);
    this.router.navigate(['/login']);
  }

  // Update the authentication status
  /**
   * Updates the authentication status of the user.
   *
   * @param {boolean} isAuthenticated - Indicates whether the user is authenticated.
   * @return {void} This method does not return a value.
   */
  private setAuthStatus(isAuthenticated: boolean): void {
    this._authStatus.next(isAuthenticated);
  }
}
