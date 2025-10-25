import {HttpInterceptorFn} from '@angular/common/http';
import {inject} from '@angular/core';
import {AuthService} from '../services/auth-service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  // if the token is present, clone the rerquest
  // replacing the original headers with the authorization
  if(token){
    req = req.clone({
      // Add a Bearer token as a header to access the API
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  // Send the request to the next handler
  return next(req);
}
