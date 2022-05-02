import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { PublicProposalDetailComponent } from './public-proposal-detail.component';
import { ApprovedProposalConfirmDialogComponent } from './approved-proposal-confirm-dialog/approved-proposal-confirm-dialog.component';
import { RouterModule, Routes } from '@angular/router';
import { PublicProposalDetailResolver } from './public-proposal-detail-resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { LightboxModule } from 'ngx-lightbox';
import { PipesModule } from '@app/core/pipes/pipes.module';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { CdkTableModule } from '@angular/cdk/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatRadioModule } from '@angular/material/radio';
import { MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';

const routes: Routes = [
  {
    path: 'proposals/proposal-detail/:guid',
    component: PublicProposalDetailComponent,
    resolve: {
      resolver: PublicProposalDetailResolver
    }
  }
];

@NgModule({
  imports: [

    FuseSharedModule,
    RouterModule.forChild(routes),
    LightboxModule,

    MatIconModule,
    MatButtonModule,
    MatTableModule,
    CdkTableModule,
    MatSortModule,
    MatFormFieldModule,
    MatSlideToggleModule,
    MatRadioModule,
    MatInputModule,
    MatDialogModule,
    MatProgressBarModule,
    MatCardModule,
    MatToolbarModule,
    MatTooltipModule,

    // Pipes module
    PipesModule

  ],
  declarations: [
    PublicProposalDetailComponent,
    ApprovedProposalConfirmDialogComponent
  ],
  providers: [
  ],
  entryComponents: [
    ApprovedProposalConfirmDialogComponent
  ]

})
export class PublicProposalDetailModule { }
