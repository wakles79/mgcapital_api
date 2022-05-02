import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExpenseTypesListComponent } from './expense-types-list/expense-types-list.component';
import { ExpensesTypesComponent } from './expenses-types.component';
import { ExpensesTypesResolver } from './expenses-types-resolver';
import { RouterModule, Routes } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { FuseSidebarModule } from '@fuse/components/sidebar/sidebar.module';
import { ExpenseTypeFormComponent } from '@app/core/modules/expenses-form/expense-type-form/expense-type-form.component';
import { ExpenseSubcategoryFormComponent } from '@app/core/modules/expenses-form/expense-subcategory-form/expense-subcategory-form.component';
import { ExpensesFormModule } from '@app/core/modules/expenses-form/expenses-form.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatMenuModule } from '@angular/material/menu';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

const routes: Routes = [
  {
    path: '**',
    component: ExpensesTypesComponent,
    resolve: {
      resolver: ExpensesTypesResolver
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
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatMenuModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,


    // Add shared modules
    ExpensesFormModule

  ],
  declarations: [
    ExpensesTypesComponent,
    ExpenseTypesListComponent
  ],
  providers: [

  ],
  entryComponents: [
    ExpenseTypeFormComponent,
    ExpenseSubcategoryFormComponent
  ]
})
export class ExpensesTypesModule { }
