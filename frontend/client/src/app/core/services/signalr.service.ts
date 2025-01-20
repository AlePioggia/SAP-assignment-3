import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  public bikePositionUpdates: { bikeId: number; x: number; y: number }[] = [];

  constructor() {}

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5001/bikeHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
        this.registerListeners();
      })
      .catch((err) => console.error('Error starting SignalR connection:', err));
  }

  registerListeners() {
    this.hubConnection.on('ReceiveBikePosition', (bikeId: number, x: number, y: number) => {
      console.log(`Bike ${bikeId} moved to (${x}, ${y})`);
      this.bikePositionUpdates.push({ bikeId, x, y });
    });
  }

  sendPositionUpdate(bikeId: number, x: number, y: number) {
    this.hubConnection
      .invoke('BroadcastBikePosition', bikeId, x, y)
      .catch((err) => console.error('Error sending position update:', err));
  }

  stopConnection() {
    this.hubConnection
      .stop()
      .then(() => console.log('SignalR connection stopped'))
      .catch((err) => console.error('Error stopping SignalR connection:', err));
  }
}
