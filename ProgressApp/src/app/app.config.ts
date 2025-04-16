import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideIndexedDb, DBConfig } from 'ngx-indexed-db';
import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';

const dbConfig: DBConfig  = {
  name: 'ProgressDb',
  version: 1,
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
    ]
  }
  ]
};

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    provideIndexedDb(dbConfig)
  ]
};
