import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VendorsComponent } from './vendors.component';
import { SelectedBarComponent } from './selected-bar/selected-bar.component';
import { MainComponent } from './sidenavs/main/main.component';
import { VendorFormComponent } from './vendor-form/vendor-form.component';
import { VendorListComponent } from './vendor-list/vendor-list.component';
import { VendorProfileComponent } from './vendor-profile/vendor-profile.component';
import { VendorProfileAdminComponent } from './vendor-profile/vendor-profile-admin/vendor-profile-admin.component';
import { VendorProfileProfileComponent } from './vendor-profile/vendor-profile-profile/vendor-profile-profile.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { Routes, RouterModule } from '@angular/router';
import { VendorsService } from './vendors.service';
import { VendorProfileService } from './vendor-profile/vendor-profile.service';
import { NgxChartsModule } from '@swimlane/ngx-charts';

const routes: Routes = [
  {
    path: ':id',
    component: VendorProfileComponent,
    resolve: {
      vendor: VendorProfileService
    },
    children: [
      {
        path: '',
        component: VendorProfileProfileComponent,
        outlet: 'profile'
      },
      {
        path: 'admin',
        component: VendorProfileAdminComponent,
        outlet: 'profile'
      },
    ]
  },
  {
    path: '',
    component: VendorsComponent,
    resolve: {
      vendors: VendorsService
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    NgxChartsModule

  ],
  declarations: [
    VendorsComponent,
    SelectedBarComponent,
    MainComponent,
    VendorFormComponent,
    VendorListComponent,
    VendorProfileComponent,
    VendorProfileAdminComponent,
    VendorProfileProfileComponent
  ],
  providers: [

  ],
  entryComponents: [
    VendorFormComponent,
  ]

})
export class VendorsModule { }
