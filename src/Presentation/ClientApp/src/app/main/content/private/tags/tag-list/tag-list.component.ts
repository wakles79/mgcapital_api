import { DataSource } from '@angular/cdk/table';
import { AfterViewInit, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { TagBaseModel } from '@app/core/models/tag/tag-base.model';
import { TagGridModel } from '@app/core/models/tag/tag-grid.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { merge, Observable } from 'rxjs';
import { debounceTime, tap } from 'rxjs/operators';
import { TagFormComponent } from '../tag-form/tag-form.component';
import { TagsService } from '../tags.service';

@Component({
  selector: 'app-tag-list',
  templateUrl: './tag-list.component.html',
  styleUrls: ['./tag-list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: fuseAnimations
})
export class TagListComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  loading$ = this._tagService.loadingSubject.asObservable();

  searchInput: FormControl;

  tags: TagGridModel[] = [];
  dataSource: TagDataSource | null;
  columnsToDisplay = ['color', 'description', 'buttons'];
  get getTagCount(): number { return this._tagService.elementsCount; }

  // dialog
  tagFormDialog: MatDialogRef<TagFormComponent>;
  confirmDialog: MatDialogRef<FuseConfirmDialogComponent>;

  constructor(
    private _tagService: TagsService,
    private _snackBar: MatSnackBar,
    private _dialog: MatDialog
  ) {
    this.dataSource = new TagDataSource(this._tagService);
    this.searchInput = new FormControl(this._tagService.searchText);
  }

  ngOnInit(): void {

    try {
      this.searchInput.valueChanges
        .pipe(debounceTime(300))
        .subscribe(searchText => {
          this.paginator.pageIndex = 0;
          this._tagService.searchTextChanged.next(searchText);
        });
    } catch (e) { console.log(e); }

  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this._tagService
          .getElements(
            'readall',
            '',
            this.sort.active,
            this.sort.direction,
            this.paginator.pageIndex,
            this.paginator.pageSize))
      )
      .subscribe();
  }

  // Buttons
  editTag(id: number): void {
    this._tagService.loadingSubject.next(true);
    this._tagService.get(id).subscribe((response) => {

      this._tagService.loadingSubject.next(false);
      if (!response) {
        this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        return;
      }

      const tag = new TagBaseModel(response);
      this.tagFormDialog = this._dialog.open(TagFormComponent, {
        panelClass: 'tag-form-dialog',
        data: {
          action: 'edit',
          tag: tag
        }
      });

      this.tagFormDialog.afterClosed()
        .subscribe((form: FormGroup) => {
          if (!form) {
            return;
          }

          this._tagService.loadingSubject.next(true);
          this._tagService.updateElement(form.getRawValue())
            .then(result => {
              this._tagService.loadingSubject.next(false);
              this._snackBar.open('Tag updated successfully!!!', 'close', { duration: 1000 });
            }).catch(error => {
              this._tagService.loadingSubject.next(false);
              this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            });
        });
    }, (error) => {
      this._tagService.loadingSubject.next(false);
      this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
    });
  }

  deleteTag(id: number, count: number): void {

    if (count > 0) {
      this._snackBar.open('Oops,  Cannot be deleted', 'close', { duration: 3000 });
      return;
    }

    this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });
    this.confirmDialog.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialog.afterClosed().subscribe(result => {
      if (result) {
        this._tagService.loadingSubject.next(true);
        this._tagService.deleteTag(id)
          .subscribe(
            () => {
              this._snackBar.open('Tag delete successfully!!!', 'close', { duration: 3000 });
              this._tagService.getElements();
            },
            () => {
              this._tagService.loadingSubject.next(false);
              this._snackBar.open('Oops, Cannot be deleted', 'close', { duration: 3000 });
            });
      }
      this.confirmDialog = null;
    });
  }
}

export class TagDataSource extends DataSource<any>{
  constructor(private _tagService: TagsService) {
    super();
  }

  connect(): Observable<any[]> {
    return this._tagService.allElementsChanged;
  }

  disconnect(): void {

  }
}
