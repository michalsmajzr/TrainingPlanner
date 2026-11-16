import { CommonModule } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft } from '@ng-icons/lucide';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

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
  selector: 'app-record-workout',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgIcon, HlmButtonImports, HlmInputImports, HlmLabelImports, HlmCardImports, TranslatePipe],
  providers: [
    provideIcons({
      lucideArrowLeft,
    }),
  ],
  templateUrl: './record-workout.html',
})


export class RecordWorkoutPage {
  private readonly http = inject(HttpClient);
  private readonly translate = inject(TranslateService);

  readonly exercises = signal<ExerciseListItemDto[]>([]);
  readonly loadError = signal<string | null>(null);
  readonly isLoading = signal(false);

  ngOnInit(): void {
    this.loadExercises();
  }

  loadExercises(): void {
    this.isLoading.set(true);
    this.loadError.set(null);

    this.http
      .get<ExerciseListItemDto[]>('https://localhost:7001/api/exercises', {
        withCredentials: true,
      })
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
