import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideArrowLeft } from '@ng-icons/lucide';

@Component({
  selector: 'app-create-workout',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgIcon, HlmButtonImports, HlmInputImports, HlmLabelImports],
  providers: [
    provideIcons({
      lucideArrowLeft,
    }),
  ],
  templateUrl: './create-workout.html',
})
export class CreateWorkoutPage {
  private readonly http = inject(HttpClient);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);

  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const body = {
      name: this.form.value.name!,
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
          .subscribe(() => {
            toast.success('Trénink byl úspěšně vytvořen.');
            this.router.navigateByUrl('/dashboard');
          });
      });
  }
}
