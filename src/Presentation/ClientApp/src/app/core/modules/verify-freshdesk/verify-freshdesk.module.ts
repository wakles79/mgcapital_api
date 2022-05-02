import { NgModule } from '@angular/core';

import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { VerifyFreshdeskComponent } from './verify-freshdesk/verify-freshdesk.component';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FuseSharedModule } from '@fuse/shared.module';

@NgModule({
  imports: [
    FuseSharedModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatInputModule
  ],
  declarations: [
   VerifyFreshdeskComponent
  ],
  exports: [
   VerifyFreshdeskComponent
  ],
  entryComponents: [

  ],
  providers: [

  ]
})
export class VerifyFreshdeskModule { }
