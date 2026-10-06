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

    version = "2026.5";

    constructor() {
        this._config = environment;
    }

    public getConfig(): IEnvironment {
        return this._config
    }

    private readonly VIEW_STYLE_KEY = 'viewStyle';

    public get viewStyle(): number {
        const stored = localStorage.getItem(this.VIEW_STYLE_KEY);
        return stored !== null ? Number(stored) : 0;
    }

    public set viewStyle(value: number) {
        localStorage.setItem(this.VIEW_STYLE_KEY, value.toString());
    }
}