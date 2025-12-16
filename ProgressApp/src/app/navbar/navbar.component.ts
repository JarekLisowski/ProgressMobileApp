import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PwaService } from '../../services/pwa.service';
import { AppConfigService } from '../../services/app-config.service';

@Component({
    selector: 'app-navbar',
    imports: [RouterModule],
    templateUrl: './navbar.component.html',
    styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
    showInstallButton = false;
    @Output() LogoutEvent = new EventEmitter<any>();
    pwaService = inject(PwaService);
    appConfigService = inject(AppConfigService);
    get appVersion() {
        return this.appConfigService.version;
    }


    ngOnInit() {
        this.pwaService.installPrompt$.subscribe(canInstall => {
            this.showInstallButton = canInstall;
        });
    }

    installPwa() {
        this.pwaService.promptToInstall();
    }
}
