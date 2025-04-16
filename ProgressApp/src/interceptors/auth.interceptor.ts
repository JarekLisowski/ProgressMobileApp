import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, finalize, switchMap, take } from 'rxjs/operators';
import {
  AUTHENTICATE_LOGIN_URL,
  AUTHENTICATE_REFRESH_TOKEN_URL,
} from '../services/urls.const';
import { AuthenticationService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(
    null,
  );
  constructor(private authService: AuthenticationService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    //console.log('Request URL: ' + request.url);
    if (
      request.url.indexOf(AUTHENTICATE_REFRESH_TOKEN_URL()) !== -1 ||
      request.url.indexOf(AUTHENTICATE_LOGIN_URL()) !== -1 ||
      request.url.indexOf('assets/config.json') !== -1
    ) {
      console.log("Handle request");
      return next.handle(request);
    }
    const authRequest = this.authService.addToken(request);
    console.log("Request with token " + request.url);    
    return next.handle(authRequest).pipe(
      catchError((error) => {
        console.log("Request error: "  + request.url);    
        if (error instanceof HttpErrorResponse && error.status === 401) {
          return this.handle401Error(request, next);
        } else {
          return throwError(() => error);
        }
      }),
    );
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response) => {
          this.refreshTokenSubject.next(response.accessToken);
          return next.handle(this.addToken(request, response.accessToken));
        }),
        finalize(() => (this.isRefreshing = false)),
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter((token) => token != null),
        take(1),
        switchMap((jwt) => {
          return next.handle(this.addToken(request, jwt));
        }),
      );
    }
  }

  private addToken(request: HttpRequest<any>, token: string | undefined) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
