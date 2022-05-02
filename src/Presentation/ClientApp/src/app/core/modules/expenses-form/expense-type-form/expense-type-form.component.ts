import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ExpenseTypeBaseModel } from '@app/core/models/expense-type/expense-type-base.model';
import { ExpensesTypesService } from '@app/main/content/private/expenses-types/expenses-types.service';
import { forkJoin } from 'rxjs';
import { ExpenseSubcategoryBaseModel } from '@app/core/models/expense-subcategory/expense-subcategory-base.model';
import { ExpenseSubcategoryFormComponent } from '../expense-subcategory-form/expense-subcategory-form.component';

@Component({
  selector: 'app-expense-type-form',
  templateUrl: './expense-type-form.component.html',
  styleUrls: ['./expense-type-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ExpenseTypeFormComponent implements OnInit {

  expenseTypeDialog: any;
  expenseType: ExpenseTypeBaseModel;
  expenseTypeFormGroup: FormGroup;

  dialogTitle: string;
  action: string;

  categories: any[] = [
    { 'id': 0, 'name': 'Labor' },
    { 'id': 1, 'name': 'Equipments' },
    { 'id': 2, 'name': 'Supplies' },
    { 'id': 3, 'name': 'Others' }
  ];

  totalSubcategories: number;
  subcategories: ExpenseSubcategoryBaseModel[] = [];

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<ExpenseTypeFormComponent>,
    private expenseTypeService: ExpensesTypesService,
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {
    this.action = data.action;

    if (this.action === 'new') {
      this.dialogTitle = 'New Type of Expense';
      this.totalSubcategories = 1;
      this.expenseTypeFormGroup = this.createExpenseTypeForm();
    } else {
      this.dialogTitle = 'Update Type of Expense';
      this.expenseType = data.expenseType;
      this.getExpenseSubcategories(this.expenseType.id);
      this.expenseTypeFormGroup = this.updateExpenseTypeForm();
    }
  }

  ngOnInit(): void {
  }

  createExpenseTypeForm(): FormGroup {
    return this.formBuilder.group({
      expenseCategory: ['', [Validators.required]],
      description: ['', Validators.required],
      status: [1]
    });
  }

  updateExpenseTypeForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.expenseType.id],
      expenseCategory: [{ value: this.expenseType.expenseCategory, disabled: this.readOnly }, [Validators.required]],
      description: [{ value: this.expenseType.description, disabled: this.readOnly }, Validators.required],
      status: [{ value: this.expenseType.status, disabled: this.readOnly }]
    });
  }

  newSubcategory(): void {
    this.expenseTypeDialog = this.dialog.open(ExpenseSubcategoryFormComponent, {
      panelClass: 'subcategory-dialog-form',
      data: {
        action: 'new'
      }
    });

    this.expenseTypeDialog.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        if (this.action === 'new') {
          response.addControl('id', new FormControl(this.totalSubcategories));
          this.totalSubcategories++;
          this.subcategories.push(response.getRawValue());
        } else {
          response.addControl('expenseTypeId', new FormControl(this.expenseType.id));
          this.expenseTypeService.saveSubcategory(response.getRawValue())
            .subscribe(
              () => {
                this.snackBar.open('Expense subcategory added successfully!!!', 'close', { duration: 1000 });
                this.getExpenseSubcategories(this.expenseType.id);
              },
              (error) => this.snackBar.open('Oops, there was an error.', 'close', { duration: 1000 })
            );
        }
      });
  }

  updateSubcategory(id: number): void {

    if (this.readOnly) {
      return;
    }

    if (this.action === 'new') {

      const index = this.subcategories.findIndex(s => s.id === id);
      const subcategoryToUpdate = this.subcategories[index];

      this.expenseTypeDialog = this.dialog.open(ExpenseSubcategoryFormComponent, {
        panelClass: 'subcategory-dialog-form',
        data: {
          expenseSubcategory: subcategoryToUpdate,
          action: 'edit'
        }
      });

      this.expenseTypeDialog.afterClosed()
        .subscribe((subcategoryForm: FormGroup) => {
          if (!subcategoryForm) {
            return;
          }

          const subcategoryUpdated = new ExpenseSubcategoryBaseModel(subcategoryForm.getRawValue());
          this.subcategories[index] = subcategoryUpdated;
        });

    } else {

      // Get the subcategory
      this.expenseTypeService.getSubcategory(id)
        .subscribe(
          (response: any) => {
            // create new object
            const subcategoryToUpdate = new ExpenseSubcategoryBaseModel(response);

            this.expenseTypeDialog = this.dialog.open(ExpenseSubcategoryFormComponent, {
              panelClass: 'subcategory-dialog-form',
              data: {
                expenseSubcategory: subcategoryToUpdate,
                action: 'edit'
              }
            });

            this.expenseTypeDialog.afterClosed()
              .subscribe((subcategoryForm: FormGroup) => {

                if (!subcategoryForm) {
                  return;
                }

                const expenseTypeUpdated = new ExpenseSubcategoryBaseModel(subcategoryForm.getRawValue());
                this.expenseTypeService.updateSubcategory(expenseTypeUpdated)
                  .then(
                    () => {
                      this.getExpenseSubcategories(this.expenseType.id);
                      this.snackBar.open('Expense type updated successfully!!!', 'close', { duration: 1000 });
                    },
                    () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 })
                  )
                  .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
              });

          }, (error) => {
            this.snackBar.open('Oops, there was an error.', 'close', { duration: 1000 });
          }
        );

    }

  }

  removeSubcategory(id: number): void {

    if (this.action === 'edit') {
      return;
    }

    if (this.subcategories) {
      const index = this.subcategories.findIndex(s => s.id === id);

      if (this.subcategories.length === 1) {
        this.subcategories.pop();
      } else {
        this.subcategories.splice(index, 1);
      }
    }
  }

  getExpenseSubcategories(id: number): void {
    this.expenseTypeService.getAllSubcategories(id)
      .subscribe(
        (response: { count: number, payload: ExpenseSubcategoryBaseModel[] }) => {
          this.subcategories = response.payload;
        }, (error) => {
          this.snackBar.open('Oops, there was an error getting subcategories', 'close', { duration: 1000 });
        });
  }

  generatePostRequest(expenseTypeId: number): void {
    const posts = [];
    this.subcategories.forEach(
      subcategory => {
        subcategory.id = 0;
        subcategory.expenseTypeId = expenseTypeId;
        posts.push(this.expenseTypeService.saveSubcategory(subcategory));
      }
    );

    forkJoin(posts).subscribe(
      response => {
        this.dialogRef.close('success');
      }, (error) => this.dialogRef.close('Oops, there was an error adding subcategory'));
  }

  save(expenseForm: FormGroup): void {
    this.expenseTypeService.createElement(expenseForm.getRawValue())
      .then(
        (expenseType) => {
          if (this.subcategories.length === 0) {
            this.dialogRef.close('success');
          } else {
            this.generatePostRequest(Number((expenseType['body']['id'])));
          }
        },
        () => this.dialogRef.close('Oops, there was an error'))
      .catch(() => this.dialogRef.close('Oops, there was an error'));
  }

  update(expenseForm: FormGroup): void {
    this.expenseTypeService.updateElement(expenseForm.getRawValue())
      .then(
        () => this.dialogRef.close('success'),
        () => this.dialogRef.close('Oops, there was an error'))
      .catch(() => this.dialogRef.close('Oops, there was an error'));
  }

}
