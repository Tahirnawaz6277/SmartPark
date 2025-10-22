import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, throwError, finalize, tap } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const startTime = Date.now();
  
  console.log(`[HTTP] ${req.method} ${req.url}`);

  return next(req).pipe(
    tap(response => {
      const elapsed = Date.now() - startTime;
      console.log(`[HTTP] ${req.method} ${req.url} - Success (${elapsed}ms)`);
    }),
    catchError((error: HttpErrorResponse) => {
      const elapsed = Date.now() - startTime;
      
      // Log error details
      console.error(`[HTTP] ${req.method} ${req.url} - Error (${elapsed}ms)`, {
        status: error.status,
        statusText: error.statusText,
        message: error.error?.Message || error.error?.message || error.message,
        error: error.error
      });

      // Handle different error types
      let errorMessage = 'An error occurred';
      
      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Client Error: ${error.error.message}`;
      } else {
        // Server-side error
        switch (error.status) {
          case 0:
            errorMessage = 'Unable to connect to server. Please check your internet connection.';
            break;
          case 400:
            errorMessage = error.error?.Message || error.error?.message || 'Bad request';
            break;
          case 401:
            errorMessage = 'Unauthorized. Please log in again.';
            break;
          case 403:
            errorMessage = 'Access forbidden. You do not have permission.';
            break;
          case 404:
            errorMessage = 'Resource not found.';
            break;
          case 500:
            errorMessage = error.error?.Message || error.error?.message || 'Internal server error';
            break;
          default:
            errorMessage = error.error?.Message || error.error?.message || `Error: ${error.status}`;
        }
      }

      // Attach user-friendly message to error
      const enhancedError = {
        ...error,
        userMessage: errorMessage
      };

      return throwError(() => enhancedError);
    }),
    finalize(() => {
      // Cleanup or logging after request completes
      const elapsed = Date.now() - startTime;
      console.log(`[HTTP] ${req.method} ${req.url} - Completed (${elapsed}ms)`);
    })
  );
};
