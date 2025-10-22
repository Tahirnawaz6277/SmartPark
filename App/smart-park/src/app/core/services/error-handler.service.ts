import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {

  constructor() {}

  /**
   * Get user-friendly error message from HTTP error
   */
  getErrorMessage(error: any): string {
    if (error instanceof HttpErrorResponse) {
      // Server-side error
      if (error.status === 0) {
        return 'Unable to connect to server. Please check your internet connection.';
      }
      
      if (error.status === 400) {
        return error.error?.message || 'Invalid request. Please check your input.';
      }
      
      if (error.status === 401) {
        return 'Unauthorized. Please login again.';
      }
      
      if (error.status === 403) {
        return 'Access denied. You do not have permission to perform this action.';
      }
      
      if (error.status === 404) {
        return 'Resource not found.';
      }
      
      if (error.status === 409) {
        return error.error?.message || 'Conflict. This resource already exists.';
      }
      
      if (error.status === 422) {
        return error.error?.message || 'Validation error. Please check your input.';
      }
      
      if (error.status >= 500) {
        return 'Server error. Please try again later.';
      }
      
      // Return server message if available
      return error.error?.message || error.message || 'An unexpected error occurred.';
    }
    
    // Client-side error
    return error?.message || 'An unexpected error occurred.';
  }

  /**
   * Show error notification (can be extended with toast/snackbar)
   */
  showError(message: string): void {
    console.error('Error:', message);
    // TODO: Integrate with toast/snackbar service
    // this.toastService.error(message);
  }

  /**
   * Show success notification
   */
  showSuccess(message: string): void {
    console.log('Success:', message);
    // TODO: Integrate with toast/snackbar service
    // this.toastService.success(message);
  }

  /**
   * Log error for debugging
   */
  logError(error: any, context?: string): void {
    const errorMessage = this.getErrorMessage(error);
    console.error(`[${context || 'Error'}]:`, errorMessage, error);
  }
}
