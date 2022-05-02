import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { HomeComponent } from './home.component';
import { FuseSharedModule } from '@fuse/shared.module';

const routes = [
  {
    path: '**',
    component: HomeComponent
  }
];

@NgModule({
  declarations: [
    HomeComponent
  ],
  providers: [
  ],
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    HomeComponent
  ]
})

export class HomeModule {
}
