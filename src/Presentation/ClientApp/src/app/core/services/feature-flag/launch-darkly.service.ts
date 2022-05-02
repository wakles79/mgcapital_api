import { Injectable } from '@angular/core';
import { ConfigLoaderService } from '@app/core/services/config-loader.service';
import { initialize, LDClient, LDFlagChangeset, LDFlagSet } from 'launchdarkly-js-client-sdk';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LaunchDarklyService {
  ldClient: LDClient;
  flags: LDFlagSet;
  flagsOnChange$: Subject<LDFlagChangeset> = new Subject<LDFlagChangeset>();
  flagsOnInitialize$: BehaviorSubject<LDFlagSet> = new BehaviorSubject<LDFlagSet>([]);

  constructor(
    private configService: ConfigLoaderService
  ) {
    const config = this.configService.config;
    this.ldClient = initialize(config.launchDarkly.clientId,
      { key: config.launchDarkly.userKey });

    this.ldClient.on('change', (flags: LDFlagChangeset) => {
      this.flagsOnChange$.next(flags);
    });

    this.ldClient.on('ready', () => {
      this.initialize();
    });

  }

  initialize(): void {
    this.flags = this.ldClient.allFlags();
    this.flagsOnInitialize$.next(this.flags);
  }

  changeUser(user: string): void {
    if (user !== 'Anonymous') {
      this.ldClient.identify({ key: user, name: user, anonymous: false });
    }
    else {
      this.ldClient.identify({ key: 'anon', anonymous: true });
    }
  }

}
