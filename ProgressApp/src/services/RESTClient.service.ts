import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { AppConfigService } from "./app-config.service";

@Injectable({
    providedIn : 'root'
}
)

export class RESTClientService {
        
    public baseUrl() {
        return this.appConfigService.getCurrentConfig().backendUrl;
    }

    constructor(private appConfigService: AppConfigService, private httpClient : HttpClient) {
    }

    get<T>(apiAddress : string) : Observable<T> {
        var url = this.baseUrl() + apiAddress;
        return this.httpClient.get<T>(url);
    }

    delete<T>(apiAddress : string) : Observable<T> {
        var url = this.baseUrl() + apiAddress;
        return this.httpClient.delete<T>(url);
    }

    post<T>(apiAddress : string, data: any) : Observable<T> {
        var url = this.baseUrl() + apiAddress;
        return this.httpClient.post<T>(url, data);
    }

    put<T>(apiAddress : string, data: any) : Observable<T> {
        var url = this.baseUrl() + apiAddress;
        return this.httpClient.put<T>(url, data);
    }
}