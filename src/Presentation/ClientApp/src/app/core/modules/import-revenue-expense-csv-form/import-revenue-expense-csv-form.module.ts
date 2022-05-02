import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImportRevenueExpenseCsvFormComponent } from './import-revenue-expense-csv-form/import-revenue-expense-csv-form.component';
import { FuseSharedModule } from '@fuse/shared.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';



@NgModule({

  imports: [

    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCardModule,


  ],
  declarations: [
    ImportRevenueExpenseCsvFormComponent
  ],
  exports: [
    ImportRevenueExpenseCsvFormComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class ImportRevenueExpenseCsvFormModule { }
