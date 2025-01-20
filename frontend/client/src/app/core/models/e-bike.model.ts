export interface EBike {
    id: string;
    name: string;
    x: number;
    y: number;
    isAvailable: boolean;
    createdAt: Date;
  }
  
  export interface CreateEBikeRequest {
    name: string;
    x: number;
    y: number;
    isAvailable: boolean;
  }
  
  export interface EBikeResponse {
    id: string;
    name: string;
    x: number;
    y: number;
    isAvailable: boolean;
    createdAt: Date;
  }
  