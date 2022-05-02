import { Component, OnInit } from '@angular/core';
import { FuseConfigService } from '@fuse/services/config.service';


@Component({
  selector: 'fuse-error-404',
  templateUrl: './error-404.component.html',
  styleUrls: ['./error-404.component.scss']
})
export class FuseError404Component implements OnInit {
  constructor(
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
}
