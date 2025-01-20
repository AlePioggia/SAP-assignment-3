// app-routing.module.ts

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RideComponent } from './components/ride/ride.component';
import { EbikeComponent } from './components/ebike/ebike.component';
import { UserComponent } from './components/user/user.component';

const routes: Routes = [
  { path: 'ride', component: RideComponent },
  { path: 'ebike', component: EbikeComponent },
  { path: 'user', component: UserComponent },
  { path: '', redirectTo: '/ride', pathMatch: 'full' }, // Redirect alla pagina principale (opzionale)
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
