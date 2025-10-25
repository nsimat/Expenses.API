import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpRequest,
  HttpStatusCode
} from '@angular/common/http';
import {Observable, tap} from 'rxjs';
import {ErrorHandler, inject} from '@angular/core';
import {Router} from '@angular/router';
import {AuthService} from '../services/auth-service';

export const errorHandlerInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const errorHandler = inject(ErrorHandler);

  return next(req).pipe(
    // catch the error
    tap({
      error: (errorResponse: HttpErrorResponse) => {
        // if the status is 401 Unauthorized
        if(errorResponse.status === HttpStatusCode.Unauthorized){
          // Redirect to login page
          authService.logout();
          router.navigateByUrl('/login');
        } else  {
          // Notify the user
          errorHandler.handleError(errorResponse);
        }
      }
    })
  );
}
