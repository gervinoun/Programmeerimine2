import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Car } from '../models/car';

@Injectable({
  providedIn: 'root'
})
export class CarsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5086/api/Cars';

  getCars(): Observable<Car[]> {
    return this.http.get<Car[]>(this.apiUrl);
  }
}
