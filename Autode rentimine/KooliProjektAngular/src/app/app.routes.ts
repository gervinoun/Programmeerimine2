import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'cars',
    loadComponent: () => import('./cars/cars').then((module) => module.Cars)
  },
  {
    path: '',
    redirectTo: 'cars',
    pathMatch: 'full'
  }
];
