import { NgModule } from '@angular/core';
import { BuildingsComponent } from './buildings.component';
import { MainComponent } from './sidenavs/main/main.component';
import { BuildingListComponent } from './building-list/building-list.component';
import { BuildingActivityLogDialogComponent } from './building-activity-log-dialog/building-activity-log-dialog.component';
import { RouterModule, Routes } from '@angular/router';

import { FuseSharedModule } from '@fuse/shared.module';
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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { PipesModule } from '../../../../core/pipes/pipes.module';

import { ContactFormModule } from '@app/core/modules/contact-form/contact-form.module';
import { PhoneFormComponent } from '@app/core/modules/contact-form/phone-form/phone-form.component';
import { EmailFormComponent } from '@app/core/modules/contact-form/email-form/email-form.component';
import { AddressFormComponent } from '@app/core/modules/contact-form/address-form/address-form.component';
import { ContactFormComponent } from '@app/core/modules/contact-form/contact-form/contact-form.component';
import { BuildingFormComponent } from '@app/core/modules/building-form/building-form/building-form.component';
import { BuildingFormModule } from '../../../../core/modules/building-form/building-form.module';
import { BuildingsResolver } from './buildings-resolver';
import { SearchContactFormComponent } from '@app/core/modules/contact-form/search-contact-form/search-contact-form.component';
import { CustomerFormComponent } from '@app/core/modules/customer-form/customer-form.component';
import { CustomerFormModule } from '../../../../core/modules/customer-form/customer-form.module';

const routes: Routes = [
  {
    path: '**',
    component: BuildingsComponent,
    resolve: {
      resolver: BuildingsResolver
    }

  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    FuseSidebarModule,
    RouterModule.forChild(routes),

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
    MatCardModule,

    // App modules import
    ContactFormModule,
    BuildingFormModule,
    CustomerFormModule,

    // App pipes
     PipesModule
  ],
  declarations: [
    BuildingsComponent,
    MainComponent,
    BuildingListComponent,
    BuildingActivityLogDialogComponent
  ],
  providers: [

  ],
  entryComponents: [
    BuildingFormComponent,
    ContactFormComponent,
    CustomerFormComponent,
    SearchContactFormComponent,
    PhoneFormComponent,
    EmailFormComponent,
    AddressFormComponent,
    BuildingActivityLogDialogComponent
  ]

})
export class BuildingsModule { }
