import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft } from '@ng-icons/lucide';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PageLayoutComponent } from '@components/page-layout/page-layout';
import { PageHeaderComponent } from '@components/page-header/page-header'; 

type ExerciseListItemDto = {
  id: string;
  name: string;
  muscleGroup: string;
  equipment: string | null;
  isDefault: boolean;
};

type ApiErrorResponse = {
  statusCode: number;
  errorCode: string;
  title: string;
  detail?: string;
};

@Component({
  selector: 'app-add-exercise',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    NgIcon,
    HlmButtonImports,
    HlmInputImports,
    HlmLabelImports,
    HlmCardImports,
    TranslatePipe,
    PageLayoutComponent,
    PageHeaderComponent
  ],
  providers: [
    provideIcons({
      lucideArrowLeft,
    }),
  ],
  templateUrl: './add-exercise.html',
})


export class AddExercisePage {
  private readonly http = inject(HttpClient);
  private readonly translate = inject(TranslateService);

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly workoutId = this.route.snapshot.queryParamMap.get('workoutId');
  readonly isSaving = signal(false);
  readonly saveError = signal<string | null>(null);

  addExercise(exerciseId: string): void {
    if (this.isSaving() || !this.workoutId) return;
    this.isSaving.set(true);
    this.saveError.set(null);

    this.http.post(
        '/api/workout/' + encodeURIComponent(this.workoutId) + '/exercises',
        { exerciseId },
    ).pipe(
      finalize(() => this.isSaving.set(false)),
    ).subscribe({
      next: () => {
        void this.router.navigate(['/record-workout'], { queryParams: { workoutId: this.workoutId } });
      },
      error: (error: HttpErrorResponse) => this.saveError.set(this.getErrorMessage(error, 'RECORD_WORKOUT.ADD_FAILED')),
    });
  }

  readonly exercises = signal<ExerciseListItemDto[]>([]);
  readonly loadError = signal<string | null>(null);
  readonly isLoading = signal(false);

  ngOnInit(): void {
    if (!this.workoutId) {
      this.loadError.set(this.translate.instant('RECORD_WORKOUT.MISSING_ID'));
      return;
    }
    this.loadExercises();
  }

  loadExercises(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.http
      .get<ExerciseListItemDto[]>('/api/exercises')
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.exercises.set(Array.isArray(data) ? data : []);
        },
        error: (error: HttpErrorResponse) => {
          const message = this.getErrorMessage(error, 'ERRORS.EXERCISES.LOAD_FAILED');
          this.loadError.set(message);
          console.error(error);
        },
      });
  }

  private getErrorMessage(error: HttpErrorResponse, fallbackKey: string): string {
    const apiError = error.error as ApiErrorResponse | undefined;

    if (apiError?.errorCode) {
      const translationKey = `ERRORS.${apiError.errorCode}`;
      const translated = this.translate.instant(translationKey);

      if (translated !== translationKey) {
        return translated;
      }
    }

    return this.translate.instant(fallbackKey);
  }
}
