import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServicesComponent } from './services.component';
import { ServiceListComponent } from './service-list/service-list.component';
import { ServiceFormComponent } from './service-form/service-form.component';
import { MainComponent } from './sidenavs/main/main.component';
import { Routes, RouterModule } from '@angular/router';
import { ServiceResolver } from './services-resolver';
import { FuseSharedModule } from '@fuse/shared.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { MatMenuModule } from '@angular/material/menu';
import { CdkTableModule } from '@angular/cdk/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSortModule } from '@angular/material/sort';
import { MatRippleModule } from '@angular/material/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

const routes: Routes = [
  {
    path: '**',
    component: ServicesComponent,
    resolve: {
      resolver: ServiceResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatMenuModule,
    MatPaginatorModule,
    MatInputModule,
    MatDialogModule,
    MatRippleModule,
    MatToolbarModule,
    MatTooltipModule
  ],
  declarations: [
    ServicesComponent,
    ServiceListComponent,
    ServiceFormComponent,
    MainComponent
  ],
  providers: [

  ],
  entryComponents: [
    ServiceFormComponent
  ]

})
export class ServicesModule { }
