import {Component, inject} from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {AuthService} from '../../services/auth-service';
import {User} from '../../models/user';
import {Router, RouterLink} from '@angular/router';
import {LoginResult} from '../../models/login-result';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './signup.html',
  styleUrl: './signup.css'
})
export class Signup {

  title?: string;
  result?: LoginResult;
  errorMessage: string | null = null;

  private readonly formBuilder = inject(FormBuilder);
  private readonly apiService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly signupForm = this.formBuilder.group(
    {
      email: [''!, [Validators.required, Validators.email]],
      password: [''!, [Validators.required, Validators.minLength(6)]],
      confirmPassword: [''!, [Validators.required, Validators.minLength(6), Signup.passwordMatch]],
    }
  );

  // Method to check if a form control has a specific error and has been touched or dirty
  hasError(controlName: string, errorName: string): boolean {
    const control = this.signupForm.get(controlName);
    return (control?.touched || control?.dirty) && control?.hasError(errorName) || false;
  }

  // Custom validator to check if password and confirmPassword match
  private  static passwordMatch(control: AbstractControl<string>): ValidationErrors | null {
    const formGroup = control.parent as FormGroup;
    return formGroup?.get('password')?.value === control?.value ? null : {matchingError: true};
  }

  // Method for handling form submission when the user clicks the "Sign Up" button
  register(): void {
    console.log('Submitting form with values:', this.signupForm.value);
    this.errorMessage = null; // Clear previous error messages
    if (this.signupForm.valid) {
      const signUpData = this.signupForm.value;
      console.log('Form submitted is valid:', signUpData);
      this.apiService.register(<User>signUpData).subscribe(
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
            if(error.status == 400){
              this.router.navigate(['/signup']);
            }
          }
        }
      )
    } else {
      // Form is invalid, display form validation errors to the user
      console.log('Form submitted is invalid:', this.signupForm.errors);
      this.router.navigate(['/signup']);
    }
  }
}
