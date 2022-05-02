import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContractsComponent } from './contracts.component';
import { MainComponent } from './sidenavs/main/main.component';
import { ContractListComponent } from './contract-list/contract-list.component';
import { ContractFormComponent } from './contract-form/contract-form.component';
import { Routes, RouterModule } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { ContractsResolver } from './contracts-resolver';
import { FuseSidebarModule } from '@fuse/components';
import { EditContractFormComponent } from './edit-contract-form/edit-contract-form.component';
import { ImportRevenueExpenseCsvFormModule } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form.module';
import { ImportRevenueExpenseCsvFormComponent } from '@app/core/modules/import-revenue-expense-csv-form/import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { ResultImportCsvModule } from '@app/core/modules/result-import-csv/result-import-csv.module';
import { ResultImportCsvComponent } from '@app/core/modules/result-import-csv/result-import-csv/result-import-csv.component';
import { NewContractFormComponent } from './new-contract-form/new-contract-form.component';
import { ContractConfirmDialogComponent } from './contract-confirm-dialog/contract-confirm-dialog.component';
import { PipesModule } from '@app/core/pipes/pipes.module';
import { BudgetDetailComponent } from './budget-detail/budget-detail.component';
import { BudgetTrackingComponent } from './budget-tracking/budget-tracking.component';
import { ContractSummaryComponent } from './contract-summary/contract-summary.component';
import { ContractBalanceDetailComponent } from './contract-balance-detail/contract-balance-detail.component';
import { ContractReportDetailComponent } from './contract-report-detail/contract-report-detail.component';
import { DeleteItemConfirmDialogComponent } from './delete-item-confirm-dialog/delete-item-confirm-dialog.component';
import { OfficeSpaceFormComponent } from './office-space-form/office-space-form.component';
import { CustomerFormModule } from '@app/core/modules/customer-form/customer-form.module';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';
import { BuildingFormModule } from '@app/core/modules/building-form/building-form.module';
import { BuildingFormComponent } from '@app/core/modules/building-form/building-form/building-form.component';
import { BudgetDetailResolver } from './budget-detail/budget-detail-resolver';
import { ContractExpenseFormComponent } from './contract-form/contract-expense-form/contract-expense-form.component';
import { ContractItemFormComponent } from './contract-form/contract-item-form/contract-item-form.component';
import { BudgetActivityLogNotesDialogModule } from '@app/core/modules/budget-activity-log-notes-dialog/budget-activity-log-notes-dialog.module';
import { BudgetActivityLogNotesDialogComponent } from '@app/core/modules/budget-activity-log-notes-dialog/budget-activity-log-notes-dialog/budget-activity-log-notes-dialog.component';
import { ExpensesFormModule } from '@app/core/modules/expenses-form/expenses-form.module';
import { BudgetTrackingResolver } from './budget-tracking/budget-tracking-resolver';
import { ExpensesFormComponent } from './contract-balance-detail/expenses-form/expenses-form.component';
import { RevenuesFormComponent } from './contract-balance-detail/revenues-form/revenues-form.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { LightboxModule } from 'ngx-lightbox';
import { ContractReportDetailResolver } from './contract-report-detail/contract-report-detail-resolver';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { SearchContactFormComponent } from '@app/core/modules/contact-form/search-contact-form/search-contact-form.component';
import { ContactFormModule } from '@app/core/modules/contact-form/contact-form.module';
import { OfficeTypeFormModule } from '@app/core/modules/office-type-form/office-type-form.module';
import { OfficeTypeFormComponent } from '@app/core/modules/office-type-form/office-type-form.component';
import { AddressFormComponent } from '@app/core/modules/contact-form/address-form/address-form.component';
import { ExpenseTypeFormComponent } from '@app/core/modules/expenses-form/expense-type-form/expense-type-form.component';
import { ExpenseSubcategoryFormComponent } from '@app/core/modules/expenses-form/expense-subcategory-form/expense-subcategory-form.component';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { ShareUrlDialogModule } from '@app/core/modules/share-url-dialog/share-url-dialog.module';
import { ContractReportDetailBalanceResolver } from './contract-balance-detail/contract-balance-detail-resolver';
import { MainBalanceComponent } from './contract-balance-detail/sidenavs/main-balance/main.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';


const routes: Routes = [
  {
    path: 'budgets',
    component: ContractsComponent,
    resolve: {
      services: ContractsResolver
    }
  },
  {
    path: 'budget-report/:id',
    component: BudgetDetailComponent,
    resolve: {
      services: BudgetDetailResolver
    }
  },
  {
    path: 'budget-balance/:id',
    component: ContractReportDetailComponent,
    resolve: {
      services: ContractReportDetailResolver
    }
  },
  {
    path: 'budget-tracking/:id',
    component: BudgetTrackingComponent,
    resolve: {
      services: BudgetTrackingResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),
    NgxChartsModule,
    LightboxModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatSelectModule,
    NgxMatSelectSearchModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatRippleModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatMenuModule,
    MatPaginatorModule,
    MatToolbarModule,
    MatDialogModule,
    MatCardModule,
    MatCheckboxModule,
    MatSidenavModule,
    MatListModule,
    MatTooltipModule,
    MatExpansionModule,

    // Add shared modules
    ImportRevenueExpenseCsvFormModule,
    ResultImportCsvModule,
    CustomerFormModule,
    BuildingFormModule,
    BudgetActivityLogNotesDialogModule,
    ExpensesFormModule,
    ContactFormModule,
    OfficeTypeFormModule,
    ShareUrlDialogModule,

    // Add pipes
    PipesModule
  ],
  declarations: [
    ContractsComponent,
    MainComponent,
    MainBalanceComponent,
    ContractListComponent,
    ContractFormComponent,
    EditContractFormComponent,
    NewContractFormComponent,
    ContractConfirmDialogComponent,
    BudgetDetailComponent,
    BudgetTrackingComponent,
    ContractSummaryComponent,
    ContractBalanceDetailComponent,
    ContractReportDetailComponent,
    DeleteItemConfirmDialogComponent,
    OfficeSpaceFormComponent,
    ContractExpenseFormComponent,
    ContractItemFormComponent,
    ExpensesFormComponent,
    RevenuesFormComponent
  ],
  providers: [
    ContractReportDetailBalanceResolver,
  ],
  entryComponents: [
    ContractFormComponent,
    SearchContactFormComponent,
    CustomerFormComponent,
    ContractItemFormComponent,
    ContractExpenseFormComponent,
    OfficeTypeFormComponent,
    BuildingFormComponent,
    AddressFormComponent,
    ExpenseTypeFormComponent,
    ExpenseSubcategoryFormComponent,
    ShareUrlDialogComponent,
    OfficeSpaceFormComponent,
    ExpensesFormComponent,
    RevenuesFormComponent,
    ImportRevenueExpenseCsvFormComponent,
    ContractConfirmDialogComponent,
    ResultImportCsvComponent,
    EditContractFormComponent,
    NewContractFormComponent,
    DeleteItemConfirmDialogComponent,
    BudgetActivityLogNotesDialogComponent
  ]

})
export class ContractsModule { }
