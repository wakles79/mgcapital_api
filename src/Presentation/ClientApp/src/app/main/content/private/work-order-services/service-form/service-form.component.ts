import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WorkOrderServiceBaseModel, WorkOrderServiceFrequency } from '@app/core/models/work-order-services/work-order-services-base.model';
import { fuseAnimations } from '@fuse/animations';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { WorkOrderServicesService } from '../work-order-services.service';

@Component({
  selector: 'app-service-form',
  templateUrl: './service-form.component.html',
  styleUrls: ['./service-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ServiceFormComponent implements OnInit {

  action: string;
  dialogTitle: string;

  service: WorkOrderServiceBaseModel;
  serviceForm: FormGroup;

  frequencies: ListItemModel[] = [];
  categories: ListItemModel[] = [];

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') === null ? false : true;
  }

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    public dialogRef: MatDialogRef<ServiceFormComponent>,
    private _formBuilder: FormBuilder,
    private _categoryService: WorkOrderServicesService
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Work Order Service';
      this.service = data.service;
      this.serviceForm = this.updateServiceForm();
    } else {
      this.dialogTitle = 'New Work Order Service';
      this.serviceForm = this.createServiceForm();

      if (data.hasOwnProperty('categoryId')) {
        const id = Number(data.categoryId);
        if (id > 0) {
          this.serviceForm.patchValue({ workOrderServiceCategoryId: id });
        }
      }
    }
  }

  ngOnInit(): void {
    this.getFrequency();
    this.getCategory();
  }

  // Form
  createServiceForm(): FormGroup {
    return this._formBuilder.group({
      workOrderServiceCategoryId: [null, [Validators.required]],
      name: ['', [Validators.required]],
      unitFactor: ['', [Validators.required]],
      frequency: ['', [Validators.required]],
      rate: ['', [Validators.required]],
      requiresScheduling: [false],
      quantityRequiredAtClose: [false],
      hoursRequiredAtClose: [false]
    });
  }
  updateServiceForm(): FormGroup {
    return this._formBuilder.group({
      id: [this.service.id],
      workOrderServiceCategoryId: [{ value: this.service.workOrderServiceCategoryId, disabled: this.readOnly }, [Validators.required]],
      name: [{ value: this.service.name, disabled: this.readOnly }, [Validators.required]],
      unitFactor: [{ value: this.service.unitFactor, disabled: this.readOnly }, [Validators.required]],
      frequency: [{ value: this.service.frequency, disabled: this.readOnly }, [Validators.required]],
      rate: [{ value: this.service.rate, disabled: this.readOnly }, [Validators.required]],
      requiresScheduling: [{ value: this.service.requiresScheduling, disabled: this.readOnly }],
      quantityRequiredAtClose: [{ value: this.service.quantityRequiredAtClose, disabled: this.readOnly }],
      hoursRequiredAtClose: [{ value: this.service.hoursRequiredAtClose, disabled: this.readOnly }]
    });
  }

  // Frequency
  getFrequency(): void {
    /*Covert enum WORK_ORDERS_PRIORITIES to array workOdersPriorities*/
    for (const n in WorkOrderServiceFrequency) {
      if (typeof WorkOrderServiceFrequency[n] === 'number') {
        this.frequencies.push({ id: WorkOrderServiceFrequency[n] as any, name: n.split(/(?=[A-Z])/).join(' ') });
      }
    }
  }

  // Category
  getCategory(): void {
    this._categoryService.readAllCategoriesAsList()
      .subscribe((result: { count: number, payload: any[] }) => {
        this.categories = result.payload;
      }, (error) => {

      });
  }

}
