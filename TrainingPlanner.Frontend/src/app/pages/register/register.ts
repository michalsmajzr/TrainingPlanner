import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { toast } from '@spartan-ng/brain/sonner';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { HlmInputImports } from '@spartan-ng/helm/input';
import { HlmLabelImports } from '@spartan-ng/helm/label';
import { AuthApiService } from '../../auth/auth-api.service';

@Component({
  selector: 'app-register-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    ...HlmButtonImports,
    ...HlmCardImports,
    ...HlmInputImports,
    ...HlmLabelImports,
  ],
  templateUrl: './register.html',
})
export class RegisterPage {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly router = inject(Router);

  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);

  private readonly passwordsMatch = (control: AbstractControl) =>
    control.get('password')?.value === control.get('confirmPassword')?.value
      ? null
      : { passwordsMismatch: true };

  readonly form = this.fb.nonNullable.group(
    {
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
    },
    { validators: this.passwordsMatch },
  );

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { firstName, lastName, email, password } = this.form.getRawValue();

    this.error.set(null);
    this.submitting.set(true);

    this.authApi
      .register({ firstName, lastName, email, password })
      .pipe(finalize(() => this.submitting.set(false)))
      .subscribe({
        next: () => {
          toast('Registrace byla úspěšná.');
          void this.router.navigateByUrl('/');
        },
        error: (error: unknown) =>
          this.error.set(
            this.authApi.errorMessage(error, 'Registrace se nezdařila.'),
          ),
      });
  }
}