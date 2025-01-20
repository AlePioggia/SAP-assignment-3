import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Ride, RideStartRequest, RideResponse } from '../models/ride.model';

@Injectable({
  providedIn: 'root'
})
export class RideService {
  private apiUrl = 'http://localhost:9000/api/ride';

  constructor(private http: HttpClient) {}

  startRide(request: RideStartRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/start`, request);
  }

  endRide(rideId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/end/${rideId}`, {});
  }

  getRideById(rideId: string): Observable<RideResponse> {
    return this.http.get<RideResponse>(`${this.apiUrl}/${rideId}`);
  }
}
