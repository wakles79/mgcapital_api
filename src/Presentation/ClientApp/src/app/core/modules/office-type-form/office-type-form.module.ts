import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OfficeTypeFormComponent } from './office-type-form.component';
import { FuseSharedModule } from '@fuse/shared.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatInputModule } from '@angular/material/input';



@NgModule({
  imports: [
    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatSelectModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatInputModule
  ],
  declarations: [
    OfficeTypeFormComponent
  ],
  exports: [
    OfficeTypeFormComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]

})
export class OfficeTypeFormModule { }
