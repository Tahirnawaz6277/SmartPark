import { Observable } from 'rxjs';
import { Auth } from '../services/auth';
import { Injectable } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';


// @Injectable()
// export class authInterceptor implements  HttpInterceptor  {

//   constructor(private authservice : Auth){}

//   intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//     const token = this.authservice.getToken();
//     if (token) {
//       const cloned = req.clone({
//         setHeaders : {Authorization : `Bearer ${token}}`}
//       });
//       return next.handle(cloned);
//     }
//     return next.handle(req);

//   }


// };

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(req);
};


