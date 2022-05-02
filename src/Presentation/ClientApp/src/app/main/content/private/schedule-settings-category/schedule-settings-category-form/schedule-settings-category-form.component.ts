import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ScheduleCategoryBaseModel } from '@app/core/models/schedule-category/schedule-category-base.model';
import { fuseAnimations } from '@fuse/animations';
import { ScheduleSubcategoryBaseModel } from '@app/core/models/schedule-subcategory/schedule-subcategory-base.model';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScheduleSettingsCategoryService } from '../schedule-settings-category.service';
import { ScheduleSettingsSubcategoryFormComponent } from '../schedule-settings-subcategory-form/schedule-settings-subcategory-form.component';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-schedule-settings-category-form',
  templateUrl: './schedule-settings-category-form.component.html',
  styleUrls: ['./schedule-settings-category-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ScheduleSettingsCategoryFormComponent implements OnInit {

  subCategoryDialog: any;
  schedule: ScheduleCategoryBaseModel;
  scheduleFormGroup: FormGroup;

  dialogTitle: string;
  action: string;

  loading$ = this.scheduleSettingsCategoryService.loadingSubject.asObservable();

  categories: any[] = [
    { 'id': 0, 'name': 'Tile' },
    { 'id': 1, 'name': 'Stone' },
    { 'id': 2, 'name': 'Carpet' },
    { 'id': 3, 'name': 'PowerWash' },
    { 'id': 4, 'name': 'Other/Misc' }
  ];
  color: any[] = [
    { 'id': 0, 'color': 'Red' },
    { 'id': 1, 'color': 'Green' },
    { 'id': 2, 'color': 'Blue' },
    { 'id': 3, 'color': 'Yellow' },
    { 'id': 4, 'color': 'Purple' },
    { 'id': 5, 'color': 'Maroon' },
    { 'id': 6, 'color': 'Orange' },
    { 'id': 7, 'color': 'Pink' },
    { 'id': 8, 'color': 'Gray' }
  ];

  totalSubcategories: number;
  subcategories: ScheduleSubcategoryBaseModel[] = [];

  get readOnly(): any {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<ScheduleSettingsCategoryFormComponent>,
    private scheduleSettingsCategoryService: ScheduleSettingsCategoryService,
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Schedule Setting';
      this.totalSubcategories = 1;
      this.scheduleFormGroup = this.createForm();
    } else if (this.action === 'edit') {
      this.dialogTitle = 'Update Schedule Setting ';
      this.schedule = data.objSchedule;
      this.getSubcategories(this.schedule.id);
      this.scheduleFormGroup = this.updateForm();
    }
   }

  ngOnInit(): void {
  }

  createForm(): any {
    return this.formBuilder.group({
      scheduleCategoryType: [0],
      description: ['', Validators.required],
      status: [1],
      color: ['', [Validators.required]]
    });
  }

  updateForm(): any {
    return this.formBuilder.group({
      id: [this.schedule.id],
      scheduleCategoryType: [0],
      description: [{ value: this.schedule.description, disabled: this.readOnly }, Validators.required],
      status: [{ value: this.schedule.status, disabled: this.readOnly }],
      color: [{ value: this.schedule.color, disabled: this.readOnly }, Validators.required]
    });
  }

  getSubcategories(id: number): void {
    this.scheduleSettingsCategoryService.getAllSubcategories(id)
      .subscribe(
        (response: { count: number, payload: ScheduleSubcategoryBaseModel[] }) => {
          this.subcategories = response.payload;
        }, (error) => {
          this.snackBar.open('Oops, there was an error getting subcategories', 'close', { duration: 1000 });
        });
  }

  newSubcategory(): void {
    if (this.action === 'new') {
      this.subCategoryDialog = this.dialog.open(ScheduleSettingsSubcategoryFormComponent, {
        panelClass: 'Schedule-Subcategory-form-dialog',
        data: {
          action: 'new'
        }
      });

      this.subCategoryDialog.afterClosed()
        .subscribe((response: FormGroup) => {
          response.addControl('id', new FormControl(this.totalSubcategories));
          this.totalSubcategories++;
          this.subcategories.push(response.getRawValue());
        });
    } else if (this.action === 'edit') {
      this.subCategoryDialog = this.dialog.open(ScheduleSettingsSubcategoryFormComponent, {
        panelClass: 'Schedule-Subcategory-form-dialog',
        data: {
          action: 'new'
        }
      });

      this.subCategoryDialog.afterClosed()
        .subscribe((response: FormGroup) => {
          if (!response) {
            return;
          }

          const scheduleSubCategoryUpdated = new ScheduleSubcategoryBaseModel(response.getRawValue());
          scheduleSubCategoryUpdated.scheduleSettingCategoryId = this.scheduleFormGroup.controls['id'].value;
          this.scheduleSettingsCategoryService.saveSubcategory(scheduleSubCategoryUpdated);
          response.addControl('id', new FormControl(this.totalSubcategories));
          this.totalSubcategories++;
          this.subcategories.push(response.getRawValue());
          this.scheduleSettingsCategoryService.saveSubcategory(scheduleSubCategoryUpdated)
            .subscribe(
              () => {
                this.snackBar.open('Expense subcategory added successfully!!!', 'close', { duration: 1000 });
                this.getSubcategories(this.scheduleFormGroup.controls['id'].value);
              },
              (error) => this.snackBar.open('Oops, there was an error.', 'close', { duration: 1000 })
            );
        });
    }

  }

  updateSubcategory(id: number): void {

    if (this.readOnly) {
      this.snackBar.open('You dont have permissions to edit', 'close', { duration: 1000 });
      return;
    }

    const subcategoryToUpdate = this.subcategories.find(s => s.id === id);
    // const subcategoryToUpdate = this.subcategories[index];

    this.subCategoryDialog = this.dialog.open(ScheduleSettingsSubcategoryFormComponent, {
      panelClass: 'Schedule-Subcategory-form-dialog',
      data: {
        subcategories: subcategoryToUpdate,
        action: 'edit'
      }
    });

    this.subCategoryDialog.afterClosed()
      .subscribe((subcategoryForm: FormGroup) => {

        if (!subcategoryForm) {
          return;
        }

        const subCategoryUpdated = new ScheduleSubcategoryBaseModel(subcategoryForm.getRawValue());
        this.scheduleSettingsCategoryService.loadingSubject.next(true);
        this.scheduleSettingsCategoryService.updateSubcategory(subCategoryUpdated)
          .then(
            () => {
              this.getSubcategories(this.schedule.id);
              this.scheduleSettingsCategoryService.loadingSubject.next(false);
              this.snackBar.open('Expense type updated successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.scheduleSettingsCategoryService.loadingSubject.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.scheduleSettingsCategoryService.loadingSubject.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });
  }

  save(scheduleFormGroup: FormGroup): void {
    this.scheduleSettingsCategoryService.loadingSubject.next(true);
    this.scheduleSettingsCategoryService.createElement(scheduleFormGroup.getRawValue())
      .then(
        (expenseType) => {
          if (this.subcategories.length === 0) {
            this.scheduleSettingsCategoryService.loadingSubject.next(false);
            this.dialogRef.close('success');
          } else {
            this.generatePostRequest(Number((expenseType['body']['id'])));
          }
        },
        () => this.dialogRef.close('Oops, there was an error'))
      .catch(() => {
        this.dialogRef.close('Oops, there was an error');
        this.scheduleSettingsCategoryService.loadingSubject.next(false);
      });
  }

  update(scheduleFormGroup: FormGroup): void {
    this.scheduleSettingsCategoryService.updateElement(scheduleFormGroup.getRawValue())
      .then(
        () => this.dialogRef.close('success'),
        () => this.dialogRef.close('Oops, there was an error'))
      .catch(() => this.dialogRef.close('Oops, there was an error'));
  }

  generatePostRequest(scheduleCategoryId: number): void {
    const posts = [];
    this.subcategories.forEach(
      subcategory => {
        subcategory.id = 0;
        subcategory.scheduleSettingCategoryId = scheduleCategoryId;
        posts.push(this.scheduleSettingsCategoryService.saveSubcategory(subcategory));
      }
    );

    forkJoin(posts).subscribe(
      response => {
        this.dialogRef.close('success');
        this.scheduleSettingsCategoryService.loadingSubject.next(false);
      }, (error) => {
        this.scheduleSettingsCategoryService.loadingSubject.next(false);
        this.dialogRef.close('Oops, there was an error adding subcategory');
      });
  }

  removeSubcategory(item: ScheduleSubcategoryBaseModel, index: number): void {
    const indice = this.subcategories.indexOf(item, index);
    this.subcategories.splice(indice, 1);
  }
}
