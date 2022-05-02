import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TagFormComponent } from './tag-form/tag-form.component';
import { TagsComponent } from './tags.component';
import { TagListComponent } from './tag-list/tag-list.component';
import { TagMainComponent } from './sidenavs/tag-main/tag-main.component';
import { RouterModule, Routes } from '@angular/router';
import { TagsService } from './tags.service';
import { TagsResolver } from './tags-resolver';

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
import { FuseSharedModule } from '@fuse/shared.module';

const routes: Routes = [
  {
    path: '**',
    component: TagsComponent,
    resolve: {
      tags: TagsResolver
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
  ],
  declarations: [
    TagFormComponent,
    TagsComponent,
    TagListComponent,
    TagMainComponent,
  ],
  providers: [
    TagsService,
    TagsResolver
  ],
  entryComponents: [
    TagFormComponent
  ]
})
export class TagsModule { }
