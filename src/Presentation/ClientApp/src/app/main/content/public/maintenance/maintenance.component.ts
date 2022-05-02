import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';


@Component({
  selector: 'fuse-maintenance',
  templateUrl: './maintenance.component.html',
  styleUrls: ['./maintenance.component.scss'],
  animations: fuseAnimations
})
export class FuseMaintenanceComponent implements OnInit {
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
    this.location.back();
  }
}
