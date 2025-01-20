// src/app/core/services/ebike.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateEBikeRequest, EBikeResponse } from '../models/e-bike.model';

@Injectable({
  providedIn: 'root'
})
export class EBikeService {
  private apiUrl = 'http://localhost:9000/api/eBike';

  constructor(private http: HttpClient) {}

  createEBike(request: CreateEBikeRequest): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/create`, request);
  }

  getEBike(eBikeId: string): Observable<EBikeResponse> {
    return this.http.get<EBikeResponse>(`${this.apiUrl}/${eBikeId}`);
  }

  getAllEBikes(): Observable<EBikeResponse[]> {
    return this.http.get<EBikeResponse[]>(`${this.apiUrl}/all`);
  }

  deleteEBike(eBikeId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${eBikeId}`);
  }
}
