import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FuseSelectCompanyComponent } from '@app/main/content/public/authentication/select-company/select-company.component';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { FuseSharedModule } from '@fuse/shared.module';


const routes = [
  {
    path: 'auth/select-company',
    component: FuseSelectCompanyComponent
  }
];

@NgModule({
  declarations: [
    FuseSelectCompanyComponent
  ],
  imports: [
    // Material Modules
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatButtonModule,
    FuseSharedModule,
    RouterModule.forChild(routes)
  ]
})

export class SelectCompanyModule {

}
