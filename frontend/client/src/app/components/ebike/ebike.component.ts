import { Component, OnInit } from '@angular/core';
import { CreateEBikeRequest, EBikeResponse } from '../../core/models/e-bike.model';
import { EBikeService } from '../../core/services/ebike.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgFor } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-ebike',

  standalone: true,
  imports: [FormsModule, NgFor, CommonModule, HttpClientModule],
  templateUrl: './ebike.component.html',
  styleUrl: './ebike.component.css'
})
export class EbikeComponent implements OnInit {
  createEBikeRequest: CreateEBikeRequest = { name: '', x: 0, y: 0, isAvailable: true };
  ebikes: EBikeResponse[] = [];

  constructor(private eBikeService: EBikeService) {}

  ngOnInit(): void {
    this.getAllEBikes();
  }

  createEBike(): void {
    this.eBikeService.createEBike(this.createEBikeRequest).subscribe(
      response => {
        alert(`eBike created with ID: ${response}`);
        this.getAllEBikes();
      }
    );
  }

  getAllEBikes(): void {
    this.eBikeService.getAllEBikes().subscribe(
      response => {
        this.ebikes = response;
      }
    );
  }
}
