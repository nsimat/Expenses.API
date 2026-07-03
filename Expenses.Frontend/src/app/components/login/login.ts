import {Component, inject, ChangeDetectionStrategy} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {AuthService} from '../../services/auth-service';
import {Router, RouterLink} from '@angular/router';

import {LoginRequest} from '../../models/login-request';


@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './login.css'
})
export class Login {

  // Message to display if login fails, initialized to null
  errorMessage: string | null = null;

  // Injecting FormBuilder, AuthService, and Router into the component
  private readonly formBuilder = inject(FormBuilder);
  private readonly apiService = inject(AuthService);
  private readonly router = inject(Router);

  // Reactive form for login with validation rules
  protected readonly loginForm = this.formBuilder.group({
    email: ['', [
      Validators.required,
      Validators.email,
      Validators.pattern(String.raw`^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$`)
    ]],
    password: ['', [
      Validators.required,
      Validators.minLength(6)
    ]]
  });

  // Method to check if a form control has a specific error and has been touched or dirty
  /**
   * Checks if a specific control within the form has a specified validation error.
   *
   * @param controlName The name of the control to check.
   * @param errorName The name of the error to verify.
   * @return A boolean indicating whether the specified error exists on the control.
   */
  hasError(controlName: string, errorName: string): boolean {
    const control = this.loginForm.get(controlName);
    return (control?.touched || control?.dirty) && control?.hasError(errorName) || false;
  }

  // Method for handling form submission when the user clicks the "Login" button
  /**
   * Handles the login process by validating the form, submitting the login request
   * to the server, and navigating to the appropriate route based on the response.
   * If the login form is invalid, it logs the errors without submitting.
   *
   * @return {void} No return value. Executes navigation on successful login.
   */
  login(): void {
    console.log('Submitting form with values:', this.loginForm.value);
    this.errorMessage = null;// Clear previous error messages

    if (this.loginForm.valid) {
      const loginData = this.loginForm.getRawValue();
      console.log('Form submitted is valid:', loginData);
      this.apiService
        .login(<LoginRequest>loginData)
        .subscribe(
        {
          next: (loginResult) => {
            console.log('Login successful, navigating to dashboard...');
            if (loginResult.success) {
              console.log('Login accepted for:', loginData.email);

              // Store the user's full name in local storage for later use
              this.setFullName(<string>loginData.email);
              const fullName = this.apiService.getUserFullName();
              console.log('User Full Name in Login:', fullName);

              // Navigate to the transactions page after successful login
              this.router.navigate(['/transactions']);
            }
          },
          error: (error) => {
            console.log('Login failed with error:', error);
            this.errorMessage = error.error?.message || 'An error occurred during login. Please try again.';
            if (error.status == 401) {
              this.errorMessage = 'Invalid email or password. Please try again.';
              this.router.navigate(['/login']);
            }
          }
        });
    } else {
      console.log('Form submitted is invalid:', this.loginForm.errors);
    }
  }

  /**
   * Sets the full name of a user in local storage by fetching their details using the provided email.
   *
   * @param {string} email - The email address of the user whose full name is to be retrieved and stored.
   * @return {void} This method does not return a value.
   */
  private setFullName(email: string): void {
    this.apiService
      .getUser(email)
      .subscribe({
      next: (user) => {
        if(user.firstName && user.lastName){
          // Store the full name in local storage
          const fullName = `${user.firstName} ${user.lastName}`;
          localStorage.setItem('userFullName', fullName);
          console.log('User full name set in local storage:', fullName);
        }
      },
      error: (error) => {
        console.error('Error fetching user profile: ', error);
      }
    });
  }

}
