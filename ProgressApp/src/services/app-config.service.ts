import { Injectable } from "@angular/core";
import { Config, Profile } from "../domain/config";

@Injectable(
    {
        providedIn: 'root'
    }
)
export class AppConfigService {
    
    private _config: Config;// | null = null;//= new Config();

    constructor() {
        this._config = new Config();
        this._currentProfile = {
            backendApiUrl: 'http://localhost:5085/',
            backendUrl: 'http://localhost:5085/',
            name: "default"
        };
        // this._config.profiles.push({
        // /
        // // });
    }


    private _currentProfile: Profile = new Profile();

    public set config(data : any) {
        //console.dir(data);
        this._config = data as Config;
        this._config.profiles.forEach(profileItem => {
            if (this._config.currentProfileName == profileItem.name)
            {
                this._currentProfile = new Profile(profileItem);
            }
        });
    }

    public get config() {
        return this._config;
    }

    public getCurrentConfig() : Profile {
        console.log("Current config profile: " + this._currentProfile.name);
        return this._currentProfile;
    }
}