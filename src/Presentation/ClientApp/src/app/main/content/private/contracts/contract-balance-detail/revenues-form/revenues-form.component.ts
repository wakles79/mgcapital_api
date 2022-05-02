import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RevenueBaseModel } from '@app/core/models/revenue/revenue-base.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-revenues-form',
  templateUrl: './revenues-form.component.html',
  styleUrls: ['./revenues-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class RevenuesFormComponent implements OnInit {

  private revenue: RevenueBaseModel;
  revenueForm: FormGroup;
  action: any;
  private today = new Date();
  dialogTitle: string;

  constructor(
    public dialogRef: MatDialogRef<RevenuesFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private snackBar: MatSnackBar
  ) {
    this.action = data.action;
    if (this.action === 'add') {
      this.revenueForm = this.createRevenueForm();
      this.dialogTitle = 'New Revenue';
    }else if (this.action === 'edit') {
      this.revenue = data.item;
      this.revenueForm = this.updateRevenueForm();
      this.dialogTitle = 'Edit Revenue';

    }


   }

  ngOnInit(): void {
  }

  createRevenueForm(): any {
    return this.formBuilder.group({
      date: [new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0), [Validators.required]],
      subTotal: [''],
      tax: [''],
      total: [''],
      description: [''],
      reference: [''],
      buildingFilter: [''],
      contractFilter: [''],
      custumer: [''],
      buildingId: [''],
      customerId: [''],
      contractId: ['']
    });
  }

  updateRevenueForm(): any {
    console.log((this.revenue.date));
    return this.formBuilder.group({
      id: [this.revenue.id],
      date: [this.revenue.date],
      subTotal: [this.revenue.subTotal],
      tax: [this.revenue.tax],
      total: [this.revenue.total],
      description: [this.revenue.description],
      reference: [this.revenue.reference],
      buildingFilter: [''],
      contractFilter: [''],
      custumer: [''],
      buildingId: [this.revenue.buildingId],
      customerId: [this.revenue.customerId],
      contractId: [this.revenue.contractId]
    });
  }

  onChangeTax(): void {
    const tax = this.revenueForm.get('tax').value;
    const subTotal = this.revenueForm.get('subTotal').value;

    const value = subTotal + ((subTotal / 100) * tax);

    if (subTotal !== 0) {
      this.revenueForm.patchValue({
        total: value
      });
    }
  }

}
