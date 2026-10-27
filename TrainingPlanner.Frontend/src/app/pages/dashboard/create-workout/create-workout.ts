import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HlmButtonImports } from '@spartan-ng/helm/button';

@Component({
  selector: 'app-create-workout',
  imports: [CommonModule, ReactiveFormsModule, HlmButtonImports],
  templateUrl: './create-workout.html',
})
export class CreateWorkoutPage {
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    date: ['', Validators.required], // datetime-local
  });

  submit(): void {
    if (this.form.invalid) return;

    const body = {
      name: this.form.value.name!,
      date: new Date(this.form.value.date!).toISOString(),
    };

    this.http
      .get<{ requestToken: string }>('https://localhost:7001/api/auth/csrf', {
        withCredentials: true,
      })
      .subscribe((csrf) => {
        this.http
          .post('https://localhost:7001/api/workouts', body, {
            withCredentials: true,
            headers: new HttpHeaders({ 'X-CSRF-TOKEN': csrf.requestToken }),
          })
          .subscribe(() => this.router.navigateByUrl('/dashboard'));
      });
  }
}
