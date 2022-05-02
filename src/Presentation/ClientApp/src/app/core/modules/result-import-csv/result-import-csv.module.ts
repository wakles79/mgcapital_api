import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResultImportCsvComponent } from './result-import-csv/result-import-csv.component';
import { FuseSharedModule } from '@fuse/shared.module';

// Material
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';



@NgModule({
  imports: [

    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatCardModule,
    MatDialogModule,
    MatInputModule

  ],
  declarations: [
    ResultImportCsvComponent
  ],
  exports: [
    ResultImportCsvComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class ResultImportCsvModule { }
