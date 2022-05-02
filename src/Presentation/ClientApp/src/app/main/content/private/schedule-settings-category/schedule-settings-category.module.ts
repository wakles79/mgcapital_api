import { NgModule } from '@angular/core';
import { ScheduleSettingsCategoryComponent } from './schedule-settings-category.component';
import { ScheduleSettingsCategoryFormComponent } from './schedule-settings-category-form/schedule-settings-category-form.component';
import { ScheduleSettingsCategoryListComponent } from './schedule-settings-category-list/schedule-settings-category-list.component';
import { ScheduleSettingsSubcategoryFormComponent } from './schedule-settings-subcategory-form/schedule-settings-subcategory-form.component';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleSettingsCategoryService } from './schedule-settings-category.service';
import { ScheduleSettingsCategoryResolver } from './schedule-settings-category.resolver';

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
import {MatCardModule} from '@angular/material/card';
import {MatDividerModule} from '@angular/material/divider';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatListModule} from '@angular/material/list';
import {MatTooltipModule} from '@angular/material/tooltip';
import { FuseSharedModule } from '@fuse/shared.module';

const routes: Routes = [
  {
    path: '**',
    component: ScheduleSettingsCategoryComponent,
    resolve: {
      resolver: ScheduleSettingsCategoryResolver
    }
  }
];

@NgModule({
  imports: [
    FuseSharedModule,
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
    MatCardModule,
    MatDividerModule,
    MatCheckboxModule,
    MatExpansionModule,
    MatListModule,
    MatTooltipModule
  ],
  declarations: [
    ScheduleSettingsCategoryComponent,
    ScheduleSettingsCategoryFormComponent,
    ScheduleSettingsCategoryListComponent,
    ScheduleSettingsSubcategoryFormComponent],
  providers: [
    ScheduleSettingsCategoryService,
    ScheduleSettingsCategoryResolver
  ],
  entryComponents: [
    ScheduleSettingsCategoryFormComponent,
    ScheduleSettingsSubcategoryFormComponent
  ]

})
export class ScheduleSettingsCategoryModule { }
