import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { fuseAnimations } from '@fuse/animations';
import { UsersService } from '../../users.service';

@Component({
  selector: 'app-main-user',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss'],
  animations: fuseAnimations
})
export class MainComponent implements OnInit, OnDestroy {

  user: any;
  filterActive: string;
  filterBy: { [key: string]: string } = {};
  roles: any[] = [];

  constructor(private usersService: UsersService) {
    this.filterActive = 'all';
  }

  ngOnInit(): void {
    this.getRoles();
  }
  ngOnDestroy(): void {
  }

  changeFilter(filter): void {
    this.filterActive = filter;
    this.filterBy = { 'roleId': filter };
    this.usersService.onFilterChanged.next(this.filterBy);
  }

  getRoles(): void {
    this.usersService.getAllAsList('readallcboroles', '', 0, 20, null)
      .subscribe((response: { count: number, payload: any[] }) => {
        this.roles = response.payload;
      }
      );
  }

}
