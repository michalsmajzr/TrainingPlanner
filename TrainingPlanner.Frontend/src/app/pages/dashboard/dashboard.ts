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
import { PageLayoutComponent } from '@components/page-layout/page-layout';
import { PageHeaderComponent } from '@components/page-header/page-header'; 

type WorkoutListItem = {
  id: string;
  startedAt: string;
  finishedAt: string | null;
  exerciseCount: number;
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
    LanguageSwitcherComponent,
    PageLayoutComponent,
    PageHeaderComponent
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
      .get<WorkoutListItem[]>('https://localhost:7001/api/workout', {
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
