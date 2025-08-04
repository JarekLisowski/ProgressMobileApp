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
        var url = 'http://192.168.33.2';
        this._currentProfile = {
            backendApiUrl: `${url}:5085/`,
            backendUrl: `${url}:4200/`,
            name: "test"
        };
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