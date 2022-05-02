import { NgModule } from '@angular/core';
import { UsersComponent } from './users.component';
import { SelectedBarComponent } from './selected-bar/selected-bar.component';
import { ShareBuildingDialogComponent } from './share-building-dialog/share-building-dialog.component';
import { MainComponent } from './sidenavs/main/main.component';
import { UserFormComponent } from './user-form/user-form.component';
import { UserListComponent } from './user-list/user-list.component';
import { RouterModule, Routes } from '@angular/router';
import { UsersResolver } from './users-resolver';

import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatMenuModule } from '@angular/material/menu';
import { FuseSidebarModule } from '@fuse/components';
import { ReactiveFormsModule } from '@angular/forms';
import { FuseSharedModule } from '@fuse/shared.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTooltipModule } from '@angular/material/tooltip';

import { PhoneFormComponent } from '@app/core/modules/contact-form/phone-form/phone-form.component';
import { EmailFormComponent } from '@app/core/modules/contact-form/email-form/email-form.component';
import { AddressFormComponent } from '@app/core/modules/contact-form/address-form/address-form.component';
import { MultipleDeleteConfirmDialogComponent } from '@app/core/modules/delete-confirm-dialog/multiple-delete-confirm-dialog/multiple-delete-confirm-dialog.component';
import { VerifyFreshdeskComponent } from '@app/core/modules/verify-freshdesk/verify-freshdesk/verify-freshdesk.component';
import { ContactFormModule } from '@app/core/modules/contact-form/contact-form.module';
import { ConfirmDialogModule } from '@app/core/modules/confirm-dialog/confirm-dialog.module';
import { DeleteConfirmDialogModule } from '@app/core/modules/delete-confirm-dialog/delete-confirm-dialog.module';
import { VerifyFreshdeskModule } from '@app/core/modules/verify-freshdesk/verify-freshdesk.module';
import { EditorModule } from '@progress/kendo-angular-editor';


const routes: Routes = [
  {
    path: '**',
    component: UsersComponent,
    resolve: {
      resolver: UsersResolver
    }

  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),

    EditorModule,

    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatSnackBarModule,
    MatDialogModule,
    MatToolbarModule,
    MatTableModule,
    CdkTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatMenuModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatSidenavModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    MatRippleModule,
    MatExpansionModule,
    MatListModule,
    MatSlideToggleModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatTooltipModule,
    // App modules import
    ContactFormModule,
    ConfirmDialogModule,
    DeleteConfirmDialogModule,
    VerifyFreshdeskModule
  ],
  declarations: [
    UsersComponent,
    SelectedBarComponent,
    ShareBuildingDialogComponent,
    MainComponent,
    UserFormComponent,
    UserListComponent
  ],
  providers: [
    // UsersResolver,
    // UsersService,
    // DepartmentsService,
    // ContactsService,
    // AuthService,
    // BuildingService,
    // SettingsService
  ],
  entryComponents: [
    UserFormComponent,
    PhoneFormComponent,
    EmailFormComponent,
    AddressFormComponent,
    MultipleDeleteConfirmDialogComponent,
    ShareBuildingDialogComponent,
    VerifyFreshdeskComponent
  ]

})
export class UsersModule { }
