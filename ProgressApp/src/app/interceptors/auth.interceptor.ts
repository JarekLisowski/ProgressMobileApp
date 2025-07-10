import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { map, Observable, switchMap, tap } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { UserService } from '../../services/user.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService, private userSerivce: UserService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var obs = this.userSerivce.getToken().pipe(
            switchMap(token => {
                if (token) {
                    request = request.clone({
                        setHeaders: {
                            Authorization: `Bearer ${token}`
                        }
                    });
                }
                return next.handle(request);
            }),
        );
        return obs;
    }
}
