import {Component, inject} from '@angular/core';
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
  styleUrl: './login.css'
})
export class Login {

  errorMessage: string | null = null;

  private readonly formBuilder = inject(FormBuilder);
  private readonly apiService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly loginForm = this.formBuilder.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  // Method to check if a form control has a specific error and has been touched or dirty
  hasError(controlName: string, errorName: string): boolean {
    const control = this.loginForm.get(controlName);
    return (control?.touched || control?.dirty) && control?.hasError(errorName) || false;
  }

  // Method for handling form submission when the user clicks the "Login" button
  login(): void {
    console.log('Submitting form with values:', this.loginForm.value);
    this.errorMessage = null;// Clear previous error messages

    if (this.loginForm.valid) {
      const loginData = this.loginForm.value;
      console.log('Form submitted is valid:', loginData);
      this.apiService.login(<LoginRequest>loginData).subscribe(
        {
          next: (loginResult) => {
            console.log('Login successful, navigating to dashboard...');
            if (loginResult.success) {
              console.log('Login accepted for:', loginData.email);
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
}
