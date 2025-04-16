// import { HttpClient, HttpRequest } from '@angular/common/http';
// import { Inject, Injectable } from '@angular/core';
// import { jwtDecode } from 'jwt-decode';
// import { finalize, map } from 'rxjs/operators';

// import { BehaviorSubject, Observable } from 'rxjs';
// import {
//   AUTHENTICATE_LOGIN_URL,
//   AUTHENTICATE_REFRESH_TOKEN_URL,
//   AUTHENTICATE_REVOKE_URL,
// } from './urls.const';
// import { UserInfo } from '../domain/user-info';
// //import { AuthLoginInfoDto, AuthLoginRequestDto } from '../classes/authLoginInfo';
// import { AppConfigService } from './app-config.service';
// //import { AuthLoginInfoDto, AuthLoginRequestDto } from '../classes/generated/apimodel';

// @Injectable({
//   providedIn: 'root',
// })
// export class AuthenticationService {
//   _userInfo: UserInfo | null | undefined = undefined;
//   private currentUserSubject: BehaviorSubject<UserInfo | null | undefined>;
//   public currentUser: Observable<UserInfo | null | undefined>;

//   // constructor(
//   //   private http: HttpClient,
//   //   private appConfigService: AppConfigService,
//   //   @Inject('LOCALSTORAGE') private localStorage: Storage,
//   // ) {
//   //   this.currentUserSubject = new BehaviorSubject<UserInfo | null | undefined>(
//   //     this.currentUserInfo,
//   //   );
//   //   this.currentUser = this.currentUserSubject.asObservable();
//   // }

//   private baseUrl() {
//     return this.appConfigService.getCurrentConfig().backendUrl;
//   }

//   private apiUrl() {
//     return this.appConfigService.getCurrentConfig().backendApiUrl;
//   }

//   // login(username: string, password: string): Observable<AuthLoginInfoDto> {
//   //   console.log("Login" + username);
//   //   return this.http
//   //     .post<AuthLoginInfoDto>(
//   //       `${this.apiUrl()}${AUTHENTICATE_LOGIN_URL()}`,
//   //       new AuthLoginRequestDto({ username: username, password: password }),
//   //     )
//   //     .pipe(map((response) => this.storeAuthResult(response)));
//   // }

//   // refreshToken(): Observable<AuthLoginInfoDto> {
//   //   return this.http
//   //     .post<AuthLoginInfoDto>(
//   //       `${this.apiUrl()}${AUTHENTICATE_REFRESH_TOKEN_URL()}`,
//   //       {
//   //         accessToken: this.currentUserInfo?.authLoginInfo.accessToken,
//   //         refreshToken: this.currentUserInfo?.authLoginInfo.refreshToken,
//   //       },
//   //     )
//   //     .pipe(map((response) => this.storeAuthResult(response)));
//   // }

//   logout(): Observable<boolean> {
//     this._userInfo = undefined;
//     this.currentUserSubject.next(null);
//     return this.http
//       .post(`${this.baseUrl()}${AUTHENTICATE_REVOKE_URL()}`, {})
//       .pipe(
//         finalize(() => {
//           // clear token remove user from local storage to log user out
//           this.logoutLocally();
//         }),
//         map(() => true),
//       );
//   }

//   logoutLocally() {
//     this.localStorage.removeItem('currentUser');
//     this.localStorage.removeItem('currentUserAuthInfo');
//     this.localStorage.removeItem('language');
//     this._userInfo = undefined;
//   }

//   // get currentUserInfo(): UserInfo | null {
//   //   // TODO: Enable after implementation
//   //   if (this._userInfo === undefined) {
//   //     const cu = this.localStorage.getItem('currentUser');
//   //     const cua = this.localStorage.getItem('currentUserAuthInfo');
//   //     if (!cu || !cua) {
//   //       return null;
//   //     }

//   //     const ali = new AuthLoginInfoDto(JSON.parse(cua));
//   //     this._userInfo = this.createUserInfo(JSON.parse(cu), ali);
//   //   }
//   //   return this._userInfo;
//   // }

//   // private createUserInfo(
//   //   currentUser: any,
//   //   authResult: AuthLoginInfoDto,
//   // ): UserInfo {
//   //   return new UserInfo(currentUser, authResult);
//   // }

//   // addToken(request: HttpRequest<any>) {
//   //   const currentUser = this.currentUserInfo;
//   //   const isLoggedIn = currentUser?.authLoginInfo?.accessToken;
//   //   const isApiUrl = request.url.startsWith(request.url);
//   //   if (isLoggedIn && isApiUrl) {
//   //     return request.clone({
//   //       setHeaders: {
//   //         Authorization: `Bearer ${currentUser.authLoginInfo.accessToken}`,
//   //       },
//   //     });
//   //   }
//   //   return request;
//   // }

//   // private storeAuthResult(authResult: AuthLoginInfoDto): AuthLoginInfoDto {
//   //   if (!authResult || !authResult.accessToken) {
//   //     throw new Error('Accesss Token is not valid.');
//   //   }
//   //   this._userInfo = undefined;
//   //   // set token property
//   //   const decodedToken = jwtDecode(authResult.accessToken);

//   //   //store email and jwt token in local storage to keep user logged in between page refreshes
//   //   this.localStorage.setItem('currentUser', JSON.stringify(decodedToken));
//   //   //store refresh token
//   //   this.localStorage.setItem(
//   //     'currentUserAuthInfo',
//   //     JSON.stringify(authResult),
//   //   );
//   //   this.currentUserSubject.next(this.createUserInfo(decodedToken, authResult));
//   //   return authResult;
//   // }
// }
