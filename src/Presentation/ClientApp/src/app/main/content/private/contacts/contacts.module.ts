import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContactsComponent } from './contacts.component';
import { ContactsResolver } from './contacts-resolver';
import { RouterModule, Routes } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { ContactListComponent } from './contact-list/contact-list.component';
import { ContactsSelectedBarComponent } from './selected-bar/selected-bar.component';
import { ContactsMainComponent } from './sidenavs/main/main.component';
import { ContactFormModule } from '@app/core/modules/contact-form/contact-form.module';
import { ContactFormComponent } from '@app/core/modules/contact-form/contact-form/contact-form.component';
import { PhoneFormComponent } from '@app/core/modules/contact-form/phone-form/phone-form.component';
import { EmailFormComponent } from '@app/core/modules/contact-form/email-form/email-form.component';
import { AddressFormComponent } from '@app/core/modules/contact-form/address-form/address-form.component';
import { DeleteConfirmDialogModule } from '@app/core/modules/delete-confirm-dialog/delete-confirm-dialog.module';
import { MultipleDeleteConfirmDialogComponent } from '@app/core/modules/delete-confirm-dialog/multiple-delete-confirm-dialog/multiple-delete-confirm-dialog.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';

const routes: Routes = [
  {
    path: '**',
    component: ContactsComponent,
    resolve: {
      resolver: ContactsResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatTableModule,
    CdkTableModule,
    MatCheckboxModule,
    MatMenuModule,
    MatPaginatorModule,
    MatSortModule,

    // App shared modules
    ContactFormModule,
    DeleteConfirmDialogModule


  ],
  declarations: [
    ContactsComponent,
    ContactListComponent,
    ContactsSelectedBarComponent,
    ContactsMainComponent,
  ],
  providers: [

  ],
  entryComponents: [
    ContactFormComponent,
    PhoneFormComponent,
    EmailFormComponent,
    AddressFormComponent,
    MultipleDeleteConfirmDialogComponent
  ]
})
export class ContactsModule { }
