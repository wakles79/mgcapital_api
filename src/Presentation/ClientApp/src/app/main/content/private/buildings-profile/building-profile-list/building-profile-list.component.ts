import { Component, OnInit, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { fuseAnimations } from '@fuse/animations';
import { merge } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { BuildingsDataSource } from '../../buildings/building-list/building-list.component';
import { BuildingsService } from '../../buildings/buildings.service';

@Component({
  selector: 'app-building-profile-list',
  templateUrl: './building-profile-list.component.html',
  styleUrls: ['./building-profile-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None

})
export class BuildingProfileListComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSource: BuildingsDataSource | null;

  displayedColumns = ['name', 'fullAddress', 'operationsManagerFullName', 'emergencyPhone', 'isComplete', 'isActive'];

  loading$ = this.buildingService.loadingSubject.asObservable();

  get buildingsCount(): number { return this.buildingService.elementsCount; }
  searchInput: FormControl;
  constructor(
    private buildingService: BuildingsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
  ) {
    this.searchInput = new FormControl(this.buildingService.searchText);
  }

  ngOnInit(): void {
    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.buildingService.searchTextChanged.next(searchText);
      });
    this.dataSource = new BuildingsDataSource(this.buildingService);
  }

  // tslint:disable-next-line: use-life-cycle-interface
  ngAfterViewInit(): void {
    // reset the paginator after sorting
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.buildingService.getElements(
          'ReadAllBuildingProfile', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize))
      )
      .subscribe();
  }

  activeStatus(value): string {
    return value ? 'Active' : 'Not Active';
  }

  completeStatus(value): string {
    return value ? 'Yes' : 'No';
  }

  availableStatus(value): string {
    return value ? 'Yes' : 'No';
  }

}
