import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ExpenseTypeFormComponent } from './expense-type-form/expense-type-form.component';
import { FuseSharedModule } from '../../../../@fuse/shared.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatDialogModule } from '@angular/material/dialog';
import { ExpenseSubcategoryFormComponent } from './expense-subcategory-form/expense-subcategory-form.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';



@NgModule({
  imports: [

    FuseSharedModule,
    MatIconModule,
    // MatProgressSpinnerModule,
    MatButtonModule,
    // MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    // MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatExpansionModule,
    MatListModule,
    MatTooltipModule,
    MatSelectModule,
    MatCheckboxModule,

  ],
  declarations: [
    ExpenseTypeFormComponent,
    ExpenseSubcategoryFormComponent
  ],
  exports: [
    ExpenseTypeFormComponent,
    ExpenseSubcategoryFormComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class ExpensesFormModule { }
