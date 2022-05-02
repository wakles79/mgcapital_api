import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingsComponent } from './settings.component';
import { Resolve, RouterModule, Routes } from '@angular/router';
import { SettingsResolver } from './settings-resolver';
import { FuseSharedModule } from '@fuse/shared.module';
import { FeatureFlagModule } from '@app/core/modules/feature-flag.module';
import { VerifyFreshdeskModule } from '@app/core/modules/verify-freshdesk/verify-freshdesk.module';
import { VerifyFreshdeskComponent } from '@app/core/modules/verify-freshdesk/verify-freshdesk/verify-freshdesk.component';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { EditorModule } from '@progress/kendo-angular-editor';

const routes: Routes = [
  {
     path: '**',
     component: SettingsComponent,
     resolve: {
       resolver: SettingsResolver
     }
  }
];

@NgModule({
    imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),

    EditorModule,

    FeatureFlagModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSidenavModule,
    MatCardModule,
    MatInputModule,
    MatFormFieldModule,
    MatSlideToggleModule,
    MatCheckboxModule,

    // Add shared modules
    VerifyFreshdeskModule

  ],
  declarations: [
    SettingsComponent
  ],
  providers: [

  ],
  entryComponents: [
    VerifyFreshdeskComponent
  ]

})
export class SettingsModule { }
