import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExpensesComponent } from './expenses.component';
import { Routes, RouterModule } from '@angular/router';
import { ExpensesRevolver } from './expenses-resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { FuseSidebarModule } from '../../../../../@fuse/components/sidebar/sidebar.module';
import { ExpenseFormComponent } from './expense-form/expense-form.component';
import { ImportRevenueExpenseCsvFormModule } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form.module';
import { MainComponent } from './sidenavs/main/main.component';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { ExpenseListComponent } from './expense-list/expense-list.component';
import { SelectedBarComponent } from './selected-bar/selected-bar.component';
import { ExpenseImportCsvComponent } from './expense-import-csv/expense-import-csv.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatOptionModule, MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';


const routes: Routes = [
  {
    path: '**',
    component: ExpensesComponent,
    resolve: {
      resolver: ExpensesRevolver
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
    MatDialogModule,
    MatProgressSpinnerModule,
    MatTableModule,
    CdkTableModule,
    MatCheckboxModule,
    MatMenuModule,
    MatPaginatorModule,
    MatSortModule,
    MatToolbarModule,
    MatSlideToggleModule,

    // Add pipes

    // Add shared modules
    ImportRevenueExpenseCsvFormModule

  ],
  declarations: [
    ExpensesComponent,
    ExpenseFormComponent,
    MainComponent,
    ExpenseListComponent,
    SelectedBarComponent,
    ExpenseImportCsvComponent
  ],
  providers: [

  ],
  entryComponents: [
    ExpenseFormComponent,
    ExpenseImportCsvComponent,
    ImportRevenueExpenseCsvFormComponent
  ]

})
export class ExpensesModule { }
