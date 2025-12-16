import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SearchBarComponent } from "../search-bar/search-bar.component";
import { PwaService } from '../../services/pwa.service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-header',
    standalone: true,
    imports: [RouterModule, SearchBarComponent, CommonModule],
    templateUrl: './header.component.html',
    styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  showInstallButton = false;

  constructor(private pwaService: PwaService) {}

  ngOnInit() {
    this.pwaService.installPrompt$.subscribe(canInstall => {
      this.showInstallButton = canInstall;
    });
  }

  installPwa() {
    this.pwaService.promptToInstall();
  }
}
