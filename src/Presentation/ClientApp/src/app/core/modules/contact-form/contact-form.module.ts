import { NgModule } from '@angular/core';

import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatDatepickerModule } from '@angular/material/datepicker';

import { PhoneFormComponent } from './phone-form/phone-form.component';
import { EmailFormComponent } from './email-form/email-form.component';
import { AddressFormComponent } from './address-form/address-form.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { ContactFormComponent } from './contact-form/contact-form.component';
import { SearchContactFormComponent } from './search-contact-form/search-contact-form.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';


@NgModule({
  imports: [

   FuseSharedModule,
   MatCheckboxModule,
   MatIconModule,
   MatProgressSpinnerModule,
   MatButtonModule,
   MatSnackBarModule,
   MatDialogModule,
   MatToolbarModule,
   MatMenuModule,
   MatFormFieldModule,
   MatInputModule,
   MatSelectModule,
   NgxMatSelectSearchModule,
   MatDatepickerModule,
   MatSlideToggleModule,
   MatNativeDateModule,
   MatTooltipModule,
   MatExpansionModule,
   MatListModule,
   MatRippleModule


  ],
  declarations: [
    PhoneFormComponent,
    EmailFormComponent,
    AddressFormComponent,
    ContactFormComponent,
    SearchContactFormComponent
  ],
  exports: [
    PhoneFormComponent,
    EmailFormComponent,
    AddressFormComponent,
    ContactFormComponent,
    SearchContactFormComponent

  ],
  entryComponents: [

  ],
  providers: [

  ]

})
export class ContactFormModule { }
