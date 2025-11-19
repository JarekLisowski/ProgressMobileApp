import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, filter, subscribeOn, switchMap, tap } from 'rxjs/operators';
import { UserService } from '../../services/user.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private userService: UserService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err.status === 401) {
                this.userService.isLoggedIn().pipe(
                    tap(loggedIn => console.log("Bład 401. Zalogowany: " + loggedIn)),
                    filter(loggedIn => loggedIn),
                    switchMap(() => this.userService.logout()))
                    .subscribe(() => {
                        console.log("Wylogowano użytkownika z powodu błedu 401.");
                        location.reload();
                    });                
            }
            const error = err.message ?? err.statusText ?? "Nieznany błąd HTTP.";
            return throwError(error);
        }))
    }
}
