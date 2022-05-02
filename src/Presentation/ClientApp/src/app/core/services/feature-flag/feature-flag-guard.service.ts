import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { FeatureFlagService } from '@app/core/services/feature-flag/feature-flag.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagGuardService implements CanActivate {

  constructor(
    private featureFlagService: FeatureFlagService
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {

    const featureFlag = route.data['featureFlag'];
    if (!featureFlag) {
      return true;
    }

    return this.featureFlagService.featureFlags$
      .pipe(
        map((flags: any) => {
          return this.featureFlagService.featureOn(featureFlag);
        })
      );

  }

}
