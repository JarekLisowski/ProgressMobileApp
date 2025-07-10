import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideIndexedDb, DBConfig } from 'ngx-indexed-db';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';

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

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    provideIndexedDb(dbConfig),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
  ]
};
