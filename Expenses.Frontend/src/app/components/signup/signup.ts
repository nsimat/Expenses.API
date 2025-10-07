import {Component, inject} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {AuthService} from '../../services/auth-service';
import {User} from '../../models/user';
import {Router, RouterLink} from '@angular/router';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './signup.html',
  styleUrl: './signup.css'
})
export class Signup {

  private readonly formBuilder = inject(FormBuilder);
  private readonly apiService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly signupForm = this.formBuilder.group(
    {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
    },
    {
      validator: this.passwordMatchValidator
    }
  );

  title?: string;
  loginResult?: string;

  private passwordMatchValidator(form: FormGroup) {
    return form.get('password')?.value === form.get('confirmPassword')?.value
      ? null
      : {'passwordMissMatch': true};
  }

  protected register(): void {
  }

  onSubmit(loginRequest: User): void {
    if(this.signupForm.valid){
      const newUser = this.signupForm.value;
      console.log('Form submitted is valid:', loginRequest);
      this.apiService.register(newUser).subscribe(
        {
          next: (response) => {
          this.loginResult = 'Registration successful!';
          console.log('Registration successful!-', response);
          this.router.navigate(['/login']);
          },
          error: (error) => {
            console.log('Error during registration:', error);
            this.loginResult = 'Registration has failed! Please try again!';
            // Optionally, you can display error details from the server
            if (error.error && error.error.message) {
              this.loginResult += ` Details: ${error.error.message}`;
            }
          }
        }
      )
    }
    console.log('Form submitted is invalid:', this.signupForm.errors);
    // Optionally, you can display form validation errors to the user
  }
}
