import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractExpenseBaseModel } from '@app/core/models/contract-expense/contract-expense-base.model';
import { ExpenseSubcategoryFormComponent } from '@app/core/modules/expenses-form/expense-subcategory-form/expense-subcategory-form.component';
import { ExpenseTypeFormComponent } from '@app/core/modules/expenses-form/expense-type-form/expense-type-form.component';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { ExpensesTypesService } from '../../../expenses-types/expenses-types.service';

@Component({
  selector: 'app-contract-expense-form',
  templateUrl: './contract-expense-form.component.html',
  styleUrls: ['./contract-expense-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractExpenseFormComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  action: string;

  contractExpense: ContractExpenseBaseModel;
  contractExpenseForm: FormGroup;

  expenseType: any;
  expenseTypes: ListItemModel[] = [];
  filteredExpenseTypes: Subject<any[]> = new Subject<any[]>();
  listExpenseTypesSubscription: Subscription;

  hasSubcategories = false;
  subcategorySelected: any;
  subcategories: ListItemModel[] = [];
  listSubcategoriesSubscription: Subscription;

  expenseTypeFormRef: any;
  expenseSubcategoryFormRef: any;

  daysPerMonth: number;

  types: any[] = [
    { 'id': 0, 'name': 'Hour' },
    // { 'id': 1, 'name': 'Amount' },
    { 'id': 2, 'name': 'Room' },
    { 'id': 3, 'name': 'Square Feet' },
    { 'id': 4, 'name': 'Unit' },
    { 'id': 5, 'name': 'Device' }
  ];

  periodicities: any[] = [
    { 'id': 0, 'name': 'Daily' },
    { 'id': 0, 'name': 'Monthly' },
    { 'id': 0, 'name': 'Yearly' }
  ];

  constructor(
    public dialogRef: MatDialogRef<ContractExpenseFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private expenseTypeService: ExpensesTypesService,
    public snackBar: MatSnackBar,
    public dialog: MatDialog
  ) {
    this.action = data.action;
    this.daysPerMonth = data.daysPerMonth === 0 ? 1 : data.daysPerMonth;
    if (this.action === 'new') {
      this.dialogTitle = 'New Expense';
      this.contractExpenseForm = this.createExpenseForm();
    } else {
      this.dialogTitle = 'Update Expense';
      this.contractExpense = data.contractExpense;
      this.contractExpenseForm = this.updateExpenseForm();

      // this.calculateTotals(this.contractExpense.value);
    }
  }

  ngOnInit(): void {
    this.getAllExpenseTypes();
  }

  ngOnDestroy(): void {

    if (this.listExpenseTypesSubscription && !this.listExpenseTypesSubscription.closed) {
      this.listExpenseTypesSubscription.unsubscribe();
    }

    if (this.listSubcategoriesSubscription && !this.listSubcategoriesSubscription.closed) {
      this.listSubcategoriesSubscription.unsubscribe();
    }

  }

  createExpenseForm(): FormGroup {
    return this.formBuilder.group({
      quantity: [1, [Validators.required]],
      description: ['', [Validators.required]],
      expenseTypeId: ['', [Validators.required]],
      expenseCategory: [0],
      expenseTypeName: [''],
      expenseSubcategoryId: ['', [Validators.required]],
      expenseSubcategoryName: [''],
      rate: [0, [Validators.required]],
      overheadPercent: [14],
      rateType: [0],
      ratePeriodicity: [''],
      value: [0],
      dailyRate: [{ value: '', disabled: true }, [Validators.required]],
      monthlyRate: [{ value: '', disabled: true }, [Validators.required]],
      biMonthlyRate: [{ value: '', disabled: true }, [Validators.required]],
      quarterly: [{ value: '', disabled: true }, [Validators.required]],
      biAnnually: [{ value: '', disabled: true }, [Validators.required]],
      yearlyRate: [{ value: '', disabled: true }, [Validators.required]]
    });
  }

  updateExpenseForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.contractExpense.id],
      quantity: [this.contractExpense.quantity, [Validators.required]],
      description: [this.contractExpense.description, [Validators.required]],
      expenseTypeId: [this.contractExpense.expenseTypeId, [Validators.required]],
      contractId: [this.contractExpense.contractId],
      expenseCategory: [this.contractExpense.expenseCategory],
      expenseTypeName: [this.contractExpense.expenseTypeName],
      expenseSubcategoryId: [this.contractExpense.expenseSubcategoryId, [Validators.required]],
      expenseSubcategoryName: [this.contractExpense.expenseSubcategoryName],
      rate: [this.contractExpense.rate, [Validators.required]],
      overheadPercent: [this.contractExpense.overheadPercent],
      rateType: [this.contractExpense.rateType],
      ratePeriodicity: [this.contractExpense.ratePeriodicity],
      value: [this.contractExpense.value],
      dailyRate: [{ value: '', disabled: true }, [Validators.required]],
      monthlyRate: [{ value: '', disabled: true }, [Validators.required]],
      biMonthlyRate: [{ value: '', disabled: true }, [Validators.required]],
      quarterly: [{ value: '', disabled: true }, [Validators.required]],
      biAnnually: [{ value: '', disabled: true }, [Validators.required]],
      yearlyRate: [{ value: '', disabled: true }, [Validators.required]]
    });
  }


  /**
   * Get all the Expense Types that are enabled
   */
  getAllExpenseTypes(): void {

    if (this.listExpenseTypesSubscription && !this.listExpenseTypesSubscription.closed) {
      this.listExpenseTypesSubscription.unsubscribe();
    }

    this.listExpenseTypesSubscription = this.expenseTypeService
      .getAllAsList('readallcbo', '', 0, 999, 1, { 'isActive': '1' })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.expenseTypes = response.payload;
        this.filteredExpenseTypes.next(this.expenseTypes);


        if (this.contractExpense) {
          this.expenseTypeChanged(this.contractExpense.expenseTypeId);
        }

      });

  }
  /**
   * Display expense-type-form to create a new element
   */
  newExpenseType(): void {
    this.expenseTypeFormRef = this.dialog.open(ExpenseTypeFormComponent, {
      panelClass: 'expense-type-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.expenseTypeFormRef.afterClosed()
      .subscribe((response: string) => {
        if (!response) {
          return;
        }

        if (response === 'success') {
          this.snackBar.open('Expense type created successfully!!!', 'close', { duration: 1000 });
          this.getAllExpenseTypes();
        } else {
          this.snackBar.open(response, 'close', { duration: 1000 });
        }
      });
  }
  /**
   * Occurs when the value of Expense Type (Select) has been changed
   * @param id
   */
  expenseTypeChanged(id: number): void {

    if (id !== null) {
      this.expenseType = this.expenseTypes.find(e => e.id === id);

      if (this.expenseType.expenseCategory !== 0) {
        this.contractExpenseForm
          .patchValue({ overheadPercent: 0 });
      } else {
        this.contractExpenseForm
          .patchValue({ overheadPercent: this.contractExpense ? this.contractExpense.overheadPercent : 0 });
      }

      this.contractExpenseForm.patchValue({
        expenseTypeId: this.expenseType.id,
        expenseTypeName: this.expenseType.name,
        expenseCategory: this.expenseType.expenseCategory,
        expenseSubcategoryId: this.contractExpense ? this.contractExpense.expenseSubcategoryId : ''
      });
      this.getSubcategories(id);
    }

  }

  /**
   * Get all the subcategories of the expense type
   * @param expenseTypeId
   */
  getSubcategories(expenseTypeId: number): void {

    this.hasSubcategories = false;

    if (this.listSubcategoriesSubscription && !this.listSubcategoriesSubscription.closed) {
      this.listSubcategoriesSubscription.unsubscribe();
    }

    this.listSubcategoriesSubscription = this.expenseTypeService
      .getAllSubcategories(expenseTypeId, { 'isEnabled': '1' })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.subcategories = response.payload;

        if (this.contractExpense) {
          this.expenseSubcategoryChanged(this.contractExpense.expenseSubcategoryId);
        }
      });
  }
  /**
   * Display expense-subcategory-form to create a new element
   */
  newSubcategory(): void {
    this.expenseSubcategoryFormRef = this.dialog.open(ExpenseSubcategoryFormComponent, {
      panelClass: 'subcategory-dialog-form',
      data: {
        action: 'new'
      }
    });

    this.expenseSubcategoryFormRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        response.addControl('expenseTypeId', new FormControl(this.expenseType.id));
        this.expenseTypeService.saveSubcategory(response.getRawValue())
          .subscribe(
            () => {
              this.snackBar.open('Expense subcategory added successfully!!!', 'close', { duration: 1000 });
              this.getSubcategories(this.expenseType.id);
            },
            (error) => this.snackBar.open('Oops, there was an error.', 'close', { duration: 1000 })
          );
      });
  }
  expenseSubcategoryChanged(id: number): void {
    if (id !== null) {
      this.subcategorySelected = this.subcategories.find(s => s.id === id);
      if (this.subcategorySelected) {
        if (this.subcategorySelected.id === (this.contractExpense ? this.contractExpense.expenseSubcategoryId : 0)) {
          this.subcategorySelected.rateType = this.contractExpense.rateType;
          this.subcategorySelected.periodicity = this.contractExpense.ratePeriodicity;
        }
        this.contractExpenseForm.patchValue({
          expenseSubcategoryId: this.subcategorySelected.id,
          expenseSubcategoryName: this.subcategorySelected.name,
          rate: this.contractExpense ? this.contractExpense.rate : this.subcategorySelected.rate,
          rateType: this.subcategorySelected.rateType,
          ratePeriodicity: this.subcategorySelected.periodicity
        });

        if (this.contractExpense) {
          this.calculateTotals(this.contractExpense.value);
        }
      }
    }
  }

  /* MATH */
  calculateTotals(value: any): void {
    let rate: number;
    let quantity: number;

    if (!Number(value)) {
      this.setPeriodicityFormFieldsValues('', '', '', '', '', '');
      return;
    }

    if (Number(this.contractExpenseForm.controls['quantity'].value)) {
      quantity = this.contractExpenseForm.controls['quantity'].value;
    } else {
      this.setPeriodicityFormFieldsValues('', '', '', '', '', '');
      return;
    }

    if (Number(this.contractExpenseForm.controls['rate'].value)) {
      rate = this.contractExpenseForm.controls['rate'].value;
    } else {
      this.setPeriodicityFormFieldsValues('', '', '', '', '', '');
      return;
    }

    if (this.expenseType.expenseCategory === 0) {
      rate = rate + (rate * (Number(this.contractExpenseForm.get('overheadPercent').value) / 100));
    }

    this.calculateValues(value, rate, quantity);
  }

  calculateValues(value: number, rate: number, quantity: number): void {
    let dailyRate = 0;
    let monthlyRate = 0;
    let yearlyRate = 0;
    let biMonthlyRate = 0;
    let quarterly = 0;
    let biAnnually = 0;

    switch (this.subcategorySelected.periodicity) {
      case 'Daily':
        dailyRate = (value * rate);
        monthlyRate = (dailyRate * this.daysPerMonth);
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        yearlyRate = (monthlyRate * 12);
        break;

      case 'Monthly':
        monthlyRate = (value * rate);
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        dailyRate = (monthlyRate / this.daysPerMonth);
        yearlyRate = (monthlyRate * 12);
        break;

      case 'Bi-Monthly':
        biMonthlyRate = (value * rate);
        monthlyRate = biMonthlyRate / 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        dailyRate = (monthlyRate / this.daysPerMonth);
        yearlyRate = (monthlyRate * 12);
        break;

      case 'Quarterly':
        quarterly = (value * rate);
        biAnnually = quarterly * 2;
        yearlyRate = biAnnually * 2;
        monthlyRate = yearlyRate / 12;
        biMonthlyRate = monthlyRate * 2;
        dailyRate = (monthlyRate / 21.5);
        break;

      case 'Bi-Annually':
        biAnnually = (value * rate);
        yearlyRate = biAnnually * 2;
        monthlyRate = yearlyRate / 12;
        dailyRate = monthlyRate / this.daysPerMonth;
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        break;

      case 'Yearly':
        yearlyRate = (value * rate);
        monthlyRate = (yearlyRate / 12);
        biMonthlyRate = monthlyRate * 2;
        quarterly = monthlyRate * 3;
        biAnnually = quarterly * 2;
        dailyRate = (monthlyRate / this.daysPerMonth);
        break;
    }

    dailyRate = (dailyRate * quantity);
    monthlyRate = (monthlyRate * quantity);
    yearlyRate = (yearlyRate * quantity);

    this.setPeriodicityFormFieldsValues(
      Math.round(dailyRate * 100) / 100,
      Math.round(monthlyRate * 100) / 100,
      Math.round(biMonthlyRate * 100) / 100,
      Math.round(quarterly * 100) / 100,
      Math.round(biAnnually * 100) / 100,
      Math.round(yearlyRate * 100) / 100);
  }

  /* COMPLEMENTS */
  setPeriodicityFormFieldsValues(daily: any, monthly: any, biMonthlyRate: any, quarterly: any, biAnnually: any, yearly: any): void {
    this.contractExpenseForm.patchValue({
      dailyRate: daily,
      monthlyRate: monthly,
      biMonthlyRate: biMonthlyRate,
      quarterly: quarterly,
      biAnnually: biAnnually,
      yearlyRate: yearly
    });
  }

  // SAVE
  submit(): void {
    this.contractExpenseForm.patchValue({
      rateType: this.subcategorySelected.rateType,
      ratePeriodicity: this.subcategorySelected.periodicity
    });
    this.dialogRef.close(this.contractExpenseForm);
  }

}
