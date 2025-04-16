// import {
//   HttpErrorResponse,
//   HttpEvent,
//   HttpHandler,
//   HttpInterceptor,
//   HttpRequest,
// } from '@angular/common/http';
// import { Injectable } from '@angular/core';
// import { Router } from '@angular/router';
// import { Observable, throwError } from 'rxjs';
// import { catchError } from 'rxjs/operators';
// //import { AuthenticationService } from '../services/auth.service';

// @Injectable()
// export class ErrorInterceptor implements HttpInterceptor {
//   constructor(
//     private authService: AuthenticationService,
//     private router: Router,
//   ) {}
//   intercept(
//     request: HttpRequest<unknown>,
//     next: HttpHandler,
//   ): Observable<HttpEvent<unknown>> {
//     return next.handle(request).pipe(
//       catchError((error) => {
//         console.log("Error interceptor!");
//         console.dir(error);
//         if (error instanceof HttpErrorResponse && error.status === 401) {
//           console.log("Error interceptor 2!");
//           //this.dialog.closeAll();
//           this.router.navigate(['/auth/login']);
//         }
//         return throwError(() => error);
//       }),
//     );
//   }
// }
