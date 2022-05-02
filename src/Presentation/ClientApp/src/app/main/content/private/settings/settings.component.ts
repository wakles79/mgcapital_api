import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CompanySettingsDetailModel } from '@app/core/models/company-settings/company-settings-detail.model';
import { AuthService } from '@app/core/services/auth.service';
import { FilesService } from '@app/core/services/files.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { SettingsService } from './settings.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class SettingsComponent implements OnInit {

  loading$ = new BehaviorSubject<boolean>(false);
  settingsForm: FormGroup;

  settingsChangedSubscription: Subscription;
  companySettings: CompanySettingsDetailModel;

  editMode: boolean = false;

  logoUrl = 'assets/images/logos/angular-material.png';

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  public freshdeskKey: string;
  public freshdeskAgentId: string;

  constructor(
    private settingsService: SettingsService,
    private formBuilder: FormBuilder,
    private snackBar: MatSnackBar,
    private _fileService: FilesService,
    private _navigationService: FuseNavigationService,
    private _authService: AuthService
  ) {

    this.loading$.next(true);
    this.settingsChangedSubscription = this.settingsService.onSettingsDetailChanged.subscribe(
      (result: CompanySettingsDetailModel) => {
        this.companySettings = new CompanySettingsDetailModel(result);
        this.freshdeskKey = this.companySettings.freshdeskDefaultApiKey;
        this.freshdeskAgentId = this.companySettings.freshdeskDefaultAgentId;

        if (this.companySettings.logoFullUrl.length > 0) {
          this.logoUrl = this.companySettings.logoFullUrl;
        }
        this.settingsForm = this.createUpdateForm();
        this.loading$.next(false);
      }, (error) => {
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        this.loading$.next(false);
      });

  }

  ngOnInit(): void {
    this.settingsForm.controls['minimumProfitMarginPercentage'].valueChanges
      .subscribe(value => {

        let newValue = 0;
        if (value === 0 || !value) {
          // is null or 0
        } else if (!Number(value)) {
          this.snackBar.open('Oops, invalid number.', 'close', { duration: 1000 });
        } else {
          newValue = value;
        }

        this.settingsForm.patchValue({
          minimumProfitMarginPercentage: newValue
        });

      });

    this.settingsForm.get('freshdeskDefaultApiKey').valueChanges
      .subscribe(value => {
        this.freshdeskKey = value;
      });

    this.settingsForm.get('freshdeskDefaultAgentId').valueChanges
      .subscribe(value => {
        this.freshdeskAgentId = value;
      });
  }

  createUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.companySettings.id],
      companyId: [this.companySettings.companyId],
      minimumProfitMarginPercentage: [this.companySettings.minimumProfitMarginPercentage.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      federalInsuranceContributionsAct: [this.companySettings.federalInsuranceContributionsAct.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      medicare: [this.companySettings.medicare.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      federalUnemploymentTaxAct: [this.companySettings.federalUnemploymentTaxAct.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      stateUnemploymentInsurance: [this.companySettings.stateUnemploymentInsurance.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      workersCompensation: [this.companySettings.workersCompensation.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      generalLedger: [this.companySettings.generalLedger.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      stateTax: [this.companySettings.stateTax.toFixed(2), [Validators.required, Validators.pattern('^\\d+\\.\\d+$')]],
      freshdeskDefaultAgentId: [this.companySettings.freshdeskDefaultAgentId],
      freshdeskDefaultApiKey: [this.companySettings.freshdeskDefaultApiKey],
      gmailEnabled: [this.companySettings.gmailEnabled],
      gmailEmail: [this.companySettings.gmailEmail],
      emailSignature: [this.companySettings.emailSignature],
    });
  }

  updateSettings(): void {

  }

  /** Buttons */
  enableEditMode(): void {
    this.editMode = true;
  }

  saveChanges(): void {

    if (!this.settingsForm.valid) {
      this.snackBar.open('Set valid values', 'close', { duration: 1000 });
      return;
    }

    this.settingsForm.patchValue({
      id: this.companySettings.id,
      companyId: this.companySettings.companyId
    });

    this.loading$.next(true);
    this.settingsService.update(this.settingsForm.getRawValue())
      .subscribe(() => {
        this.settingsService.loadSettings();
        this.editMode = false;
        this.snackBar.open('Settings updated successfully!', 'close', { duration: 1000 });
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
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

  // LOGO
  fileChange(files: File[]): void {
    this.loading$.next(true);
    if (files.length > 0) {
      try {
        this._fileService.uploadFile(files)
          .subscribe((response: any) => {
            if (response.body.length > 0) {
              const logo = {
                companySettingsId: this.companySettings.id,
                blobName: response.body[0].blobName,
                fullUrl: response.body[0].fullUrl
              };

              this.settingsService.update(logo, 'UpdateLogo')
                .subscribe(() => {

                  try {
                    this._navigationService.onCompanyChange.next({ name: '', logo: logo.fullUrl });
                    const user = this._authService.currentUser;
                    user.companyLogoUrl = logo.fullUrl;
                    this._authService.setEmployee(user);
                  } catch (error) { console.log(error); }

                  this.settingsService.loadSettings();
                }, () => {
                  this.loading$.next(false);
                  this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                });
            }
            console.log(JSON.stringify(response));
          }, () => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      } catch (error) { console.log(error); }
    }
  }

}
