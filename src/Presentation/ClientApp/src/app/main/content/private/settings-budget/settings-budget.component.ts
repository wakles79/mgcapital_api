import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BudgetSettingsBaseModel } from '@app/core/models/budget-settings/budget-settings-base.model';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject } from 'rxjs';
import { SettingsBudgetService } from './settings-budget.service';

@Component({
  selector: 'app-settings-budget',
  templateUrl: './settings-budget.component.html',
  styleUrls: ['./settings-budget.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class SettingsBudgetComponent implements OnInit {

  loading$ = new BehaviorSubject<boolean>(false);
  settingsForm: FormGroup;

  editMode = false;

  budgetSettings: BudgetSettingsBaseModel = new BudgetSettingsBaseModel({
    companyId: 0,
    federalInsuranceContributionsAct: 0,
    federalUnemploymentTaxAct: 0,
    generalLedger: 0,
    id: 0,
    medicare: 0,
    minimumProfitMarginPercentage: 0,
    stateUnemploymentInsurance: 0,
    workersCompensation: 0
  });

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private settingsService: SettingsBudgetService,
    private _formBuilder: FormBuilder,
    private _snackBar: MatSnackBar) {
    this.settingsForm = this.createUpdateForm();
  }

  ngOnInit(): void {
  }

  createUpdateForm(): FormGroup {
    return this._formBuilder.group({
      id: [this.budgetSettings.id],
      companyId: [this.budgetSettings.companyId],
      minimumProfitMarginPercentage: [this.budgetSettings.minimumProfitMarginPercentage.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      federalInsuranceContributionsAct: [this.budgetSettings.federalInsuranceContributionsAct.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      medicare: [this.budgetSettings.medicare.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      federalUnemploymentTaxAct: [this.budgetSettings.federalUnemploymentTaxAct.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      stateUnemploymentInsurance: [this.budgetSettings.stateUnemploymentInsurance.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      workersCompensation: [this.budgetSettings.workersCompensation.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      generalLedger: [this.budgetSettings.generalLedger.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]]
    });
  }

  // BUTTONS
  enableEditMode(): void {
    this.editMode = true;
  }

  saveChanges(): void {

    if (!this.settingsForm.valid) {
      this._snackBar.open('Set valid values', 'close', { duration: 1000 });
      return;
    }

    this.settingsForm.patchValue({
      id: this.budgetSettings.id,
      companyId: this.budgetSettings.companyId
    });

    // this.loading$.next(true);
    // this.settingsService.update(this.settingsForm.getRawValue())
    //   .subscribe(() => {
    //     this.settingsService.loadSettings();
    //     this.editMode = false;
    //     this._snackBar.open('Settings updated successfully!', 'close', { duration: 1000 });
    //   }, (error) => {
    //     this.loading$.next(false);
    //     this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
    //   });
  }

  cancelEditMode(): void {
    this.loading$.next(true);
    this.settingsService.loadSettings();
    this.editMode = false;
  }

  // MATH
  get totalBudgetPercentage(): number {
    let total = 0;
    total += this.settingsForm.controls['minimumProfitMarginPercentage'].valid ? Number(this.settingsForm.controls['minimumProfitMarginPercentage'].value) : 0;
    total += this.settingsForm.controls['federalInsuranceContributionsAct'].valid ? Number(this.settingsForm.controls['federalInsuranceContributionsAct'].value) : 0;
    total += this.settingsForm.controls['medicare'].valid ? Number(this.settingsForm.controls['medicare'].value) : 0;
    total += this.settingsForm.controls['federalUnemploymentTaxAct'].valid ? Number(this.settingsForm.controls['federalUnemploymentTaxAct'].value) : 0;
    total += this.settingsForm.controls['stateUnemploymentInsurance'].valid ? Number(this.settingsForm.controls['stateUnemploymentInsurance'].value) : 0;
    total += this.settingsForm.controls['workersCompensation'].valid ? Number(this.settingsForm.controls['workersCompensation'].value) : 0;
    total += this.settingsForm.controls['generalLedger'].valid ? Number(this.settingsForm.controls['generalLedger'].value) : 0;
    return total;
  }

}
