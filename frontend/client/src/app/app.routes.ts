import { Routes } from '@angular/router';
import { RideComponent } from './components/ride/ride.component';
import { EbikeComponent } from './components/ebike/ebike.component';
import { UserComponent } from './components/user/user.component';
import { BikeGridComponent } from './components/bike-grid/bike-grid.component';

export const routes: Routes = [
    { path: 'ride', component: RideComponent },
    { path: 'ebike', component: EbikeComponent },
    { path: 'user', component: UserComponent },
    { path: 'grid', component: BikeGridComponent},
    { path: '', redirectTo: '/ride', pathMatch: 'full' } 
  ];