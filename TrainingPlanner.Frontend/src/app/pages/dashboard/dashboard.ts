import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { finalize, switchMap } from 'rxjs';
import { TranslateService, TranslatePipe } from '@ngx-translate/core';
import { toast } from '@spartan-ng/brain/sonner';
import { HlmAlertDialogImports } from '@spartan-ng/helm/alert-dialog';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { LanguageSwitcherComponent } from '@components/language-switcher';

type WorkoutListItem = {
  id: string;
  name: string;
  date: string;
};

type ApiErrorResponse = {
  statusCode: number;
  errorCode: string;
  title: string;
  detail?: string;
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TranslatePipe,
    HlmButtonImports,
    HlmCardImports,
    HlmAlertDialogImports,
    LanguageSwitcherComponent
  ],
  templateUrl: './dashboard.html',
})
export class DashboardPage implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly translate = inject(TranslateService);

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
        error: (error: HttpErrorResponse) => {
          const message = this.getErrorMessage(error, 'ERRORS.WORKOUTS.LOAD_FAILED');
          this.loadError.set(message);
          console.error(error);
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
          toast.success(this.translate.instant('WORKOUTS.DELETE_SUCCESS'));
        },
        error: (error: HttpErrorResponse) => {
          const errorMessage = this.getErrorMessage(error, 'ERRORS.WORKOUTS.DELETE_FAILED');
          toast.error(errorMessage);
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
