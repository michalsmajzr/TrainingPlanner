import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
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

interface CsrfResponse {
  requestToken: string;
}

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly url = 'https://localhost:7001/api/auth';

  login(request: LoginRequest): Observable<AuthUser> {
    return this.postWithCsrf<AuthUser>('login', request);
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.postWithCsrf<AuthUser>('register', request);
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

  private postWithCsrf<T>(path: string, body: unknown): Observable<T> {
    return this.http
      .get<CsrfResponse>(`${this.url}/csrf`, { withCredentials: true })
      .pipe(
        map((response) => response.requestToken),
        switchMap((token) =>
          this.http.post<T>(`${this.url}/${path}`, body, {
            headers: new HttpHeaders({ 'X-CSRF-TOKEN': token }),
            withCredentials: true,
          }),
        ),
      );
  }
}
