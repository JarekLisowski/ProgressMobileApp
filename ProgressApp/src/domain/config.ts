export class Config {
    currentProfileName: string = "";
    profiles: Profile[] = [];
}

export class Profile {
    name : string = "";
    backendUrl : string = "";
    public get backendApiUrl() { 
        return this.backendUrl + 'api/';
    };

    constructor(data?: Profile) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }
}