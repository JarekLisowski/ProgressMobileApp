import { Injectable } from "@angular/core";
import { Config, Profile } from "../domain/config";
import { environment } from "../environments/environment";
import { IEnvironment } from "../environments/environment.prod";

@Injectable(
    {
        providedIn: 'root'
    }
)
export class AppConfigService {
    
    private _config: any;

    constructor() {
        // this._config = new Config();
        // var url = 'http://192.168.33.5';
        // this._currentProfile = {
        //     backendApiUrl: `${url}:5085/`,
        //     backendUrl: `${url}:4200/`,
        //     name: "test"
        // };
        this._config = environment;
    }


    // private _currentProfile: Profile = new Profile();

    // public set config(data : any) {
    //     //console.dir(data);
    //     this._config = data as Config;
    //     this._config.profiles.forEach(profileItem => {
    //         if (this._config.currentProfileName == profileItem.name)
    //         {
    //             this._currentProfile = new Profile(profileItem);
    //         }
    //     });
    // }

    // public get config() {
    //     return this._config;
    // }

    public getConfig() : IEnvironment {        
        return this._config
    }
}