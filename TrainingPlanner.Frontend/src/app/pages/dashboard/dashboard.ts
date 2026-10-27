import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';

type WorkoutListItem = {
  id: string;
  name: string;
  date: string;
};

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink, HlmButtonImports, HlmCardImports],
  templateUrl: './dashboard.html',
})
export class DashboardPage implements OnInit {
  private readonly http = inject(HttpClient);
  readonly workouts = signal<WorkoutListItem[]>([]);
  readonly loadError = signal<string | null>(null);
  readonly isLoading = signal(false);

  ngOnInit(): void {
    this.loadWorkouts();
  }

  loadWorkouts(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.http
      .get<WorkoutListItem[]>('https://localhost:7001/api/workouts', {
        withCredentials: true,
      })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.workouts.set(Array.isArray(data) ? data : []);
        },
        error: (error) => {
          this.loadError.set('Tréninky se nepodařilo načíst. Zkuste to prosím znovu.');
          console.error('Nepodařilo se načíst tréninky:', error);
        },
      });
  }
}
