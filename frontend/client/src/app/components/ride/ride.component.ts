import { Component, OnInit, Signal } from '@angular/core';
import { RideStartRequest, RideResponse } from '../../core/models/ride.model';
import { RideService } from '../../core/services/ride.service';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { SignalRService } from '../../core/services/signalr.service';

@Component({
  selector: 'app-ride',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './ride.component.html',
  styleUrl: './ride.component.css'
})
export class RideComponent implements OnInit{
  startRideRequest: RideStartRequest = { userId: 'b8cc6fda-8f78-414d-b66a-f689861e81d2', eBikeId: '2c03baee-068d-4968-b779-e8f0114d8378' };
  rideResponse: RideResponse | null = null;

  constructor(private rideService: RideService, private signalRService: SignalRService) {}

  ngOnInit(): void {
    this.signalRService.startConnection();
    this.signalRService.registerListeners();
  }

  startRide(): void {
    this.rideService.startRide(this.startRideRequest).subscribe(
      response => {
        alert(`Ride started with ID: ${response}`);
      },
      error => {
        alert('Failed to start ride.');
      }
    );
  }

  endRide(rideId: string): void {
    this.rideService.endRide(rideId).subscribe(
      () => {
        alert('Ride ended successfully.');
      },
      error => {
        alert('Failed to end ride.');
      }
    );
  }
}
