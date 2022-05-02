import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fuseAnimations } from '@fuse/animations';
import { TagFormComponent } from './tag-form/tag-form.component';
import { TagsService } from './tags.service';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class TagsComponent implements OnInit {

  // dialog
  tagFormDialog: MatDialogRef<TagFormComponent>;

  constructor(
    private _tagService: TagsService,
    private _snackBar: MatSnackBar,
    private _dialog: MatDialog
  ) { }

  ngOnInit(): void {
  }

  // Buttons
  newTag(): void {
    this.tagFormDialog = this._dialog.open(TagFormComponent, {
      panelClass: 'tag-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.tagFormDialog.afterClosed()
      .subscribe((form: FormGroup) => {
        if (!form) {
          return;
        }

        this._tagService.loadingSubject.next(true);
        this._tagService.createElement(form.getRawValue())
          .then(response => {
            this._tagService.loadingSubject.next(false);
            this._snackBar.open('Tag created successfully!!!', 'close', { duration: 1000 });
          }).catch(error => {
            this._tagService.loadingSubject.next(false);
            this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });
  }

}
