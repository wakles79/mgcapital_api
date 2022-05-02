import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ElementSelectorFilterComponent } from './element-selector-filter/element-selector-filter.component';
import { FuseSharedModule } from '@fuse/shared.module';

import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { PipesModule } from '@app/core/pipes/pipes.module';

@NgModule({

  imports: [

    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    NgxMatSelectSearchModule,

    // App pipes
    PipesModule,
  ],
  declarations: [
    ElementSelectorFilterComponent

  ],
  exports: [
    ElementSelectorFilterComponent

  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class ElementSelectorFilterModule { }
