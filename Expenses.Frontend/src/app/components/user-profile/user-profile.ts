import {Component, OnInit} from '@angular/core';
import {AuthService} from '../../services/auth-service';
import {Router} from '@angular/router';

import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {User} from '../../models/user';

@Component({
  selector: 'app-user-profile',
  imports: [ReactiveFormsModule],
  templateUrl: './user-profile.html',
  styleUrl: './user-profile.css'
})
export class UserProfile implements OnInit {

  userProfileForm: FormGroup;
  user: User = {
    id: 0,
    email: '',
    firstName: '',
    lastName: '',
  };
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.userProfileForm = this.fb.group({
      email:['', [Validators.required, Validators.email]],
      firstName:['', [Validators.required, Validators.minLength(3)]],
      lastName:['', [Validators.required, Validators.minLength(3)]]
    });
  }

  // On init, display user data from database
  ngOnInit(): void {
    // Initialize the form with the data from the user
    const email = localStorage.getItem('userEmail');
    console.log('User profile email is:', email);
    if(email != null){
      console.log("Loading user profile for email:", email);
      this.authService.getUser(email).subscribe({
        next: (userdata) => {
          console.log(userdata);
          this.user.id = userdata.id;
          console.log('User ID is:', this.user.id);
          this.user.email = userdata.email;
          this.user.firstName = userdata.firstName;
          this.user.lastName = userdata.lastName;

          // Patch form values
          this.userProfileForm.patchValue({
            email: this.user.email,
            firstName: this.user.firstName,
            lastName: this.user.lastName,
          });
          console.log('Patching form with:', this.userProfileForm.value)
          // Disable the email field to prevent modification
          // this.userProfileForm.get('email')?.disable();
          console.log('Loaded user profile for editing:', userdata);
          // Disable the password field
          //this.userProfileForm.get('password')?.disable();
        },
        error: (error) => {
          console.log(error);
        }
      })
    }
  }

  // Method to check if a form control has a specific error and has been touched or dirty
  hasError(controlName: string, errorName: string): boolean {
    const control = this.userProfileForm.get(controlName);
    return (control?.touched || control?.dirty) && control?.hasError(errorName) || false;
  }

  onSubmit() {
    console.log("Submitting the user profile form...");
    // Clear previous error messages
    this.errorMessage = null;
    // Checks if the form is valid
    if (this.userProfileForm.valid){
      const modifiedUser = this.userProfileForm.value;
      console.log('Submitted User profile form is valid:', modifiedUser);
      // if the form is valid, update user data
      this.authService.updateUserProfile(this.user.id, modifiedUser).subscribe({
        next: (userdata) => {
          console.log('And this is the update result:', userdata);
          alert("Your user profile has been modified!")
          this.router.navigate(['/transactions']);
        },
        error: (error) => {
          console.error('Error updating user profile:', error);
          this.errorMessage = error.message;
        }
      })
    }
    // if error, redisplay the user profile page
  }

  protected cancel() {
    // Redirects to the main page, where transactions are displayed.
    console.log('Redirection to the transactions page...');
    this.router.navigate(['/transactions']);
  }
}
