import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, switchMap } from 'rxjs';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft } from '@ng-icons/lucide';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PageLayoutComponent } from '@components/page-layout/page-layout';
import { PageHeaderComponent } from '@components/page-header/page-header'; 

@Component({
  selector: 'app-record-workout',
  imports: [
    RouterLink,
    NgIcon,
    HlmButtonImports,
    HlmCardImports,
    TranslatePipe,
    PageLayoutComponent,
    PageHeaderComponent
  ],
  providers: [provideIcons({ lucideArrowLeft })],
  templateUrl: './record-workout.html',
})
export class RecordWorkoutPage implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly translate = inject(TranslateService);

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly exercises = signal<{ id: string; name: string; position: number }[]>([]);

  readonly isCreating = signal(false);
  readonly workoutId = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

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
    this.http.get<{ exercises: { id: string; name: string; position: number }[] }>(
      'https://localhost:7001/api/workout/' + encodeURIComponent(id),
      { withCredentials: true },
    ).subscribe({
      next: workout => this.exercises.set([...workout.exercises].sort((a, b) => a.position - b.position)),
      error: () => this.errorMessage.set(this.translate.instant('RECORD_WORKOUT.LOAD_FAILED')),
    });
  }

  private createWorkout(): void {
    if (this.isCreating() || this.workoutId()) return;
    this.isCreating.set(true);
    this.errorMessage.set(null);

    this.http.get<{ requestToken: string }>('https://localhost:7001/api/auth/csrf', {
      withCredentials: true,
    }).pipe(
      switchMap(csrf => this.http.post<{ id: string }>('https://localhost:7001/api/workout', {}, {
        withCredentials: true,
        headers: new HttpHeaders({ 'X-CSRF-TOKEN': csrf.requestToken }),
      })),
      finalize(() => this.isCreating.set(false)),
    ).subscribe({
      next: result => {
        this.workoutId.set(result.id);
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
}
