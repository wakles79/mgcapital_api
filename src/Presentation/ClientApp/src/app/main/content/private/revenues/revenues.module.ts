import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RevenuesComponent } from './revenues.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { Routes, RouterModule } from '@angular/router';
import { RevenuesResolver } from './revenues-resolver';
import { RevenueFormComponent } from './revenue-form/revenue-form.component';
import { RevenueImportCsvFormComponent } from './revenue-import-csv-form/revenue-import-csv-form.component';
import { ImportRevenueExpenseCsvFormModule } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form.module';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { FuseSidebarModule } from '@fuse/components';
import { RevenueListComponent } from './revenue-list/revenue-list.component';
import { SelectedBarComponent } from './selected-bar/selected-bar.component';
import { MainComponent } from './sidenavs/main/main.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatOptionModule, MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CdkTableModule } from '@angular/cdk/table';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialogModule } from '@angular/material/dialog';

const routes: Routes = [
  {
    path: '',
    component: RevenuesComponent,
    resolve: {
      resolver: RevenuesResolver
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
    MatSelectModule,
    NgxMatSelectSearchModule,
    MatOptionModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatCheckboxModule,
    MatMenuModule,
    MatPaginatorModule,
    MatToolbarModule,
    MatDialogModule,

    // Add shared modules
    ImportRevenueExpenseCsvFormModule
  ],
  declarations: [
    RevenuesComponent,
    RevenueFormComponent,
    RevenueImportCsvFormComponent,
    RevenueListComponent,
    SelectedBarComponent,
    MainComponent
  ],
  providers: [

  ],
  entryComponents: [
    RevenueFormComponent,
    RevenueImportCsvFormComponent,
    ImportRevenueExpenseCsvFormComponent
  ]

})
export class RevenuesModule { }
