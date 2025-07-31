import { Injectable } from "@angular/core";
import { NgxIndexedDBService } from "ngx-indexed-db";
import { User } from "../domain/generated/apimodel";
import { from, map, Observable, of, ReplaySubject, switchMap } from "rxjs";

@Injectable({
    providedIn: 'root'
})

export class UserService {

    token: string | undefined;

    constructor(private dbService: NgxIndexedDBService) {
    }

    isLoggedIn(): Observable<boolean> {
        return this.dbService.getAll<User>('user').pipe(
            map(x => {
                if (x.length > 0) {
                    return true;
                }
                return false;
            })
        );        
    }

    getUser(): Observable<User | null> {
        return this.dbService.getAll<User>('user').pipe(
            map(x => {
                if (x.length > 0) {
                    return x[0];
                }
                return null;
            })
        );
        
    }


    getToken(): Observable<string | undefined> {
        if (this.token != undefined) {
            return of(this.token);
        }
        var obs = this.dbService.getAll<User>('user').pipe(
            map(x => {
                if (x.length == 0) {
                    return undefined;
                }
                var token = x[0].token;
                return token;;
            })
        );
        return obs;
    }


    setUser(user: User): Observable<any> {
        return this.dbService.clear('user').pipe(
            switchMap(x => {
                return this.dbService.add('user', user);
            })
        );
    }

}