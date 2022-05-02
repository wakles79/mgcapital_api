import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BuildingsProfileComponent } from './buildings-profile.component';
import { BuildingProfileDetailComponent } from './building-profile-detail/building-profile-detail.component';
import { BuildingProfileListComponent } from './building-profile-list/building-profile-list.component';
import { MainComponent } from './sidenavs/main/main.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { BuildingsProfileResolver } from './buildings-profile-resolver';
import { BuildingsDetailResolver } from './building-profile-detail/building-profile-detail-resolver';
import { FuseSidebarModule } from '@fuse/components/sidebar/sidebar.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatRippleModule } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';

const routes: Routes = [
  {
    path: 'buildings-profile',
    component: BuildingsProfileComponent,
    resolve: {
      resolver: BuildingsProfileResolver
    }
  },
  {
    path: 'buildings-profile-detail/:id',
    component: BuildingProfileDetailComponent,
    resolve: {
      resolver: BuildingsDetailResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatInputModule,
    MatRippleModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatTabsModule,
    MatCardModule,


  ],
  declarations: [
    BuildingsProfileComponent,
    BuildingProfileDetailComponent,
    BuildingProfileListComponent,
    MainComponent
  ],
  providers: [

  ],
  entryComponents: [

  ]

})
export class BuildingsProfileModule { }
