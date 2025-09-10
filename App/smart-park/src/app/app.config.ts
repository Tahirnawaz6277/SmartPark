import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './services/auth.service';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: class TokenInterceptor implements HttpInterceptor {
        private auth = inject(AuthService);
        intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
          const token = this.auth.getToken();
          if (token) {
            const authReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
            return next.handle(authReq);
          }
          return next.handle(req);
        }
      }
    }
  ]
};
