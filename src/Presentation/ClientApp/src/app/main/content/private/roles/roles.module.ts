import { NgModule } from '@angular/core';
import { RolesComponent } from './roles.component';
import { RoleFormComponent } from './role-form/role-form.component';
import { RouterModule, Routes } from '@angular/router';
import { RolesService } from './roles.service';
import { RolesResolver } from './roles-resolver';
import { FeatureFlagModule } from '@app/core/modules/feature-flag.module';
import { LaunchDarklyService } from '@app/core/services/feature-flag/launch-darkly.service';
import { FeatureFlagService } from '@app/core/services/feature-flag/feature-flag.service';

import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';

import { FuseSharedModule } from '@fuse/shared.module';
import { FuseSidebarModule } from '@fuse/components';


const routes: Routes = [
  {
    path: '**',
    component: RolesComponent,
    resolve: {
      resolver: RolesResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),
    FeatureFlagModule,

    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatTableModule,
    CdkTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSidenavModule,
    MatListModule,
    MatCardModule,
    MatCheckboxModule,
    MatTooltipModule
  ],
  declarations: [
    RolesComponent,
    RoleFormComponent
  ],
  providers: [
    RolesService,
    RolesResolver,
    // Feature Flag Services
    LaunchDarklyService,
    FeatureFlagService
  ],
  entryComponents: [
    RoleFormComponent
  ]

})
export class RolesModule { }
