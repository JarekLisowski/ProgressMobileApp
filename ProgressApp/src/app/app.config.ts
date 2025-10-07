import { ApplicationConfig, provideZoneChangeDetection, inject, provideAppInitializer } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideIndexedDb, DBConfig } from 'ngx-indexed-db';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
//import { AppConfigService } from '../services/app-config.service';

const dbConfig: DBConfig  = {
  name: 'ProgressDb',
  version: 2,
  objectStoresMeta: [{
    store: 'user',
    storeConfig: { keyPath: 'id', autoIncrement: false },
    storeSchema: [
      { name: 'name', keypath: 'name', options: { unique: false } },
    ]
  },
  {
    store: 'cart',
    storeConfig: { keyPath: 'id', autoIncrement: true },
    storeSchema: [
      { name: 'code', keypath: 'code', options: { unique: false } },
      { name: 'promoSetId', keypath: 'promoSetId', options: { unique: false } },
    ]
  },
  {
    store: 'promoSet',
    storeConfig: { keyPath: 'id', autoIncrement: true },
    storeSchema: [
      { name: 'promoSetId', keypath: 'promoSetId', options: { unique: false } },
    ]
  },
  {
    store: 'transaction',
    storeConfig: { keyPath: 'id', autoIncrement: true },
    storeSchema: []
  }
  ]
};

// export function initializeApp(http: HttpClient, appConfig: AppConfigService ) {

//     return () => new Promise<any>(res => {
//         http
//           .get("/config.json")
//           .subscribe(data => {
//             console.log("Read /config.json.");
//             appConfig.config = data            
//             res('ok');
//           });
//     });
//   }

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    provideIndexedDb(dbConfig),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    // provideAppInitializer(() => {
    //     const initializerFn = (initializeApp)(inject(HttpClient), inject(AppConfigService));
    //     return initializerFn();
    //   }),
  ]
};
