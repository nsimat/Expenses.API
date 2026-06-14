import {Component, inject, ChangeDetectionStrategy} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';

import {AuthService} from '../../services/auth-service';
import {LoginRequest} from '../../models/login-request';
import {Router, RouterLink} from '@angular/router';
import {LoginResult} from '../../models/login-result';
import {map, Observable} from 'rxjs';

@Component({
  selector: 'app-signup',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './signup.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './signup.css'
})
export class Signup {
  // Optional title for the signup component
  title?: string;

  // Optional result of the signup operation, which can be used to display success or failure messages
  result?: LoginResult;

  // Error message to display if signup fails, initialized to null
  errorMessage: string | null = null;

  //
  private readonly formBuilder = inject(FormBuilder);
  private readonly apiService = inject(AuthService);
  private readonly router = inject(Router);

  // Reactive form for signup with validation rules
  /**
   * Reactive form group for user signup.
   * Contains form controls for email, password, and confirmPassword.
   * Each control is validated with specific rules:
   * - email: Required, must be a valid email format, and has a maximum length of 30 characters.
   * - password: Required, must be at least 6 characters long, and has a maximum length of 30 characters.
   * - confirmPassword: Required, must be at least 6 characters long, has a maximum length of 30 characters,
   *   and must match the password field.
   */
  protected readonly signupForm = this.formBuilder.group(
    {
      email: ['', [
        Validators.required,
        Validators.email,
        Validators.maxLength(30)
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(30)
      ]],
      confirmPassword: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(30),
        Signup.passwordMatch
      ]],
    }
  );

  // Custom validator to check if email is already taken
  emailAvailableValidator(control: AbstractControl): Observable<ValidationErrors | null> {
    const email = control.value;
    console.log("Validating email availability for:", email);
    return this.apiService
      .isEmailRegistered(email)
      .pipe(map(isEmailRegistered => {
        console.log('Email availability check result for ', email, ':', isEmailRegistered);
        return isEmailRegistered ? { emailTaken: true } : null;
      }));
  }

  // Method to check if a form control has a specific error and has been touched or dirty
  /**
   * Checks whether a specific control in the form has a specific validation error.
   *
   * @param controlName The name of the form control to check.
   * @param errorName The name of the validation error to verify.
   * @return A boolean indicating whether the specified control has the specified error and has been touched or is dirty.
   */
  hasError(controlName: string, errorName: string): boolean {
    const control = this.signupForm.get(controlName);
    return (control?.touched || control?.dirty) && control?.hasError(errorName) || false;
  }

  // Custom validator to check if password and confirmPassword match
  /**
   * Validates if the provided control's value matches the value of the 'password' control in the same form group.
   *
   * @param control The form control to validate.
   * @return A validation error object with a `matchingError` property if the values do not match, or `null` if they do match.
   */
  private static passwordMatch(control: AbstractControl<string>): ValidationErrors | null {
    const formGroup = control.parent as FormGroup;
    return formGroup?.get('password')?.value === control?.value ? null : {matchingError: true};
  }

  // Method for handling form submission when the user clicks the "Sign Up" button
  /**
   * Handles the user registration process by validating the signup form,
   * submitting the form data to the server, and managing the response.
   *
   * The method checks if the form is valid:
   * - If valid, it sends the form data to the server via the `apiService` for registration.
   * - If invalid, it logs validation errors and provides feedback to the user.
   *
   * The registration process includes:
   * - Logging the server response and updating the UI based on the outcome.
   * - Navigating to appropriate routes based on success or error conditions.
   *
   * @return {void} This method does not return a value.
   */
  register(): void {
    console.log('Submitting form with values:', this.signupForm.value);
    this.errorMessage = null; // Clear previous error messages
    if (this.signupForm.valid) {
      const signUpData = this.signupForm.value;
      console.log('Form submitted is valid:', signUpData);
      this.apiService.register(<LoginRequest>signUpData).subscribe(
        {
          next: (loginResult) => {
            this.result = loginResult;// Store the result for further processing???
            console.log('Server response: ', loginResult.message);
            if (loginResult.success) {
              console.log('Signup successful:', loginResult);
              this.router.navigate(['/login']);
            }
          },
          error: (error) => {
            console.log('Signup failed. Check the server error:', error.error?.message[0]);
            this.errorMessage = error.error?.message || 'An unknown error occurred during signup. Please try again.';
            if (error.status == 400) {
              this.router.navigate(['/signup']);
            }
          }
        }
      )
    } else {
      // Form is invalid, display form validation errors to the user
      console.log('Form submitted is invalid:', this.signupForm.errors);
      this.errorMessage = "Please, correct the errors in the form before submitting.";
      this.router.navigate(['/signup']);
    }
  }
}
