import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { CarsService } from '../services/cars.service';
import { Car } from '../models/car';

@Component({
  selector: 'app-cars',
  templateUrl: './cars.html',
  styleUrl: './cars.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Cars implements OnInit {
  private readonly carsService = inject(CarsService);

  readonly cars = signal<Car[]>([]);
  readonly isLoading = signal(true);
  readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.carsService.getCars().subscribe({
      next: (cars) => {
        this.cars.set(cars);
        this.isLoading.set(false);
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.getErrorMessage(error));
        this.isLoading.set(false);
      }
    });
  }

  private getErrorMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse && error.status === 0) {
      return 'Unable to reach the Cars API. Check that it is running and that CORS allows this Angular app origin.';
    }

    if (error instanceof HttpErrorResponse) {
      return `The Cars API request failed (HTTP ${error.status}).`;
    }

    return 'An unexpected error occurred while loading cars.';
  }
}
