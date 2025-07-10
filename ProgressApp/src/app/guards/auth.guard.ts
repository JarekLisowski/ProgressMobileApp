import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, MaybeAsync, GuardResult } from '@angular/router';
import { UserService } from '../../services/user.service';
import { map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private userService: UserService
    ) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): MaybeAsync<GuardResult> {
        return this.userService.isLoggedIn().pipe(
            map(isLoggedIn => {
                if (isLoggedIn) {
                    return true;
                }
                this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
                return false;
            })
        );
    }
}
