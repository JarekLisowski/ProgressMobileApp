import { Injectable } from '@angular/core';
import { NgxIndexedDBService } from 'ngx-indexed-db';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class DataStorage {
    constructor(private dbService: NgxIndexedDBService) {}

    // Example method to add data
    addData(storeName: string, data: any): Observable<any> {
        return this.dbService.add(storeName, data);
    }

    // Example method to get data by key
    getDataByKey(storeName: string, key: any): Observable<any> {
        return this.dbService.getByKey(storeName, key);
    }

    // Example method to delete data by key
    deleteData(storeName: string, key: any): Observable<any> {
        return this.dbService.delete(storeName, key);
    }

    // Example method to get all data
    getAllData(storeName: string): Observable<any[]> {
        return this.dbService.getAll(storeName);
    }
}