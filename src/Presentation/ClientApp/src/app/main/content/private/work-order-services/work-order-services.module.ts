import { NgModule } from '@angular/core';
import { CategoriesListComponent } from './categories-list/categories-list.component';
import { CategoryFormComponent } from './category-form/category-form.component';
import { ServiceFormComponent } from './service-form/service-form.component';
import { ServicesListComponent } from './services-list/services-list.component';
import { WorkOrderServicesComponent } from './work-order-services.component';
import { RouterModule, Routes } from '@angular/router';
import { WorkOrderServicesService } from './work-order-services.service';
import { WorkOrderServicesResolver } from './work-order-services-resolver';

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
import {MatCardModule} from '@angular/material/card';
import {MatDividerModule} from '@angular/material/divider';
import {MatCheckboxModule} from '@angular/material/checkbox';
import { FuseSharedModule } from '@fuse/shared.module';

const routes: Routes = [
{
  path: '**',
  component: WorkOrderServicesComponent,
  resolve: {
    resolver: WorkOrderServicesResolver
  }
}
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),

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
    MatCardModule,
    MatDividerModule,
    MatCheckboxModule
  ],
  declarations: [
    CategoriesListComponent,
    CategoryFormComponent,
    ServiceFormComponent,
    ServicesListComponent,
    WorkOrderServicesComponent
  ],
  providers: [
    WorkOrderServicesService,
    WorkOrderServicesResolver
  ],
  entryComponents: [
   CategoryFormComponent,
   ServiceFormComponent
  ]

})
export class WorkOrderServicesModule { }
