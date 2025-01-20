import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, UserRegistrationRequest, UserAuthenticationRequest, UserResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'http://localhost:9000/api/user';

  constructor(private http: HttpClient) {}

  registerUser(request: UserRegistrationRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/register`, request);
  }

  authenticateUser(request: UserAuthenticationRequest): Observable<{ userId: string }> {
    return this.http.post<{ userId: string }>(`${this.apiUrl}/authenticate`, request);
  }

  getUserById(id: string): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/${id}`);
  }
}
