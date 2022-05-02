import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OfficeTypesComponent } from './office-types.component';
import { OfficeTypeListComponent } from './office-type-list/office-type-list.component';
import { Routes, RouterModule } from '@angular/router';
import { OfficeTypesResolver } from './office-types-resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { OfficeTypeFormModule } from '@app/core/modules/office-type-form/office-type-form.module';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatRippleModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';

const routes: Routes = [
  {
    path: '**',
    component: OfficeTypesComponent,
    resolve: {
      resolver: OfficeTypesResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatMenuModule,
    MatPaginatorModule,
    MatRippleModule,
    MatInputModule,

    // Add shared modules
    OfficeTypeFormModule
  ],
  declarations: [
    OfficeTypesComponent,
    OfficeTypeListComponent
  ],
  providers: [

  ],
  entryComponents: [
    OfficeTypeFormComponent
  ]

})
export class OfficeTypesModule { }
