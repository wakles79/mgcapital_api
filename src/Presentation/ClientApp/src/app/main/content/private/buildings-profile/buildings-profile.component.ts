import { Component, OnInit } from '@angular/core';
import { FuseSidebarService } from '@fuse/components/sidebar/sidebar.service';

@Component({
  selector: 'app-buildings-profile',
  templateUrl: './buildings-profile.component.html',
  styleUrls: ['./buildings-profile.component.scss']
})
export class BuildingsProfileComponent implements OnInit {

  constructor(private _fuseSidebarService: FuseSidebarService
  ) { }

  ngOnInit(): void {
  }

/**
 * Toggle sidebar
 *
 * @param name
 */
  toggleSidebar(name): void {
    this._fuseSidebarService.getSidebar(name).toggleOpen();
  }

}
