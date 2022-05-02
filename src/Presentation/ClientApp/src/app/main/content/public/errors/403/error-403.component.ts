import { Component, OnInit } from '@angular/core';
import { FuseConfigService } from '@fuse/services/config.service';
import { Location } from '@angular/common';


@Component({
  selector: 'fuse-error-403',
  templateUrl: './error-403.component.html',
  styleUrls: ['./error-403.component.scss']
})
export class FuseError403Component implements OnInit {
  constructor(
    private location: Location,
    private fuseConfig: FuseConfigService
  ) {
    this.fuseConfig.config = {
      layout: {
        navbar: {
          hidden: true
        },
        toolbar: {
          hidden: true
        },
        footer: {
          hidden: true
        },
        sidepanel: {
          hidden: true
        }
      }
    };
  }

  ngOnInit(): void {
  }

  goBack(): void {
    return this.location.back();
  }
}
