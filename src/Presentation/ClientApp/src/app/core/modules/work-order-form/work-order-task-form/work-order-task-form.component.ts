import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ServiceGridModel } from '@app/core/models/service/service-grid.model';
import { WorkOrderServiceFrequency } from '@app/core/models/work-order-services/work-order-services-base.model';
import { WorkOrderTaskUpdateModel } from '@app/core/models/work-order-task/work-order-task-update.model';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { PermissionService } from '@app/core/services/permission.service';
import { ServicesService } from '@app/main/content/private/services/services.service';
import { WorkOrderServicesService } from '@app/main/content/private/work-order-services/work-order-services.service';
import { WorkOrderServiceListItemModel } from '../../../models/work-order-services/work-order-services-list.model';

@Component({
  selector: 'app-work-order-task-form',
  templateUrl: './work-order-task-form.component.html',
  styleUrls: ['./work-order-task-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class WorkOrderTaskFormComponent implements OnInit {

  action = '';
  title = '';
  taskForm: FormGroup;
  workOrderTask: WorkOrderTaskUpdateModel;

  services: ServiceGridModel[] = [];
  selectedService: ServiceGridModel;

  files: File[] = [];
  localAttachments: { id: number, url: any, description: string, file: File }[] = [];

  newWorkOrder = false;

  // Category
  categories: ListItemModel[] = [];
  selectedCategory: ListItemModel;
  workOrderServices: WorkOrderServiceListItemModel[] = [];
  selectedWorkOrderService: WorkOrderServiceListItemModel;

  hideAttachments = false;
  showDates = false;
  editAttachments = true;
  showBilling = true;

  // Work Order Service
  schedulingRequired = false;
  hoursRequired = false;
  quantityRequired = false;
  isCompleted = false;

  // View Events
  btnUpdateWorkOrderTask = false;

  // Notes for Billing
  viewNotesforBilling = false;

  constructor(
    public dialogRef: MatDialogRef<WorkOrderTaskFormComponent>,
    private _formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) data: any,
    private _service: ServicesService,
    private _snackBar: MatSnackBar,
    private _epochPipe: FromEpochPipe,
    private _workOrderCategoryService: WorkOrderServicesService,
    private _permissionService: PermissionService
  ) {
    this.action = data.action;
    this.newWorkOrder = data.newWo;
    data.hasOwnProperty('hideAttachments') ? this.hideAttachments = data.hideAttachments : this.hideAttachments = false;
    data.hasOwnProperty('showDates') ? this.showDates = data.showDates : this.showDates = false;
    data.hasOwnProperty('editAttachments') ? this.editAttachments = data.editAttachments : this.editAttachments = true;
    data.hasOwnProperty('showBilling') ? this.showBilling = data.showBilling : this.showBilling = true;

    if (this.action === 'edit') {
      this.title = 'Edit Work Order Task';
      this.workOrderTask = data.task;
      this.isCompleted = this.workOrderTask.isComplete;
      this.taskForm = this.updateTaskForm();
      this.checkAttachments();

      if (this.workOrderTask.workOrderServiceCategoryId) {
        this.taskForm.get('workOrderServiceCategoryId').setValidators([Validators.required]);
      }

      this.quantityRequired = this.workOrderTask.quantityRequiredAtClose;

      if (this.showDates) {
        this.initAdditionalEditionData();
      }
    } else {
      this.title = 'New Work Order Task';
      this.taskForm = this.createTaskForm();
    }

    this.updateViewPermissions();
    this._permissionService.onPermissionsChanged.subscribe((permissions) => {
      this.updateViewPermissions();
    });
  }

  ngOnInit(): void {
    this.getCategories();

    this.taskForm.get('workOrderServiceCategoryId').valueChanges
      .subscribe(value => {
        if (value > 0) {
          this.getWorkOrderServices(value);

          this.selectedCategory = this.categories.find(c => c.id === value);
          if (this.selectedCategory != null) {
            this.taskForm.patchValue({ categoryName: this.selectedCategory.name });
          }
        }
      });

    this.taskForm.get('workOrderServiceId').valueChanges
      .subscribe(value => {
        this.selectedWorkOrderService = this.workOrderServices.find(s => s.id === value);
        if (this.selectedWorkOrderService) {
          this.taskForm.patchValue({
            unitPrice: this.selectedWorkOrderService.rate,
            unitFactor: this.selectedWorkOrderService.unitFactor,
            frequency: this.selectedWorkOrderService.frequency,
            rate: this.selectedWorkOrderService.rate,
            serviceName: this.selectedWorkOrderService.name,
            requiresScheduling: this.selectedWorkOrderService.requiresScheduling,
            hoursRequiredAtClose: this.selectedWorkOrderService.hoursRequiredAtClose,
            quantityRequiredAtClose: this.selectedWorkOrderService.quantityRequiredAtClose
          });

          this.schedulingRequired = this.selectedWorkOrderService.requiresScheduling;
          this.hoursRequired = this.selectedWorkOrderService.hoursRequiredAtClose;
          // this.quantityRequired = this.selectedWorkOrderService.quantityRequiredAtClose;
        }
      });

    this.taskForm.get('rate').valueChanges
      .subscribe(value => {
        if (!isNaN(Number(value))) {
          const quantity = this.taskForm.get('quantity').value;
          if (!isNaN(Number(quantity)) && value) {
            const total = value * quantity;
            console.log(value + ' * ' + quantity + '=' + total);
            this.taskForm.patchValue({
              total: total
            });
          }
        } else {
          this._snackBar.open('Invalid rate value', 'close', { duration: 2000 });
        }
      });

    this.taskForm.get('quantity').valueChanges
      .subscribe(value => {
        if (!isNaN(Number(value))) {
          const rate = this.taskForm.get('rate').value;
          if (!isNaN(Number(rate)) && value) {
            const total = value * rate;
            console.log(value + ' * ' + rate + '=' + total);
            this.taskForm.patchValue({
              total: total
            });
          }
        } else {
          this._snackBar.open('Invalid quantity value', 'close', { duration: 2000 });
        }
      });

    this.taskForm.get('quantityRequiredAtClose').valueChanges
      .subscribe(value => {
        this.quantityRequired = value;
      });
  }

  // Forms
  createTaskForm(): FormGroup {
    return this._formBuilder.group({
      isComplete: [false],
      description: ['', [Validators.required]],
      serviceId: [null],
      unitPrice: [0],
      quantity: [0, [Validators.required]],
      discountPercentage: [0],
      note: [''],
      location: ['', [Validators.required]],
      workOrderServiceCategoryId: [null],
      workOrderServiceId: [null],
      unitFactor: [],
      frequency: [WorkOrderServiceFrequency.OneTime],
      rate: [0, [Validators.required]],
      serviceName: [''],
      attachments: this._formBuilder.array([]),
      total: [{ value: 0, disabled: true }],
      requiresScheduling: [false],
      hoursRequiredAtClose: [false],
      quantityRequiredAtClose: [false],
      completedDate: [null],
      categoryName: [''],
      generalNote: [''],
    });
  }

  updateTaskForm(): FormGroup {
    return this._formBuilder.group({
      id: [this.workOrderTask.id],
      workOrderId: [this.workOrderTask.workOrderId],
      isComplete: [this.workOrderTask.isComplete],
      description: [this.workOrderTask.description, [Validators.required]],
      serviceId: [this.workOrderTask.serviceId],
      unitPrice: [this.workOrderTask.unitPrice],
      quantity: [this.workOrderTask.quantity, [Validators.required]],
      discountPercentage: [this.workOrderTask.discountPercentage],
      note: [this.workOrderTask.note],
      location: [this.workOrderTask.location, [Validators.required]],
      workOrderServiceCategoryId: [this.workOrderTask.workOrderServiceCategoryId],
      workOrderServiceId: [this.workOrderTask.workOrderServiceId],
      unitFactor: [this.workOrderTask.unitFactor],
      frequency: [this.workOrderTask.frequency],
      rate: [this.workOrderTask.rate, [Validators.required]],
      serviceName: [this.workOrderTask.serviceName],
      attachments: this._formBuilder.array([]),
      total: [{ value: (this.workOrderTask.quantity * this.workOrderTask.rate), disabled: true }],
      requiresScheduling: [false],
      hoursRequiredAtClose: [false],
      quantityRequiredAtClose: [this.workOrderTask.quantityRequiredAtClose],
      quantityExecuted: [{ value: this.workOrderTask.quantityExecuted, disabled: true }],
      hoursExecuted: [{ value: this.workOrderTask.hoursExecuted, disabled: true }],
      completedDate: [{ value: this.workOrderTask.completedDate, disabled: true }],
      categoryName: [''],
      createdDate: [this.workOrderTask.createdDate],
      generalNote: [this.workOrderTask.generalNote],
    });
  }

  initAdditionalEditionData(): void {
    this.taskForm.get('createdDate').setValidators([Validators.required]);

    this.taskForm.addControl('lastCheckedDate',
      new FormControl(this.getValidDate(this.workOrderTask.lastCheckedDate, this.workOrderTask.echoLastCheckedDate)));
  }

  // Services
  getWorkOrderServices(categoryId: number): void {
    this.workOrderServices = [];
    this._workOrderCategoryService.getAllAsList('ReadAllServicesCbo', '', 0, 20, null, { 'categoryId': categoryId.toString() })
      .subscribe((response: { count: number, payload: WorkOrderServiceListItemModel[] }) => {

        this.workOrderServices = response.payload;

        if (this.workOrderTask) {
          if (this.workOrderTask.workOrderServiceId > 0) {
            this.selectedWorkOrderService = this.workOrderServices.find(s => s.id === this.workOrderTask.workOrderServiceId);
            if (this.selectedWorkOrderService) {
              this.taskForm.patchValue({
                requiresScheduling: this.selectedWorkOrderService.requiresScheduling,
                hoursRequiredAtClose: this.selectedWorkOrderService.hoursRequiredAtClose
                // quantityRequiredAtClose: this.selectedWorkOrderService.quantityRequiredAtClose
              });

              this.schedulingRequired = this.selectedWorkOrderService.requiresScheduling;
              this.hoursRequired = this.selectedWorkOrderService.hoursRequiredAtClose;
              // this.quantityRequired = this.selectedWorkOrderService.quantityRequiredAtClose;
            }
          }
        }

      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get services', 'Close');
      });
  }

  // Return a valid date, a valid date is a date different of null or default value
  getValidDate(dateToValidate: any, epochDate: number): any {

    const possibleDate: any = new Date(dateToValidate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return null;
    }
    else {
      return new Date(this._epochPipe.transform(epochDate));
    }
  }

  // Attachments
  get taskAttachments(): FormArray {
    return this.taskForm.get('attachments') as FormArray;
  }

  fileChange(files: File[]): void {
    if (files.length > 0) {
      try {
        for (let index = 0; index < files.length; index++) {
          const reader = new FileReader();
          this.files.push(files[index]);
          reader.readAsDataURL(files[index]);
          reader.onload = (_event) => {
            const newAttachment = {
              id: 0,
              url: reader.result,
              description: files[index].name,
              file: files[index]
            };
            this.localAttachments.push(newAttachment);
          };
        }
      } catch (error) { console.log(error); }
    }
  }

  removeUploadedAttachment(index: number): void {
  }

  removeLocalAttachment(index: number): void {
    this.localAttachments.splice(index, 1);
  }

  checkAttachments(): void {
    const attachmentFormGroups = this.workOrderTask.attachments.map(attachemnt => this._formBuilder.group(attachemnt));
    const attachmentFormArray = this._formBuilder.array(attachmentFormGroups);
    this.taskForm.setControl('attachments', attachmentFormArray);
  }

  // Categories
  getCategories(): void {
    this.categories = [];
    this._workOrderCategoryService.getAllAsList('ReadAllCategoriesCbo', '', 0, 20, null, {})
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {

        this.categories = response.payload;

        if (this.workOrderTask.workOrderServiceCategoryId > 0) {
          this.getWorkOrderServices(this.workOrderTask.workOrderServiceCategoryId);
          this.selectedCategory = this.categories.find(c => c.id === this.workOrderTask.workOrderServiceCategoryId);
          if (this.selectedCategory != null) {
            this.taskForm.patchValue({ categoryName: this.selectedCategory.name });
          }
        }
      }, (error) => {
        this._snackBar.open('Ops! Error when trying to get categories', 'Close');
      });
  }

  // Actions
  submit(): void {

    if (!this.btnUpdateWorkOrderTask && this.action === 'edit') {
      this._snackBar.open('You do not have the permissions to execute this action', 'Close', { duration: 3000 });
      return;
    }

    try {
      const localFiles: { description: string, fileName: string, file: File }[] = [];
      this.localAttachments.forEach(a => {
        localFiles.push(
          {
            description: a.description,
            fileName: a.file.name,
            file: a.file
          });
      });
      this.dialogRef.close({ form: this.taskForm, files: localFiles });
    } catch (error) { console.log(error); }
  }

  // Permissions
  updateViewPermissions(): void {
    if (this._permissionService.permissions.length > 0) {
      const UpdateWorkOrdersTasks = this._permissionService.permissions.find(p => p.name === 'UpdateWorkOrdersTasks');
      this.btnUpdateWorkOrderTask = UpdateWorkOrdersTasks ? UpdateWorkOrdersTasks.isAssigned : false;
      const viewNotesforBilling = this._permissionService.permissions.find(p => p.name === 'WorkOrderTaskBillingNotes');
      this.viewNotesforBilling = viewNotesforBilling ? viewNotesforBilling.isAssigned : false;
    }
  }

}
