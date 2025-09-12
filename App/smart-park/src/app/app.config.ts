import { ApplicationConfig } from '@angular/core';
import { provideRouter, Router } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi, HTTP_INTERCEPTORS, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthService } from './services/auth.service';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    // {
    //   provide: HTTP_INTERCEPTORS,
    //   multi: true,
    //   useClass: class TokenInterceptor implements HttpInterceptor {
    //     private auth = inject(AuthService);
    //     intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    //       const token = this.auth.getToken();
    //       if (token) {
    //         const authReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    //         return next.handle(authReq);
    //       }
    //       return next.handle(req);
    //     }
    //   }
    // }

    {
      provide: HTTP_INTERCEPTORS,
      multi: true,
      useClass: class TokenInterceptor implements HttpInterceptor {
        private auth = inject(AuthService);
        private router = inject(Router);
    
        intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
          const token = this.auth.getToken();
    
          const authReq = token
            ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
            : req;
    
          return next.handle(authReq).pipe(
            catchError((error: HttpErrorResponse) => {
              if (error.status === 401) {
                this.auth.logout();
                this.router.navigate(['/login']);
              }
              return throwError(() => error);
            })
          );
        }
      }
    }
    
  ]
};
