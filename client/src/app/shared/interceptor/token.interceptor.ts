import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';

import { AuthenticationService } from '../services/authentication.service';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService, private router: Router) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    request = this.setHeader(request);
    return next.handle(request)
      .pipe(catchError((response) => {
        if (response.status === 401) {
          this.router.navigate(['/auth/login'], {
            queryParams: {
              returnUrl: this.router.url
            }
          });
        }
        return throwError(response.error || response);
      }));

  }

  private setHeader(req: HttpRequest<any>) {
    const currentToken = this.authenticationService.currentTokenValue;
    if (currentToken) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${currentToken}`,
        }
      });
    }
    return req;
  }
}
