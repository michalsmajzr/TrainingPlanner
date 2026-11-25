import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map, switchMap } from 'rxjs';

export interface AuthUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest extends LoginRequest {
  firstName: string;
  lastName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly url = '/api/auth';

  login(request: LoginRequest): Observable<AuthUser> {
    return this.postAndRefreshXsrf<AuthUser>('login', request);
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.postAndRefreshXsrf<AuthUser>('register', request);
  }

  errorMessage(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse && error.error) {
      const description = (error.error as { description?: unknown }).description;
      if (typeof description === 'string') {
        return description;
      }
    }

    return fallback;
  }

  private postAndRefreshXsrf<T>(path: string, body: unknown): Observable<T> {
    // Refresh after authentication: ASP.NET Core binds the token to the user.
    return this.http.post<T>(`${this.url}/${path}`, body).pipe(
      switchMap(result =>
        this.http.get<void>(`${this.url}/csrf`).pipe(map(() => result)),
      ),
    );
  }
}
