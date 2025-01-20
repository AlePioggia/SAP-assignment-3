// src/app/core/models/ride.model.ts
export interface Ride {
    id: string;
    userId: string;
    eBikeId: string;
    startTime: Date;
    endTime?: Date;
    creditUsed: number;
  }
  
  // Richiesta per avviare una corsa
  export interface RideStartRequest {
    userId: string;
    eBikeId: string;
  }
  
  // Risposta generica per la corsa
  export interface RideResponse {
    id: string;
    startTime: Date;
    endTime?: Date;
    creditUsed: number;
    userId: string;
    eBikeId: string;
  }
  