import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ScheduleCategoryBaseModel } from '@app/core/models/schedule-category/schedule-category-base.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ScheduleSettingsCategoryFormComponent } from '../schedule-settings-category-form/schedule-settings-category-form.component';
import { ScheduleSettingsCategoryService } from '../schedule-settings-category.service';


@Component({
  selector: 'app-schedule-settings-category-list',
  templateUrl: './schedule-settings-category-list.component.html',
  styleUrls: ['./schedule-settings-category-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ScheduleSettingsCategoryListComponent implements OnInit, AfterViewInit {
  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @Input() readOnly: boolean;
  loading$ = this.schedueSettingsCategoryService.loadingSubject.asObservable();

  dialogRef: any;

  searchInput: FormControl;
  dataSource: ScheduleCategoryDataSource | null;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  displayedColumns = ['description', 'subcategories', 'status', 'buttons'];
  get listCount(): number { return this.schedueSettingsCategoryService.elementsCount; }

  constructor(
    private schedueSettingsCategoryService: ScheduleSettingsCategoryService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.searchInput = new FormControl(this.schedueSettingsCategoryService.searchText);

  }

  ngOnInit(): void {
    this.dataSource = new ScheduleCategoryDataSource(this.schedueSettingsCategoryService);
    // console.log(this.dataSource);
    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      ).subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.schedueSettingsCategoryService.searchTextChanged.next(searchText);
      });
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.schedueSettingsCategoryService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();
  }

  changeStatus(objId: any): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to change the status?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.schedueSettingsCategoryService.delete(objId);
      }
      this.confirmDialogRef = null;
    });
  }

  edit(objId: any): void {
    this.schedueSettingsCategoryService.get(objId)
      .subscribe(
        (objData: any) => {
          if (!objData) {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            return;
          }

          const objSchedule = new ScheduleCategoryBaseModel(objData);
          this.dialogRef = this.dialog.open(ScheduleSettingsCategoryFormComponent, {
            panelClass: 'Schedule-Settings-form-dialog',
            data: {
              objSchedule: objSchedule,
              action: 'edit'
            }
          });

          this.dialogRef.afterClosed()
            .subscribe((response: string) => {
              if (!response) {
                return;
              }

              if (response === 'success') {
                this.snackBar.open('Expense type updated successfully!!!', 'close', { duration: 1000 });
              } else {
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              }
            });
        },
        (error) => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
      );
  }

  activeOptions(status: boolean): any {
    return status ? 'Disable' : 'Enable';
  }

  activeStatus(status: boolean): any {
    return status ? 'Active' : 'Inactive';
  }
}

export class ScheduleCategoryDataSource extends DataSource<any>{
  constructor(private schedueCategoryService: ScheduleSettingsCategoryService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.schedueCategoryService.allElementsChanged;
  }

  disconnect(): void {
  }
}
