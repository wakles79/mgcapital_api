import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersComponent } from './customers.component';
import { CustomersMainComponent } from './sidenavs/main/main.component';
import { CustomersSelectedBarComponent } from './selected-bar/selected-bar.component';
import { CustomerListComponent } from './customer-list/customer-list.component';
import { CustomersService } from './customers.service';
import { RouterModule, Routes } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { CustomerFormModule } from '@app/core/modules/customer-form/customer-form.module';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { FuseWidgetModule } from '@fuse/components';
import { DeleteConfirmDialogModule } from '@app/core/modules/delete-confirm-dialog/delete-confirm-dialog.module';
import { MultipleDeleteConfirmDialogComponent } from '@app/core/modules/delete-confirm-dialog/multiple-delete-confirm-dialog/multiple-delete-confirm-dialog.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { CustomerProfileComponent } from './customer-profile/customer-profile.component';

const routes: Routes = [
  {
    path: '**',
    component: CustomersComponent,
    resolve: {
      resolver: CustomersService
    }
  }
];


@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    FuseWidgetModule,
    NgxChartsModule,
    MatProgressSpinnerModule,
    MatTableModule,
    CdkTableModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatSortModule,
    MatMenuModule,
    MatPaginatorModule,

    // App shared modules
    CustomerFormModule,
    DeleteConfirmDialogModule
  ],
  declarations: [
    CustomersComponent,
    CustomersMainComponent,
    CustomersSelectedBarComponent,
    CustomerListComponent,
    CustomerProfileComponent
  ],
  providers: [

  ],
  entryComponents: [
    CustomerFormComponent,
    MultipleDeleteConfirmDialogComponent
  ]
})
export class CustomersModule { }
