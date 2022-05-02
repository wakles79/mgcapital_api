import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { FeatureFlagService } from '@app/core/services/feature-flag/feature-flag.service';

@Directive({
  selector: '[mgcapRemoveIfFeatureOn]'
})
export class RemoveIfFeatureOnDirective implements OnInit {
  private hasView = false;
  private featureName: string;


  @Input() set mgcapRemoveIfFeatureOn(featureName: string) {
    this.featureName = featureName;
    this.toggle();
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private featureFlagService: FeatureFlagService) {
  }

  private toggle(): void {
    if (!this.flag && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (this.flag && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  get flag(): boolean {
    return this.featureFlagService.featureOn(this.featureName);
  }

  ngOnInit(): void {
    this.featureFlagService.featureFlags$.subscribe((flags: any) => {
      this.toggle();
    });
  }
}
