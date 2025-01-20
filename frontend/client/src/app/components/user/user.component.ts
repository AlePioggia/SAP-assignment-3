import { Component } from '@angular/core';
import { UserRegistrationRequest, UserAuthenticationRequest } from '../../core/models/user.model';
import { UserService } from '../../core/services/user.service';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent {
  registrationRequest: UserRegistrationRequest = { username: '', password: '' };
  authenticationRequest: UserAuthenticationRequest = { username: '', password: '' };
  registrationResponse: string | null = null;
  authenticationResponse: string | null = null;

  constructor(private userService: UserService) {}

  registerUser(): void {
    this.userService.registerUser(this.registrationRequest).subscribe(
      response => {
        this.registrationResponse = `User registered successfully with ID: ${response}`;
      },
      error => {
        this.registrationResponse = 'Registration failed.';
      }
    );
  }

  authenticateUser(): void {
    this.userService.authenticateUser(this.authenticationRequest).subscribe(
      response => {
        this.authenticationResponse = `User authenticated with ID: ${response.userId}`;
      },
      error => {
        this.authenticationResponse = 'Authentication failed.';
      }
    );
  }
}
