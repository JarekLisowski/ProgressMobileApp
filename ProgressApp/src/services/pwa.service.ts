import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PwaService {
  private promptEvent: any;
  public installPrompt$ = new Subject<boolean>();

  constructor() {
    window.addEventListener('beforeinstallprompt', (event: any) => {
      // Prevent the mini-infobar from appearing on mobile
      event.preventDefault();
      // Stash the event so it can be triggered later.
      this.promptEvent = event;
      // Notify components that the app can be installed
      this.installPrompt$.next(true);
    });
  }

  public promptToInstall() {
    if (this.promptEvent) {
      // Show the install prompt
      this.promptEvent.prompt();
      // Wait for the user to respond to the prompt
      this.promptEvent.userChoice.then((choiceResult: any) => {
        if (choiceResult.outcome === 'accepted') {
          console.log('User accepted the install prompt');
        } else {
          console.log('User dismissed the install prompt');
        }
        // We can't use the prompt again, so clear it
        this.promptEvent = null;
        this.installPrompt$.next(false);
      });
    }
  }
}
