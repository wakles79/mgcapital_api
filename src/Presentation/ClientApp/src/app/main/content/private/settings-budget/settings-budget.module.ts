import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SettingsBudgetComponent } from './settings-budget.component';
import { FuseSharedModule } from '@fuse/shared.module';
import { RouterModule, Routes } from '@angular/router';
import { SettingsBudgetResolver } from './settings-budget-resolver';

// Material
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';

const routes: Routes = [
  {
    path: '**',
    component: SettingsBudgetComponent,
    resolve: {
      resolver: SettingsBudgetResolver
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
    MatCardModule,
    MatFormFieldModule
  ],
  declarations: [
    SettingsBudgetComponent
  ],
  providers: [

  ],
  entryComponents: [

  ]

})
export class SettingsBudgetModule { }
