import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { finalize, switchMap } from 'rxjs';
import { toast } from '@spartan-ng/brain/sonner';
import { HlmAlertDialogImports } from '@spartan-ng/helm/alert-dialog';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';

type WorkoutListItem = {
  id: string;
  name: string;
  date: string;
};

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, RouterLink, HlmButtonImports, HlmCardImports, HlmAlertDialogImports],
  templateUrl: './dashboard.html',
})
export class DashboardPage implements OnInit {
  private readonly http = inject(HttpClient);
  readonly workouts = signal<WorkoutListItem[]>([]);
  readonly loadError = signal<string | null>(null);
  readonly isLoading = signal(false);
  readonly deletingWorkoutId = signal<string | null>(null);

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

  deleteWorkout(id: string): void {
    this.deletingWorkoutId.set(id);

    this.http
      .get<{ requestToken: string }>('https://localhost:7001/api/auth/csrf', {
        withCredentials: true,
      })
      .pipe(
        switchMap((csrf) =>
          this.http.delete(`https://localhost:7001/api/workouts/${encodeURIComponent(id)}`, {
            withCredentials: true,
            headers: new HttpHeaders({ 'X-CSRF-TOKEN': csrf.requestToken }),
          })
        ),
        finalize(() => this.deletingWorkoutId.set(null))
      )
      .subscribe({
        next: () => {
          this.workouts.update((workouts) => workouts.filter((w) => w.id !== id));
          toast.success('Trénink byl úspěšně smazán.');
        },
        error: (error) => {
          toast.error('Trénink se nepodařilo smazat. Zkuste to prosím znovu.');
          console.error('Nepodařilo se smazat trénink:', error);
        },
      });
  }
}
