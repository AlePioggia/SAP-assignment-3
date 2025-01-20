import { Component, OnDestroy, OnInit } from '@angular/core';
import { SignalRService } from '../../core/services/signalr.service';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-bike-grid',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './bike-grid.component.html',
  styleUrl: './bike-grid.component.css'
})
export class BikeGridComponent implements OnInit, OnDestroy {

  constructor(public signalRService: SignalRService) {}

  ngOnInit(): void {
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    this.signalRService.stopConnection();
  }

}
