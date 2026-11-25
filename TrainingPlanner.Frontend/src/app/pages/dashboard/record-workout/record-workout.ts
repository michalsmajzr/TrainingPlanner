import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { formatDate } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft, lucideX } from '@ng-icons/lucide';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PageLayoutComponent } from '@components/page-layout/page-layout';
import { PageHeaderComponent } from '@components/page-header/page-header';
import { HlmAlertDialogImports } from '@spartan-ng/helm/alert-dialog';

@Component({
  selector: 'app-record-workout',
  imports: [
    RouterLink,
    NgIcon,
    HlmButtonImports,
    HlmCardImports,
    TranslatePipe,
    PageLayoutComponent,
    PageHeaderComponent,
    HlmAlertDialogImports
  ],
  providers: [
    provideIcons({
      lucideArrowLeft,
      lucideX
    })],
  templateUrl: './record-workout.html',
})
export class RecordWorkoutPage implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly translate = inject(TranslateService);

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly exercises = signal<{
    id: string;
    name: string;
    isDefault: boolean;
    position: number;
  }[]>([]);
  readonly isCreating = signal(false);
  readonly workoutId = signal<string | null>(null);
  readonly startedAt = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  pageTitle(): string {
    const title = this.translate.instant('RECORD_WORKOUT.TITLE');

    const date = this.startedAt();
    if (!date) {
      return title;
    }

    const format = this.translate.instant('RECORD_WORKOUT.DATE_FORMAT');
    const formattedDate = formatDate(date, format, this.translate.currentLang() || 'cs');

    return `${title} ${formattedDate}`;
  }

  ngOnInit(): void {
    const id = this.route.snapshot.queryParamMap.get('workoutId');
    if (id) {
      this.workoutId.set(id);
      this.loadWorkout(id);
    } else {
      this.createWorkout();
    }
  }

  private loadWorkout(id: string): void {
    this.http.get<{ startedAt: string; exercises: { id: string; name: string; position: number, isDefault: boolean }[] }>(
      '/api/workout/' + encodeURIComponent(id),
    ).subscribe({
      next: workout => {
        this.startedAt.set(workout.startedAt);
        this.exercises.set([...workout.exercises].sort((a, b) => a.position - b.position));
      },
      error: () => this.errorMessage.set(this.translate.instant('RECORD_WORKOUT.LOAD_FAILED')),
    });
  }

  private createWorkout(): void {
    if (this.isCreating() || this.workoutId()) return;
    this.isCreating.set(true);
    this.errorMessage.set(null);

    this.http.post<{ id: string }>('/api/workout', {}).pipe(
      finalize(() => this.isCreating.set(false)),
    ).subscribe({
      next: result => {
        this.workoutId.set(result.id);
        this.loadWorkout(result.id);
        void this.router.navigate([], {
          relativeTo: this.route,
          queryParams: { workoutId: result.id },
          queryParamsHandling: 'merge',
          replaceUrl: true,
        });
      },
      error: (_error: HttpErrorResponse) => {
        this.errorMessage.set(this.translate.instant('RECORD_WORKOUT.FAILED'));
      },
    });
  }

  deleteExercise(workoutExerciseId: string): void {
    const workoutId = this.workoutId();
    if (!workoutId) {
      return;
    }

    this.errorMessage.set(null);

    this.http.delete<void>('/api/workout/' + encodeURIComponent(workoutId) + '/exercises/' + encodeURIComponent(workoutExerciseId))
      .subscribe({
        next: () => {
          this.exercises.update(exercises => exercises.filter(exercise => exercise.id !== workoutExerciseId),
          );
        },
        error: (_error: HttpErrorResponse) => {
          this.errorMessage.set(
            this.translate.instant('RECORD_WORKOUT.DELETE_FAILED'),
          );
        },
      });
  }
}
