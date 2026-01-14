import { Injectable } from "@angular/core";
import { environment } from "../environments/environment";
import { IEnvironment } from "../environments/environment.prod";

@Injectable(
    {
        providedIn: 'root'
    }
)
export class AppConfigService {
    
    private _config: any;

    version = "1.2.0";

    constructor() {
        this._config = environment;
    }

    public getConfig() : IEnvironment {        
        return this._config
    }
}