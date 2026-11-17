import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft } from '@ng-icons/lucide';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

type WorkoutDetail = {
  id: string;
  name: string;
};

@Component({
  selector: 'app-workout-detail',
  imports: [RouterLink, HlmButtonImports, HlmCardImports, NgIcon, TranslatePipe],
  providers: [
    provideIcons({
      lucideArrowLeft,
    }),
  ],
  templateUrl: './workout-detail.html',
})

export class WorkoutDetailPage implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  private readonly translate = inject(TranslateService);

  readonly workout = signal<WorkoutDetail | null>(null);
  readonly loadError = signal<string | null>(null);
  readonly isLoading = signal(false);

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (id) {
        this.loadWorkout(id);
      }
    });
  }

  loadWorkout(id: string): void {
    this.isLoading.set(true);
    this.loadError.set(null);
    this.workout.set(null);

    this.http
      .get<WorkoutDetail>(`https://localhost:7001/api/training-programs/${encodeURIComponent(id)}`, {
        withCredentials: true,
      })
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => this.workout.set(data),
        error: (error) => {
          this.loadError.set(
            this.translate.instant(
              error?.status === 404
                ? 'ERRORS.training_program.not_found'
                : 'ERRORS.WORKOUTS.DETAIL_LOAD_FAILED',
            ),
          );
          console.error('Failed to load workout:', error);
        },
      });
  }
}
