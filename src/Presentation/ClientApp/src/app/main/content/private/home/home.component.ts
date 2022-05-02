import { Component, OnInit, AfterViewInit } from '@angular/core';
import { AuthService } from '@core/services/auth.service';
import { MobileAppService } from '@app/core/services/mobile-app.service';
import { MobileApp, MobilePlatform } from '@app/core/models/mobile/mobile-app.enum';
import { Observable } from 'rxjs';
import { MobileAppVersion } from '@app/core/models/mobile/mobile-app-version.interface';
import { fuseAnimations } from '@fuse/animations';
import { RolesService } from '../roles/roles.service';
import { ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { PermissionAssignmentModel } from '@app/core/models/permission/permission-assignment.model';

@Component({
  selector: 'fuse-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  animations: fuseAnimations
})
export class HomeComponent implements OnInit, AfterViewInit {

  mobileAppVersions$: Observable<MobileAppVersion>;
  MobileApp = MobileApp;
  MobilePlatform = MobilePlatform;

  viewMobileAppVersions = false;


  ngOnInit(): void {
    // Fetches latest versions
    this.mobileAppVersions$ = this.mobileAppService.latestVersions();
  }
  /**
   *
   */
  constructor(
    public authService: AuthService,
    private mobileAppService: MobileAppService,
    private _roleService: RolesService,
  ) {
  }

  ngAfterViewInit(): void {
    this._roleService.getModulePermissions(ApplicationModule.Dashboard)
      .subscribe((permissions: PermissionAssignmentModel[]) => {
        this.viewMobileAppVersions = permissions.findIndex(p => p.name === 'ViewMobileAppVersions' && p.isAssigned) >= 0 ? true : false;
      });
  }

}
