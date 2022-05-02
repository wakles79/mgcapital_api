import { NgModule } from '@angular/core';
import { RemoveIfFeatureOffDirective } from '@app/core/directives/feature-flag/remove-if-feature-off.directive';
import { RemoveIfFeatureOnDirective } from '@app/core/directives/feature-flag/remove-if-feature-on.directive';


@NgModule({
  declarations: [
    RemoveIfFeatureOffDirective,
    RemoveIfFeatureOnDirective,
  ],
  exports: [
    RemoveIfFeatureOffDirective,
    RemoveIfFeatureOnDirective,
  ]
})
export class FeatureFlagModule { }
