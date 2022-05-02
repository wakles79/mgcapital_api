import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { Subject } from 'rxjs';
import { BuildingsService } from '../../../buildings/buildings.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: string } = {};

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    private buildingService: BuildingsService,
    private authService: AuthService,
    public snackBar: MatSnackBar,
  ) {
    this.filterActive = 'all';
    this.filterBy = {};
  }

  ngOnInit(): void {
  }

  // tslint:disable-next-line: use-life-cycle-interface
  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  changeFilter(filter: any): void {
    if (filter === 'all') {
      this.filterActive = 'all';
      this.filterBy = {};
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'active') {
      this.filterActive = 'active';
      this.filterBy = { 'isActive': '1' };
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'inactive') {
      this.filterActive = 'inactive';
      this.filterBy = { 'isActive': '0' };
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

    if (filter === 'notAvailable') {
      this.filterActive = 'notAvailable';
      this.filterBy = { 'isAvailable': '0', 'isActive': '' };
      this.buildingService.onFilterChanged.next(this.filterBy);
    }

  }

}
