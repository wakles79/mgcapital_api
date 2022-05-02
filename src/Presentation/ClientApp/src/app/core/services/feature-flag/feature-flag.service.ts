import { Injectable } from '@angular/core';
import { LaunchDarklyService } from '@app/core/services/feature-flag/launch-darkly.service';
import { LDFlagChangeset, LDFlagSet } from 'launchdarkly-js-client-sdk';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagService {

  private featureFlags: LDFlagSet;
  featureFlags$: BehaviorSubject<LDFlagSet> = new BehaviorSubject<LDFlagSet>(null);

  constructor(
    private featureFlagDataService: LaunchDarklyService) {

    this.featureFlagDataService.flagsOnInitialize$.subscribe((flags: LDFlagSet) => {
      this.featureFlags = flags;
      this.featureFlags$.next(this.featureFlags);
    });

    this.featureFlagDataService.flagsOnChange$
      .subscribe((flags: LDFlagChangeset) => {
        if (flags) {
          const keys = Object.keys(flags);
          keys.forEach(k => {
            this.featureFlags[k] = !!flags[k].current;
          });
          this.featureFlags$.next(this.featureFlags);
        }
      });
  }

  /**
   * Opposite of `featureOn` function
   * @param  {string} featureName
   */
  featureOff(featureName: string): boolean {
    return !this.featureOn(featureName);
  }

  /**
   * Verifies 'featureName' is activated, if no features or
   * 'fatureName' is not defined returns false to avoid misconfigurations
   * @param  {string} featureName
   */
  featureOn(featureName: string): boolean {
    if (!featureName) {
      return true;
    }

    return this.featureFlags &&
      this.featureFlags[featureName] !== undefined &&
      !!this.featureFlags[featureName];
  }



}
