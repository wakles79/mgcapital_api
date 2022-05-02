import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { FeatureFlagService } from '@app/core/services/feature-flag/feature-flag.service';

@Directive({
  selector: '[mgcapRemoveIfFeatureOff]'
})
export class RemoveIfFeatureOffDirective implements OnInit {
  private hasView = false;
  private featureName: string;


  @Input() set mgcapRemoveIfFeatureOff(featureName: string) {
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
    return this.featureFlagService.featureOff(this.featureName);
  }

  ngOnInit(): void {
    this.featureFlagService.featureFlags$.subscribe((flags: any) => {
      this.toggle();
    });
  }
}
