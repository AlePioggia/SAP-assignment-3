import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, UserRegistrationRequest, UserAuthenticationRequest, UserResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:9000/api/user';
  cookieName: string = 'jwt_token';

  constructor(private http: HttpClient) {}

  setToken(token: string): void {
    localStorage.setItem(this.cookieName, token);
  }

  registerUser(request: UserRegistrationRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/register`, request);
  }

  authenticateUser(request: UserAuthenticationRequest) {
    const response = this.http.post<{ userId: string }>(`${this.apiUrl}/authenticate`, request);
    response.subscribe(data => {
      this.setToken(data.userId);
    });
  }

  getUserById(id: string): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/${id}`);
  }
}
