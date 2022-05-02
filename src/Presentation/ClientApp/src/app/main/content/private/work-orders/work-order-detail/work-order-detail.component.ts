import { Component, OnInit, OnDestroy, ViewEncapsulation } from '@angular/core';
import { FormGroup, AbstractControl, FormControl, FormArray, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ListWOSourcesModel } from '@app/core/models/work-order/list-wo-sources.model';
import { WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model';
import { MessageDialogComponent } from '@app/core/modules/message-dialog/message-dialog/message-dialog.component';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { WoConfirmDialogComponent } from '../../../../../core/modules/work-order-dialog/wo-confirm-dialog/wo-confirm-dialog.component';
import { WoTaskFormConfirmCloseComponent } from '../../../../../core/modules/work-order-dialog/wo-task-form-confirm-close/wo-task-form-confirm-close.component';
import { WorkOrderTaskGridModel } from '../../../../../core/models/work-order-task/work-order-task-grid.model';
import { MatTableDataSource } from '@angular/material/table';
import { WorkOrderTaskFormComponent } from '@app/core/modules/work-order-form/work-order-task-form/work-order-task-form.component';
import { WorkOrderTaskCreateModel } from '../../../../../core/models/work-order-task/work-order-task-create.model';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import { WOSBillingDateTypeMOdel } from '@app/core/models/work-order/wo-billing-date-type.model';
import { ScheduleSubcategoryBaseModel } from '@app/core/models/schedule-subcategory/schedule-subcategory-base.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { DatePipe } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { WorkOrdersService } from '../work-orders.service';
import { BuildingsService } from '../../buildings/buildings.service';
import { ContactsService } from '../../contacts/contacts.service';
import { AuthService } from '@app/core/services/auth.service';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { ScheduleSettingsCategoryService } from '../../schedule-settings-category/schedule-settings-category.service';
import { UsersBaseService } from '../../users/users-base.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PermissionService } from '@app/core/services/permission.service';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { WORK_ORDERS_PRIORITIES } from '@app/core/models/work-order/work-order-priorities.model';
import { MatOptionSelectionChange } from '@angular/material/core';
import { MatSelectChange } from '@angular/material/select';
import { ListBuildingModel } from '@app/core/models/building/list-buildings.model';
import { ListScheduleCategoryModel } from '@app/core/models/schedule-category/list-schedule-category.model';
import { WorkOrderTaskUpdateModel } from '@app/core/models/work-order-task/work-order-task-update.model';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { CALENDAR_MONTH } from '@app/core/models/calendar/calendar-periodicity-enum';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { WorkOrderUpdateModel } from '@app/core/models/work-order/work-order-update.model';
import { WorkOrderTaskAttachmentModel } from '@app/core/models/work-order-task/work-order-task-attachment.model';
import { WorkOrderCreateModel } from '../../../../../core/models/work-order/work-order-create.model';
import { WorkOrderScheduleSetting } from '@app/core/models/work-order/work-order-schedule-setting.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-work-order-detail',
  templateUrl: './work-order-detail.component.html',
  styleUrls: ['./work-order-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class WorkOrderDetailComponent implements OnInit, OnDestroy {

  title = 'New';
  action = '';
  loading$ = new BehaviorSubject<boolean>(false);
  loadingTasks = false;
  workOrder: any = null;
  workOrderForm: FormGroup;
  workOrderId = 0;
  workOrderNumber: number;
  dueDate: Date = null;
  workOrderSource: WorkOrderSourceCode = WorkOrderSourceCode.Other; // EMail MG-23
  woSources: ListWOSourcesModel[] = [];

  private _today = new Date();

  // Dialogs
  messageDialog: MatDialogRef<MessageDialogComponent>;
  workOrderConfirmDialog: MatDialogRef<WoConfirmDialogComponent>;
  confirmDialog: MatDialogRef<FuseConfirmDialogComponent>;
  workOrderSequencesDialog: MatDialogRef<WorkOrderSequencesDialogComponent>;
  closeWorkOrderTaskDialog: MatDialogRef<WoTaskFormConfirmCloseComponent>;

  // Tasks
  workOrderTasks: WorkOrderTaskGridModel[] = [];
  tasksDataSource = new MatTableDataSource();
  tasksDisplayedColumns = ['completed', 'service', 'description', 'quantity', 'rate', 'total', 'options'];
  taskFormDialog: MatDialogRef<WorkOrderTaskFormComponent>;
  get workOrderDueDate(): AbstractControl { return this.workOrderForm.get('dueDate'); }

  /**
   * Save temporarily save tasks when creating a work order
   */
  unsavedTasks: WorkOrderTaskCreateModel[] = [];

  // Areas
  showAttachments = true;
  showBilling = true;
  showNotifications = true;
  showNotes = true;
  showSchedule = true;
  showCategory = false;

  // employees
  employees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  availableEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  buildingEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  addedEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  get selectedEmployees(): AbstractControl { return this.workOrderForm.get('assignedEmployees'); }
  workOrderEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  employeeTypeFormControl: FormControl;
  employeeSearch: FormControl;

  // Type, Priority, Billing Type, Role leve
  workOdersPriorities: { id: number, name: string }[] = [];
  workOrderTypes: { key: number, value: string }[] = WORK_ORDER_TYPES;
  workBillingType: { key: number, value: string }[] = WOSBillingDateTypeMOdel;
  roleLevelLoggedUser: any;

  // Category
  scheduleCategories: { id: number, description: string }[] = [];
  scheduleSubcategories: ScheduleSubcategoryBaseModel[] = [];

  // Status
  woStatus = 1;
  workOrderStatus: { id: number, name: string, disabled: boolean }[] = [];

  // Building
  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  buildingSelected = 0;
  get buildingCtrl(): AbstractControl {
    return this.workOrderForm.get('buildingCtrl');
  }

  // Attachment
  workOrderImages: Array<IAlbum> = [];
  get woAttachments(): FormArray {
    return this.workOrderForm.get('attachments') as FormArray;
  }

  // Notes
  textNote: any;

  // Schedule
  unscheduledStatus = false;
  get showSnoozeDate(): any { return this.workOrderForm.get('defineDate').value; }

  scheduleStartDate: Date = new Date();
  scheduleDate: Date = new Date();
  excludedScheduleDates: Date[] = [];
  scheduleDates: Date[] = [];
  scheduleQuantityCtrl: FormControl;
  scheduleEndDate: Date = new Date(this.scheduleStartDate.getFullYear(), this.scheduleStartDate.getMonth(), this.scheduleStartDate.getDate() + 1);
  selectedScheduleFrequency = 6;
  scheduleFrequency: { id: number; name: string }[] = [
    { id: 0, name: 'Daily' },
    { id: 1, name: 'Weekly' },
    { id: 2, name: 'Monthly' },
    { id: 3, name: 'Quarterly' },
    { id: 4, name: 'Semi-Annually' },
    { id: 5, name: 'Annually' },
    { id: 6, name: 'One Time' }
    // { id: 7, name: 'None' }
  ];
  scheduleDays: { id: number; name: string, isSelected: boolean }[] = [
    { id: 1, name: 'Monday', isSelected: false },
    { id: 2, name: 'Tuesday', isSelected: false },
    { id: 3, name: 'Wednesday', isSelected: false },
    { id: 4, name: 'Thursday', isSelected: false },
    { id: 5, name: 'Friday', isSelected: false },
    { id: 6, name: 'Saturday', isSelected: false },
    { id: 0, name: 'Sunday', isSelected: false }
  ];
  selectedScheduleOrdinal = 1;
  scheduleOrdinal: ListItemModel[] = [
    { id: 1, name: 'First' },
    { id: 2, name: 'Second' },
    { id: 3, name: 'Third' },
    { id: 4, name: 'Last' }
  ];
  scheduleStartMonth = 1;
  scheduleEndMonth = 1;
  selectedScheduleMonth = 1;
  scheduleStartYear: number;
  scheduleMonths: ListItemModel[] = [];

  // To Indicate when send notifications
  sendNotifications: boolean;

  allowEdit = true;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  // Lightbox
  lightboxAlbum: Array<IAlbum> = [];
  private _lightboxSubscription: Subscription;

  navigationSource = '';

  // View Events
  btnUpdateWorkOrder = false;

  btnAddTask = false;
  btnAddBillableTask = false;
  btnUpdateTask = false;
  btnDeleteTask = false;
  viewTaskBilling = false;

  btnAddWorkOrderNote = false;
  btnUpdateWorkOrderNote = false;
  btnDeleteWorkOrderNote = false;
  btnDeleteWorkOrderAttachment = false;

  constructor(
    private _formBuilder: FormBuilder,
    public _dialog: MatDialog,
    public _snackBar: MatSnackBar,
    private _woService: WorkOrdersService,
    private _buildingService: BuildingsService,
    private _authService: AuthService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _epochPipe: FromEpochPipe,
    private _schedueSettingsCategoryService: ScheduleSettingsCategoryService,
    private _userService: UsersBaseService,
    private _route: Router,
    private _router: ActivatedRoute,
    private _permissionService: PermissionService
  ) {
    this.sendNotifications = true;
    this.employeeTypeFormControl = new FormControl('all');
    this.employeeSearch = new FormControl('');

    this.scheduleStartYear = new Date().getFullYear();
    this.scheduleQuantityCtrl = new FormControl('');

    this.workOrderForm = this.createWorkOrderForm();

    // Readn Query params
    this._router.queryParamMap.subscribe((map: any) => {
      const action = map.params['action'];
      const workorderId: string = map.params['workorder'];
      this.navigationSource = map.params['source'];

      if (!action) {
        this.goBack();
        return;
      }

      if (action === 'edit' && isNaN(Number(workorderId))) {
        this.goBack();
        return;
      }

      this.action = action;

      if (this.action === 'edit') {
        this.workOrderId = Number(workorderId);
        this.getWorkOrder();
      } else if (this.action === 'new') {
      } else {
        this.goBack();
        return;
      }

      this.title = this.action.charAt(0).toUpperCase() + this.action.slice(1);
    });

    this.updateViewPermissions();
    this._permissionService.onPermissionsChanged.subscribe(() => {
      this.updateViewPermissions();
    });
  }

  ngOnInit(): void {

    this.getPriorities();
    this.getTicketStatus();
    this.getEmployees();
    this.getCategories();
    this.getMonths();
    this.getWOSources();

    if (this.action !== 'edit') {
      this.getBuildings();
    }

    this.roleLevelLoggedUser = this._authService.currentUser.roleLevel;

    try {
      this.workOrderForm.controls['buildingId'].valueChanges
        .subscribe(value => {
          this.buildingSelected = value;
          this.getBuildingEmployees(value);
        });

      this.buildingCtrl.valueChanges
        .pipe(takeUntil(this._onDestroy))
        .subscribe(() => {
          this.filterBuildings();
        });

      this.scheduleQuantityCtrl
        .valueChanges
        .pipe(debounceTime(400), takeUntil(this._onDestroy))
        .subscribe(value => {
          if (isNaN(Number(value))) {
          } else {
            this.scheduleEndDate = null;
            if (this.selectedScheduleFrequency === 1) {
              this.calculateWeeklyScheduleDates();
            } else if (this.selectedScheduleFrequency > 2 && this.selectedScheduleFrequency < 6) {
              this.calculateMonthlyScheduleDates();
            }
          }
        });

      this.employeeTypeFormControl.valueChanges
        .subscribe(value => {
          switch (value) {
            case 'all':
              this.getEmployees();
              break;
            case 'operationManager':
              this.getEmployees('30');
              break;
            case 'inspector':
              this.getEmployees();
              break;
            case 'supervisor':
              this.getEmployees(40);
              break;
            case 'subcontractor':
              this.getEmployees('35');
              break;
            default:
              this.getEmployees();
              break;
          }
        });

      this.employeeSearch
        .valueChanges
        .pipe(debounceTime(400), takeUntil(this._onDestroy))
        .subscribe(value => {
          this.getEmployees(null, value);
        });
    } catch (error) {
      console.log(error);
    }
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();

    if (this.action === 'new') {
      // Delete images uploaded in azure but not saved in data base
      if (this.woAttachments.length > 0) {
        for (let i = 0; i < this.woAttachments.value.length; i++) {
          if (this.woAttachments.at(i).get('id').value === -1) {
            this._woService.deleteAttachmentByBlobName(this.woAttachments.at(i).get('blobName').value).subscribe();
          }
        }
      }
    }
  }

  // Location
  goBack(): void {
    if (this.navigationSource === 'calendar') {
      this._route.navigate(['/app/calendar']);
    } else {
      this._route.navigate(['/app/work-orders']);
    }
  }

  // Forms
  createWorkOrderForm(): FormGroup {
    return this._formBuilder.group({
      id: [0],
      number: [0],
      administratorId: [this._authService.currentUser.employeeId],
      priority: [WORK_ORDERS_PRIORITIES.Low],
      type: [0],
      scheduleCategoryId: [''],
      scheduleSubCategoryId: [''],
      dueDate: [new Date(this._today.getFullYear(), this._today.getMonth(), this._today.getDate(), 18, 0, 0)],
      buildingId: ['', [Validators.required]],
      buildingCtrl: [''],
      location: ['', [Validators.required]],
      requesterFullName: ['', [Validators.required]],
      requesterEmail: ['', [Validators.required]],
      description: ['', [Validators.required]],
      assignedEmployeeId: [''],
      assignedEmployees: [],
      tasks: this._formBuilder.array([]),
      attachments: this._formBuilder.array([]),
      billingDateType: [''],
      billingName: [''],
      billingEmail: [''],
      billingNote: [''],
      statusId: [this.woStatus],
      sendRequesterNotifications: [false],
      sendPropertyManagersNotifications: [false],
      notes: this._formBuilder.array([]),
      defineDate: [false],

      clientApproved: [true],
      scheduleDateConfirmed: [true],
      setStatusByStandBy: [true],
      scheduleDate: [null],
      sendNotifications: [this.sendNotifications],
      unscheduled: [false],
      workOrderScheduleSettingId: [null],


      closingNotes: [''],
      additionalNotes: [0],
      closingNotesOther: [''],
      originWorkOrderId: [''],
      keepCloningReference: [true],
      followUpOnClosingNotes: [],

      workOrderSourceId: [],
      sourceCode: [this.workOrderSource],

      updateTasks: [false]
    });
  }
  updateWorkOrderForm(): void {
    this.workOrderForm.patchValue({
      id: this.workOrder.id,
      number: this.workOrder.number,
      administratorId: this.workOrder.administratorId,
      priority: this.workOrder.priority,
      type: this.workOrder.type,
      scheduleCategoryId: this.workOrder.scheduleCategoryId,
      scheduleSubCategoryId: this.workOrder.scheduleSubCategoryId,
      dueDate: this.dueDate ? this.dueDate : null,
      buildingId: this.workOrder.buildingId,
      location: this.workOrder.location,
      requesterFullName: this.workOrder.requesterFullName,
      requesterEmail: this.workOrder.requesterEmail,
      description: this.workOrder.description,
      billingDateType: this.workOrder.billingDateType,
      billingName: this.workOrder.billingName,
      billingEmail: this.workOrder.billingEmail,
      billingNote: this.workOrder.billingNote,
      statusId: this.woStatus,
      sendRequesterNotifications: this.workOrder.sendRequesterNotifications,
      sendPropertyManagersNotifications: this.workOrder.sendPropertyManagersNotifications,

      clientApproved: this.workOrder.clientApproved,
      scheduleDateConfirmed: this.workOrder.scheduleDateConfirmed,
      scheduleDate: this.workOrder.scheduleDate,
      unscheduled: this.workOrder.unscheduled,
      workOrderScheduleSettingId: 0,

      closingNotes: this.workOrder.closingNotes,
      additionalNotes: this.workOrder.additionalNotes,
      closingNotesOther: this.workOrder.closingNotesOther,
      originWorkOrderId: this.workOrder.originWorkOrderId,
      keepCloningReference: true,
      followUpOnClosingNotes: this.workOrder.followUpOnClosingNotes,

      workOrderSourceId: this.workOrder.workOrderSourceId,
      sourceCode: this.workOrderSource,

      updateTasks: false
    });
  }

  // Areas
  displayAttachmentArea(): void {
    if (this.workOrder) {
      if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
        return;
      }
    }
    this.showAttachments = true;
  }
  checkForAttachments(): void {
    // Check if there are attachments into woto show section "Attachments"
    if (this.workOrder.attachments.length > 0) {
      this.showAttachments = true;
      const attachmentFormGroups = this.workOrder.attachments.map(attachemnt => this._formBuilder.group(attachemnt));
      const attachmentFormArray = this._formBuilder.array(attachmentFormGroups);
      this.workOrderForm.setControl('attachments', attachmentFormArray);
    } else {
      this.workOrderForm.setControl('attachments', this._formBuilder.array([]));
    }
  }
  displayBillingArea(): void {
    if (this.workOrder) {
      if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
        return;
      }
    }
    this.showBilling = true;
  }
  checkForBilling(): void {
    if (this.workOrder.billingEmail || this.workOrder.billingName || this.workOrder.billingNote) {
      this.showBilling = true;
    }
  }
  displayNotificationArea(): void {
    if (this.workOrder) {
      if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
        return;
      }
    }
    this.showNotifications = true;
  }
  checkForNotifications(): void {
    if (this.workOrder.sendRequesterNotifications || this.workOrder.sendPropertyManagersNotifications) {
      this.showNotifications = true;
    }
  }
  displayNotesArea(): void {
    if (this.workOrder) {
      if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
        return;
      }
    }
    this.showNotes = true;
  }
  checkForNotes(): void {
    // Check if there are notes into work order to show section "Notes"
    if (this.workOrder.notes.length > 0) {
      const noteFormGroups = this.workOrder.notes.map(note => {

        // Assign created day form UTC to Local
        // const timeZone = new Date().getTimezoneOffset();

        // const createdDateTimeLocal: string = this._datePipe.transform(note.createdDate, 'yyyy-MM-dd HH:mm Z');
        // note.createdDate = new Date(createdDateTimeLocal);

        return this._formBuilder.group(note);
      });
      const noteFormArray = this._formBuilder.array(noteFormGroups);
      this.workOrderForm.setControl('notes', noteFormArray);
      this.showNotes = true;
    } else {
      this.workOrderForm.setControl('notes', this._formBuilder.array([]));
    }
  }
  displayScheduleArea(): void {
    if (this.workOrder) {
      if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
        return;
      }
    }
    this.showSchedule = true;
  }
  checkForSchedule(): void {
    if (this.workOrder.scheduleCategoryId || this.workOrder.scheduleSubCategoryId || this.workOrder.scheduleDate
      || this.workOrder.clientApproved || this.workOrder.scheduleDateConfirmed) {
      this.showSchedule = true;
    }
  }

  // Status
  getTicketStatus(): void {
    this.workOrderStatus = [
      { id: 1, name: 'Stand by', disabled: (this.woStatus === 3 || this.woStatus === 0 || this.roleLevelLoggedUser >= 30) },
      { id: 2, name: 'Active', disabled: (this.woStatus === 0 || this.roleLevelLoggedUser >= 30) },
      { id: 3, name: 'Closed', disabled: (this.woStatus === 0 || this.roleLevelLoggedUser >= 40) },
      { id: 4, name: 'Cancelled', disabled: (this.woStatus === 0 || this.roleLevelLoggedUser >= 40) }
    ];
  }

  // Employees
  getEmployees(roleLeveL = null, filter = ''): void {
    this.employees = [];
    this.availableEmployees = [];
    this._userService.getAllAsList('readallcbo', filter, 0, 99999, null, { 'roleLevel': roleLeveL })
      .subscribe((response: { count: number, payload: any[] }) => {
        response.payload.forEach(element => {
          const employee: { id: number, name: string, roleName: string, email: string, type: number } =
          {
            id: element.id,
            name: element.name,
            roleName: element.roleName,
            email: element.email,
            type: 0
          };

          switch (element.level) {
            // Supervisor
            case 40:
              employee.type = 1;
              break;

            // Operations Manager
            case 30:
              employee.type = 2;
              break;

            // Temporary Operations Manager
            case 35:
              employee.type = 4;
              break;

            // Inspecto
            default:
              employee.type = 8;
              break;
          }

          // not display master
          if (element.level > 10) {
            this.employees.push(employee);
            this.availableEmployees.push(employee);
          }
        });

      }, () => {
        this._snackBar.open('Oops, there was an error fetching employees', 'close', { duration: 1000 });
      });
  }
  getBuildingEmployees(id: number): void {
    this._userService.getEmployeesByBuilding(id)
      .subscribe((response: { count: number, payload: { id: number, name: string, roleName: string, email: string, type: number }[] }) => {
        this.buildingEmployees = response.payload;
        if (this.employees.length > 0) {
          this.availableEmployees = [];
          this.employees.forEach(e => {
            this.availableEmployees.push(e);
          });
          response.payload.forEach(b => {
            // remove from employee list
            const index = this.availableEmployees.findIndex(e => e.id === b.id);
            if (index >= 0) {
              this.availableEmployees.splice(index, 1);
            }
          });
        }
        this.verifySelectedBuildingEmployees();
      }, () => {
        this._snackBar.open('Oops, there was an error fetching employees', 'close', { duration: 1000 });
      });
  }
  compareFn(c1: any, c2: any): boolean {
    return c1 && c2 ? c1.id === c2.id : c1 === c2;
  }
  verifySelectedBuildingEmployees(): void {
    // remove from selected employees
    if (this.addedEmployees.length > 0) {
      this.buildingEmployees.forEach(b => {
        const indexS = this.addedEmployees.findIndex(e => e.id === b.id);
        if (indexS >= 0) {
          this.addedEmployees.splice(indexS, 1);
        }

        const index = this.workOrderEmployees.findIndex(e => e.id === b.id);
        if (index >= 0) {
          this.workOrderEmployees.splice(index, 1);
        }
      });
    }
  }
  removeEmployee(id: number): void {
    const index = this.workOrderEmployees.findIndex(e => e.id === id);
    if (index >= 0) {
      this.workOrderEmployees.splice(index, 1);

      const indexSelected = this.addedEmployees.findIndex(e => e.id === id);
      if (indexSelected >= 0) {
        this.addedEmployees.splice(indexSelected, 1);
        this.workOrderForm.get('assignedEmployees').setValue(this.addedEmployees);
      }
    }
  }
  onEmployeeSelectionChanged(event: MatOptionSelectionChange): void {
    if (event.isUserInput) {
      const employee = event.source.value;
      const existIndex = this.workOrderEmployees.findIndex(r => r.id === employee.id);
      if (event.source.selected) {
        // Add to work order employees
        if (existIndex < 0) {
          const index = this.employees.findIndex(e => e.id === employee.id);
          if (index >= 0) {
            this.workOrderEmployees.push(employee);
          }
        }
      } else {
        // Remove employee
        if (existIndex >= 0) {
          this.workOrderEmployees.splice(existIndex, 1);
        }
      }
    }
  }

  // Priority
  getPriorities(): void {
    /*Covert enum WORK_ORDERS_PRIORITIES to array workOdersPriorities*/
    for (const n in WORK_ORDERS_PRIORITIES) {
      if (typeof WORK_ORDERS_PRIORITIES[n] === 'number') {
        this.workOdersPriorities.push({ id: WORK_ORDERS_PRIORITIES[n] as any, name: n.replace(/_/g, ' ') });
      }
    }
  }

  // Types
  typeChanged(event: MatSelectChange): void {
    if (event.value === 4) {
      this.showCategory = true;
    } else {
      this.showCategory = false;
      this.workOrderForm.patchValue({
        scheduleCategoryId: '',
        scheduleSubCategoryId: ''
      });
      this.scheduleSubcategories = [];
    }
  }

  // Source
  getWOSources(): void {
    this._woService.getAllAsList('readallwosourcescbo', '', 0, 20, null, {})
      .subscribe((response: { count: number, payload: ListWOSourcesModel[] }) => {

        if (this.action === 'edit') {
          //this.woSources = response.payload; //MG-23
          this.woSources = response.payload.filter(source => source.code > 0 && source.code < 10 || source.code == 14 || source.code == 16); // MG-23
        }
        else {
          this.woSources = response.payload.filter(source => (source.code > 0 && source.code < 4) || (source.code > 4 && source.code < 10) || source.code == 14 || source.code == 16);
        }
      },
        () => this._snackBar.open('Oops, there was an error fetching work order sources', 'close', { duration: 1000 }),
        () => {
          // For default work order source should be "Email" : code = 0
          if (this.action !== 'edit') {
            this.workOrderForm.get('workOrderSourceId').setValue(this.woSources.find(w => w.code === 0).id);
          }
        }
      );
  }

  // Buildings
  getBuildings(filter = ''): void {
    this._buildingService.getAllAsList('readallcbo', filter, 0, 20, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        () => this._snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }
  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    const search = (this.buildingCtrl.value || '').toLowerCase();
    if (search === '' && this.buildingSelected) {
      return;
    }
    // filter the buildings
    this.getBuildings(search);
  }
  buildingChanged(event: MatSelectChange): void {
    this.buildingSelected = event.value;
  }

  // Category
  getCategories(filter = ''): void {
    this._schedueSettingsCategoryService.getAllAsList('readallcbo', filter, 0, 20, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListScheduleCategoryModel[] }) => {
        this.scheduleCategories = response.payload;
        if (this.workOrderForm.controls['scheduleCategoryId'].value) {
          this.categoryChanged({ value: this.workOrderForm.controls['scheduleCategoryId'].value, source: null });
        }
      },
        () => this._snackBar.open('Oops, there was an error fetching schedule categories', 'close', { duration: 1000 }));
  }
  categoryChanged(event: MatSelectChange): void {
    this._schedueSettingsCategoryService.getAllAsListByCategory(event.value)
      .subscribe((response: { count: number, payload: ScheduleSubcategoryBaseModel[] }) => {
        this.scheduleSubcategories = response.payload;
      }, () => {
        this._snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });

  }

  // Atachments
  fileChange(files: File[]): void {
    if (files.length > 0) {
      this._woService.uploadFile(files)
        .subscribe((response: any) => {

          if (response.status === 200 || response.status === 206) {
            for (let i = 0; i < response.body.length; i++) {
              // add images to woImages for carousel
              const src = response.body[i].fullUrl;
              const caption = response.body[i].fileName;
              if (!src || !caption) {
                continue;
              }
              const album = {
                src: src,
                caption: caption,
                thumb: ''
              };
              this.workOrderImages.push(album);
              this.addAttachment(response.body[i].fullUrl, response.body[i].fileName, response.body[i].imageTakenDate, response.body[i].blobName);
            }
          }

          if (response.status === 206) {
            this._snackBar.open('Oops, there was an error, some images were not uploaded, please try again later', 'close', { duration: 1000 });
          }
        }, () => {
          this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
    }
  }
  addAttachment(fullUrl: any, description: any, imageTakenDate: any, blobName: any): void {
    const loggedUser = this._authService.currentUser;
    const attachment = this._formBuilder.group({
      id: [-1],
      fullUrl: [fullUrl],
      description: [description],
      imageTakenDate: [imageTakenDate],
      employeeId: [loggedUser.employeeId],
      blobName: [blobName]
    });
    this.woAttachments.push(attachment);
  }
  removeAttachment(index: number): void {
    const attachmentToDelete = this.woAttachments.at(index);
    this.woAttachments.removeAt(index);
    this.workOrderImages.splice(index, 1);
    // If id of deleted attachment was -1, it's necessary deleted it from azure
    if (attachmentToDelete.get('id').value === -1) {
      this._woService.deleteAttachmentByBlobName(attachmentToDelete.get('blobName').value).subscribe();
    }
  }

  // Tasks
  addTask(billable: boolean): void {
    this.taskFormDialog = this._dialog.open(WorkOrderTaskFormComponent, {
      panelClass: 'wo-task-form-dialog',
      data: {
        action: 'new',
        newWo: this.action === 'new' ? true : false,
        hideAttachments: this.action === 'new' ? true : false,
        showBilling: billable
      }
    });

    this.taskFormDialog.afterClosed()
      .subscribe((dialogResult: { form: FormGroup, files: any[] }) => {
        if (!dialogResult) {
          return;
        }

        const task = dialogResult.form.getRawValue();
        task.workOrderId = this.workOrderId;
        if (this.action === 'edit') {
          this.verifyTask(task, dialogResult.files);
        } else if (this.action === 'new') {
          task.id = this.getNextTemporaryTaskId();
          const newTask = new WorkOrderTaskCreateModel(task);
          const newGridTask = new WorkOrderTaskGridModel(task);
          this.unsavedTasks.push(newTask);
          this.workOrderTasks.push(newGridTask);
          this.tasksDataSource = new MatTableDataSource(this.workOrderTasks);
        }

      });
  }

  editTask(id: number): void {
    if (this.action === 'edit') {
      this.loading$.next(true);
      this._woService.getTask(id)
        .subscribe((data) => {
          this.loading$.next(false);

          if (!data) {
            this._snackBar.open('Error getting task info', 'close', { duration: 3000 });
            return;
          }

          const taskUpdate = new WorkOrderTaskUpdateModel(data);
          let showBilling = taskUpdate.workOrderServiceId !== null || taskUpdate.workOrderServiceCategoryId !== null;

          if (!this.viewTaskBilling) {
            showBilling = false;
          }

          this.taskFormDialog = this._dialog.open(WorkOrderTaskFormComponent, {
            panelClass: 'wo-task-form-dialog',
            data: {
              action: 'edit',
              task: taskUpdate,
              newWo: this.action === 'new' ? true : false,
              showBilling: showBilling
            }
          });

          this.taskFormDialog.afterClosed()
            .subscribe((dialogResult: { form: FormGroup, files: any[] }) => {
              if (!dialogResult) {
                return;
              }

              if (!this.allowEdit) {
                this._snackBar.open('Cannot edit task', 'close', { duration: 3000 });
                return;
              }

              this.verifyUpdateTask(dialogResult.form.getRawValue(), dialogResult.files);

            });

        }, () => {
          this.loading$.next(false);
          this._snackBar.open('Error getting task info', 'close', { duration: 3000 });
        });
    } else if (this.action === 'new') {
      const index = this.unsavedTasks.findIndex(t => t.id === id);
      if (index >= 0) {
        const taskUpdate = new WorkOrderTaskUpdateModel(this.unsavedTasks[index]);

        this.taskFormDialog = this._dialog.open(WorkOrderTaskFormComponent, {
          panelClass: 'wo-task-form-dialog',
          data: {
            action: 'edit',
            task: taskUpdate,
            newWo: true,
            hideAttachments: true
          }
        });

        this.taskFormDialog.afterClosed()
          .subscribe((dialogResult: { form: FormGroup, files: any[] }) => {
            if (!dialogResult) {
              return;
            }

            if (!this.allowEdit) {
              this._snackBar.open('Cannot edit task', 'close', { duration: 3000 });
              return;
            }

            const isCompleted = this.workOrderTasks[index].isComplete;
            this.unsavedTasks[index] = new WorkOrderTaskCreateModel(dialogResult.form.getRawValue());
            this.unsavedTasks[index].isComplete = isCompleted;
            this.workOrderTasks[index] = new WorkOrderTaskGridModel(dialogResult.form.getRawValue());
            this.workOrderTasks[index].isComplete = isCompleted;

            this.tasksDataSource = new MatTableDataSource(this.workOrderTasks);

          });
      } else {
        this._snackBar.open('Error getting task info', 'close', { duration: 3000 });
      }
    }
  }
  verifyTask(task, files: { description: string, fileName: string, file: File }[]): void {
    this.loading$.next(true);
    if (files.length > 0) {
      const uploadFiles: File[] = [];
      files.forEach(f => {
        uploadFiles.push(f.file);
      });

      this._woService.uploadFile(uploadFiles)
        .subscribe((response: any) => {
          if (response.status === 200 || response.status === 206) {
            for (let i = 0; i < response.body.length; i++) {
              const fileName = response.body[i].fileName;
              const description = files.find(f => f.fileName === fileName).description;

              const attachemnt = new WorkOrderTaskAttachmentModel({
                id: 0,
                description: description,
                blobName: response.body[i].blobName,
                fullUrl: response.body[i].fullUrl,
                title: fileName,
                imageTakenDate: response.body[i].imageTakenDate,
                workOrderTaskId: 0
              });
              task.attachments.push(attachemnt);
            }

            this.createTask(task);
          }
        }, () => {
          this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

    } else {
      this.createTask(task);
    }
  }
  createTask(task): void {
    this._woService.create(task, 'AddTask')
      .subscribe(() => {
        this.loading$.next(false);
        this.readAllTasks();
      }, () => {
        this.loading$.next(false);
        this.readAllTasks();
      });
  }
  verifyUpdateTask(task, files: { description: string, fileName: string, file: File }[]): void {
    this.loading$.next(true);
    if (files.length > 0) {
      const uploadFiles: File[] = [];
      files.forEach(f => {
        uploadFiles.push(f.file);
      });

      this._woService.uploadFile(uploadFiles)
        .subscribe((response: any) => {
          if (response.status === 200 || response.status === 206) {
            for (let i = 0; i < response.body.length; i++) {
              const fileName = response.body[i].fileName;
              const description = files.find(f => f.fileName === fileName).description;

              const attachemnt = new WorkOrderTaskAttachmentModel({
                id: 0,
                description: description,
                blobName: response.body[i].blobName,
                fullUrl: response.body[i].fullUrl,
                title: fileName,
                imageTakenDate: response.body[i].imageTakenDate,
                workOrderTaskId: 0
              });
              task.attachments.push(attachemnt);
            }

            this.updateTask(task);
          }
        }, () => {
          this.loading$.next(false);
          this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
    } else {
      this.updateTask(task);
    }
  }
  updateTask(task): void {
    this._woService.update(task, 'UpdateTask')
      .subscribe(() => {
        this.loading$.next(false);
        this._snackBar.open('Work Order task updated successfully', 'close', { duration: 3000 });
        this.readAllTasks();
      }, () => {
        this.loading$.next(false);
        this._snackBar.open('Error updating task', 'close', { duration: 3000 });
      });
  }
  removeTask(id: number): void {
    this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialog.componentInstance.confirmMessage = 'Are you sure you want to delete this Work Order Task?';

    this.confirmDialog.afterClosed().subscribe(result => {
      if (result) {

        if (this.action === 'new') {
          const index = this.unsavedTasks.findIndex(t => t.id === id);
          if (index >= 0) {
            this.unsavedTasks.splice(index, 1);
            this.workOrderTasks.splice(index, 1);
          }
          this.tasksDataSource = new MatTableDataSource(this.workOrderTasks);
        } else if (this.action === 'edit') {
          this.loading$.next(true);
          this._woService.deleteTask(id)
            .subscribe(() => {
              this.loading$.next(false);
              this._snackBar.open('Task deleted successfully', 'close', { duration: 3000 });
              this.readAllTasks();
            }, () => {
              this.loading$.next(false);
              this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
            });
        }

      }
      this.confirmDialog = null;
    });
  }
  readAllTasks(): void {
    this.loadingTasks = true;
    this._woService.getAllTasks(this.workOrderId)
      .subscribe((result: WorkOrderTaskGridModel[]) => {
        this.loadingTasks = false;
        this.workOrderTasks = result;
        this.tasksDataSource = new MatTableDataSource(result);
      }, () => {
        this.loadingTasks = false;
      });
  }
  countTasksCompleted(): number {
    return this.workOrderTasks.filter(el => el.isComplete).length;
  }
  completedTaskChanged(checkEvent: MatCheckboxChange): void {
    event.stopPropagation();

    const taskId = Number(checkEvent.source.value);

    if (checkEvent.checked) {
      const index = this.workOrderTasks.findIndex(t => t.id === taskId);
      if (index !== null) {
        const task = this.workOrderTasks[index];
        // this.updateWorkOrderTaskStatus(taskId, true, 0, 0, new Date());
        this.closeWorkOrderTaskDialog = this._dialog.open(WoTaskFormConfirmCloseComponent, {
          panelClass: 'close-work-order-task-dialog',
          data: {
            hoursRequired: task.hoursRequiredAtClose,
            quantityRequired: task.quantityRequiredAtClose,
            hours: task.hoursExecuted,
            quantity: task.quantityExecuted
          }
        });
        this.closeWorkOrderTaskDialog.afterClosed()
          .subscribe((dialogResult: FormGroup) => {
            if (!dialogResult) {
              this.readAllTasks();
              return;
            }
            const formObject = dialogResult.getRawValue();
            this.updateWorkOrderTaskStatus(taskId, true, formObject.quantity, formObject.hours, formObject.completedDate);
          });
      }
    } else {
      this.updateWorkOrderTaskStatus(taskId, false);
    }
  }
  updateWorkOrderTaskStatus(id: number, status: boolean, quantityExecuted: number = 0, hoursExecuted: number = 0, completedDate: Date = null): void {
    if (this.action === 'new') {
      const index = this.workOrderTasks.findIndex(t => t.id === id);
      if (index >= 0) {
        this.workOrderTasks[index].isComplete = status;

        this.unsavedTasks[index].isComplete = status;
        this.unsavedTasks[index].quantityExecuted = quantityExecuted;
        this.unsavedTasks[index].hoursExecuted = hoursExecuted;
      }
    } else if (this.action === 'edit') {
      this.loading$.next(true);
      this._woService.updateCompleteTaskStatus(id, status, quantityExecuted, hoursExecuted, completedDate)
        .subscribe(() => {
          this.loading$.next(false);
          this.readAllTasks();
        }, () => {
          this.loading$.next(false);
        });
    }
  }

  getNextTemporaryTaskId(): number {
    let max = 0;
    this.workOrderTasks.forEach(t => {
      if (t.id >= max) {
        max = t.id;
      }
    });
    return max + 1;
  }

  // Billing

  // Notes
  get woNotes(): FormArray {
    return this.workOrderForm.get('notes') as FormArray;
  }
  addNote(textNote: any): void {
    const loggedUser = this._authService.currentUser;
    const note = this.createNote(textNote, loggedUser.employeeId, loggedUser.employeeFullName);
    this.woNotes.push(note);
    this.textNote = '';
  }
  createNote(textNote: any, employeeId: any, employeeFullName: any): FormGroup {
    const now = new Date();
    return this._formBuilder.group({
      note: [textNote],
      employeeId: [employeeId],
      employeeFullName: [employeeFullName],
      createdDate: [now],
      // HACK: Forcing UTC date
      epochCreatedDate: [Math.floor(now.getTime() / 1000) + now.getTimezoneOffset() * 60],
    });
  }

  // Schedule
  getMonths(): void {
    // tslint:disable-next-line: forin
    for (const month in CALENDAR_MONTH) {
      if (typeof CALENDAR_MONTH[month] === 'number') {
        this.scheduleMonths.push({ id: CALENDAR_MONTH[month] as any, name: month });
      }
    }
  }
  checkDay(event: MatCheckboxChange, day: number): void {
    const index = this.scheduleDays.findIndex(d => d.id === day);
    if (index >= 0) {
      this.scheduleDays[index].isSelected = event.checked;
    }
  }
  scheduleStartDateChanged(event: MatDatepickerInputEvent<Date>): void {
    this.scheduleStartDate = event.value;
    if (this.selectedScheduleFrequency > 2 && this.selectedScheduleFrequency < 6) {
      this.calculateMonthlyScheduleDates();
    }
  }
  scheduleEndDateChanged(event: MatDatepickerInputEvent<Date>): void {
    this.scheduleEndDate = event.value;
    if (this.selectedScheduleFrequency > 2 && this.selectedScheduleFrequency < 6) {
      this.calculateMonthlyScheduleDates();
    }
  }
  scheduleDateChanged(event: MatDatepickerInputEvent<Date>): void {
    this.scheduleDate = event.value;
  }
  scheduleUpdateDateChanged(event: MatDatepickerInputEvent<Date>): void {
    if (this.unscheduledStatus) {
      const newDate = new Date(event.value);

      this.workOrderForm.patchValue({
        scheduleDate: newDate,
        unscheduled: false
      });

      this.unscheduledStatus = false;
    }
  }
  scheduleFrequencyChanged(event: MatSelectChange): void {
    this.scheduleDates = [];
    if (event.value > 2 && event.value < 6) {
      this.scheduleEndDate = null;
    } else {
      this.scheduleEndDate = new Date(this.scheduleStartDate.getFullYear(), this.scheduleStartDate.getMonth(), this.scheduleStartDate.getDate() + 1);
    }

    // no settings
    if (event.value === 7) {
      if (this.workOrderForm.get('scheduleSettings')) {
        this.workOrderForm.removeControl('scheduleSettings');
      }
    }
  }
  removeScheduleDate(index: number): void {
    if (index === 0) {
      return;
    }
    this.excludedScheduleDates.push(new Date(this.scheduleDates[index].getFullYear(), this.scheduleDates[index].getMonth(), 1));
    this.scheduleDates.splice(index, 1);
  }
  revoveScheduleDateFrequency(): void {
    this.scheduleDate = null;
  }
  calculateMonthlyScheduleDates(): void {
    this.scheduleDates = [];
    this.excludedScheduleDates = [];
    if (!this.scheduleStartDate) {
      return;
    }

    let period = 12;

    if (this.selectedScheduleFrequency === 3) {
      // quarterly
      period = 3;
    } else if (this.selectedScheduleFrequency === 4) {
      // semi anually
      period = 6;
    }

    try {
      if (!this.scheduleEndDate) {
        if (isNaN(Number(this.scheduleQuantityCtrl.value))) {
          return;
        }
        const quantity = Number(this.scheduleQuantityCtrl.value);
        const curentMonth = new Date(this.scheduleStartDate.getFullYear(), this.scheduleStartDate.getMonth(), 1);
        while (this.scheduleDates.length < quantity) {
          this.scheduleDates.push(new Date(curentMonth.getFullYear(), curentMonth.getMonth(), 1));
          curentMonth.setMonth(curentMonth.getMonth() + period);
        }
        this.scheduleEndDate = new Date(curentMonth.getFullYear(), curentMonth.getMonth() - 1, 1);
      } else if (this.scheduleEndDate) {
        const curentMonth = new Date(this.scheduleStartDate.getFullYear(), this.scheduleStartDate.getMonth(), 1);
        while (curentMonth <= this.scheduleEndDate) {
          this.scheduleDates.push(new Date(curentMonth.getFullYear(), curentMonth.getMonth(), 1));

          curentMonth.setMonth(curentMonth.getMonth() + period);
        }
      }
    } catch (error) {
      console.log(error);
    }
  }
  calculateWeeklyScheduleDates(): void {
    if (isNaN(Number(this.scheduleQuantityCtrl.value))) {
      return;
    }
    if (this.scheduleDays.filter(d => d.isSelected).length === 0) {
      this._snackBar.open('Select at least one day in schedule settings', 'close', { duration: 1000 });
      return;
    }

    const quantity = Number(this.scheduleQuantityCtrl.value);
    let total = 0;

    const currentDate = new Date(this.scheduleStartDate.getFullYear(), this.scheduleStartDate.getMonth(), this.scheduleStartDate.getDate());
    while (total < quantity) {
      const dayWeek = currentDate.getDay();
      const match = this.scheduleDays.find(d => d.id === dayWeek && d.isSelected);
      if (match) {
        total++;
      }
      currentDate.setDate(currentDate.getDate() + 1);
    }

    this.scheduleEndDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate());
  }

  // Work Order
  getWorkOrder(): void {
    this.loading$.next(true);
    this._woService.get(this.workOrderId, 'update')
      .subscribe((workOrderData: any) => {
        this.loading$.next(false);
        if (workOrderData) {
          this.workOrder = new WorkOrderUpdateModel(workOrderData);

          if (this.workOrder.dueDate) {
            this.dueDate = this.convertUTCToLocalTime(this.workOrder.dueDate, this.workOrder.epochDueDate);
          }
          if (this.workOrder.scheduleDate) {
            this.workOrder.scheduleDate = this.convertUTCToLocalTime(this.workOrder.scheduleDate, this.workOrder.epochScheduleDate);
          }
          this.woStatus = this.workOrder.statusId;
          this.buildingSelected = this.workOrder.buildingId;
          this.unscheduledStatus = this.workOrder.unscheduled;

          this.unsavedTasks = [];
          this.addedEmployees = [];
          this.workOrderEmployees = [];
          this.workOrderForm.get('tasks').setValue([]);
          this.workOrderForm.get('assignedEmployees').setValue([]);
          if (this.workOrder.employees) {
            this.workOrder.employees.forEach(employee => {
              switch (employee.level) {
                // Supervisor
                case 40:
                  employee.type = 1;
                  break;

                // Operations Manager
                case 30:
                  employee.type = 2;
                  break;

                // Temporary Operations Manager
                case 35:
                  employee.type = 4;
                  break;

                // Inspecto
                default:
                  employee.type = 8;
                  break;
              }
              this.addedEmployees.push(employee);
              this.workOrderEmployees.push(employee);
            });

            this.verifySelectedBuildingEmployees();
          }

          // load selected building
          this.getBuildings('');

          this.checkForAttachments();
          this.checkForAttachmentsToAlbum();
          this.checkForBilling();
          this.checkForSchedule();
          this.checkForNotifications();
          this.checkForNotes();

          this.updateWorkOrderForm();

          this.workOrderForm.removeControl('sendNotifications');

          if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
            this.workOrderForm.disable();
            this.allowEdit = false;
          }

          this.typeChanged({ value: this.workOrder.type, source: null });

          this.readAllTasks();
        }
      }, () => {
        this.loading$.next(false);
      });
  }

  // Util
  convertUTCToLocalTime(dateToValidate: any, epochDate: number): any {
    const possibleDate: any = new Date(dateToValidate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return null;
    }
    else {
      return new Date(this._epochPipe.transform(epochDate));
    }
  }
  displayInformationDialog(dialogTitle: any, message: any): void {
    this.messageDialog = this._dialog.open(MessageDialogComponent, {
      disableClose: false,
    });
    this.messageDialog.componentInstance.dialogTitle = dialogTitle;
    this.messageDialog.componentInstance.message = message;
    this.messageDialog.afterClosed().subscribe(() => {
      return;
    });
  }

  // Buttons
  submit(): void {
    // set employees to array
    this.workOrderForm.get('assignedEmployees').setValue(this.workOrderEmployees);

    if (this.woStatus === 0) {
      this.workOrderForm.get('statusId').setValue(1);
    }

    if (!this.workOrderForm.controls['defineDate'].value) {
      this.workOrderForm.patchValue({
        snoozeDate: null,
        epochSnoozeDate: 0
      });
    }

    const selectedType = this.workOrderForm.get('type').value;
    const schedulingTaskCount = this.workOrderTasks.filter(t => t.requiresScheduling).length;
    let validWorkOrderType = true;

    if (schedulingTaskCount > 0) {
      validWorkOrderType = selectedType === 4 ? true : false;
    }

    if (!validWorkOrderType) {
      this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
        disableClose: false
      });
      this.confirmDialog.componentInstance.confirmMessage = 'Some of the tasks associated to this Work Order require scheduling, do you want to continue without scheduling?';

      this.confirmDialog.afterClosed().subscribe(result => {
        if (result) {
          this.validateUpdateStatusWorkOrder();
        }
      });
    } else {
      this.validateUpdateStatusWorkOrder();
    }
  }
  validateUpdateStatusWorkOrder(): void {
    // Closed status
    if (this.woStatus === 3) {
      const today = new Date();

      if (this.workOrderDueDate.value > today) {
        this.displayInformationDialog('WO Not Yet Due!',
          'This Work Order cannot be closed because it is not due yet. Please wait until its Due Date to change its status to Closed.');
      }
      else if (this.countTasksCompleted() !== this.workOrderTasks.length) {
        this.displayInformationDialog('WO with uncompleted tasks!',
          'This Work Order cannot be closed because it has uncompleted tasks.');
      } else {
        this.workOrderConfirmDialog = this._dialog.open(WoConfirmDialogComponent, {
          disableClose: false,
          data: {
            roleLevelLoggedUser: this.roleLevelLoggedUser
          }
        });

        this.workOrderConfirmDialog.componentInstance.confirmMessage = 'Are you sure you want to close this Work Order?' + ' ' +
          'Feel free to use the field below to add any Closing Notes if necessary.';

        this.workOrderConfirmDialog.afterClosed().subscribe(result => {
          if (result[0]) {
            const formData = result[1].getRawValue();

            this.workOrderForm.get('closingNotes').setValue(formData.closingNotes);
            this.workOrderForm.get('additionalNotes').setValue(formData.additionalNotes);
            this.workOrderForm.get('closingNotesOther').setValue(formData.closingNotesOther);
            this.workOrderForm.get('followUpOnClosingNotes').setValue(formData.followUpOnClosingNotes);

            if (this.countTasksCompleted() < this.workOrderTasks.length) {
              // Mark to complete all tasks
            }

            // --UPDATE -- //
            this.updateWorkOrder();
            // --UPDATE -- //
          }
        });
      }
    } else {
      // Cancelled status
      if (this.woStatus === 4) {
        this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
          disableClose: false
        });

        this.confirmDialog.componentInstance.confirmMessage = 'Are you sure you want to cancel this Work Order?';

        this.confirmDialog.afterClosed().subscribe(result => {
          if (result) {
            // -- UPDATE -- //
            this.workOrderForm.get('additionalNotes').setValue(0);
            this.updateWorkOrder();
            // -- UPDATE -- //
          }
          this.confirmDialog = null;
        });
      } else {
        // -- UPDATE -- //
        this.workOrderForm.get('additionalNotes').setValue(0);
        this.updateWorkOrder();
        // -- UPDATE -- //
      }
    }
  }
  updateWorkOrder(): void {
    this.loading$.next(true);
    // set employees to array
    this.workOrderForm.get('assignedEmployees').setValue(this.workOrderEmployees);

    const updatedWorkOrderObj = new WorkOrderUpdateModel(this.workOrderForm.getRawValue());
    updatedWorkOrderObj.updateTasks = false;
    this._woService.updateElement(updatedWorkOrderObj)
      .then(
        () => {
          this.loading$.next(false);
          this._snackBar.open('Work Order updated successfully!!!', 'close', { duration: 1000 });
          this.getWorkOrder();
        },
        () => {
          this.loading$.next(false);
          this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        })
      .catch(() => {
        this.loading$.next(false);
        this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  // Button click
  saveWorkOrder(): void {

    // schedule settings selected
    if (this.selectedScheduleFrequency !== 7) {
      const selectedDays: number[] = [];
      this.scheduleDays.forEach(d => {
        if (d.isSelected) {
          selectedDays.push(d.id);
        }
      });

      if (this.selectedScheduleFrequency === 0) {
        if (!this.scheduleStartDate || !this.scheduleEndDate) {
          this._snackBar.open('Invalid dates on schedule the settings', 'close', { duration: 1000 });
          return;
        }
      } else if (this.selectedScheduleFrequency === 1) {
        if (!this.scheduleStartDate) {
          this._snackBar.open('Invalid start date on schedule the settings', 'close', { duration: 1000 });
          return;
        }
        if (!this.scheduleEndDate) {
          this._snackBar.open('Invalid end date on schedule the settings', 'close', { duration: 1000 });
          return;
        }
        if (selectedDays.length === 0) {
          this._snackBar.open('Select at least one day in schedule settings', 'close', { duration: 1000 });
          return;
        }
      } else if (this.selectedScheduleFrequency === 2) {
        if (selectedDays.length === 0) {
          this._snackBar.open('Select at least one day in schedule settings', 'close', { duration: 1000 });
          return;
        }
        if (selectedDays.length > 1) {
          this._snackBar.open('Select only one day in schedule settings', 'close', { duration: 1000 });
          return;
        }
      } else if (this.selectedScheduleFrequency > 2 && this.selectedScheduleFrequency < 6) {
        if (!this.scheduleStartDate || !this.scheduleEndDate) {
          this._snackBar.open('Invalid dates on schedule the settings', 'close', { duration: 1000 });
          return;
        }

        // if (!this.scheduleDate) {
        //   this.snackBar.open('Invalid schedule date on schedule the settings', 'close', { duration: 1000 });
        //   return;
        // }
      } else if (this.selectedScheduleFrequency === 6) {
        if (!this.scheduleDate) {
          this._snackBar.open('Invalid dates on schedule the settings', 'close', { duration: 1000 });
          return;
        }
      }

      const startValue = this.selectedScheduleFrequency === 2 ? this.scheduleStartMonth : this.selectedScheduleMonth;
      const endValue = this.selectedScheduleFrequency === 2 ? this.scheduleEndMonth : this.scheduleStartYear;

      this.workOrderForm.setControl('scheduleSettings',
        this._formBuilder.group({
          workOrderId: [0],
          frequency: [this.selectedScheduleFrequency],
          startDate: [this.scheduleStartDate],
          endDate: [this.scheduleEndDate],
          ordinal: [this.selectedScheduleOrdinal],
          startValue: [startValue],
          endValue: [endValue],
          days: this._formBuilder.array(selectedDays),
          scheduleDate: [this.scheduleDate],
          excludedScheduleDates: this._formBuilder.array(this.excludedScheduleDates)
        }));
    }

    this.workOrderForm.patchValue({
      sendNotifications: this.sendNotifications
    });

    const selectedType = this.workOrderForm.get('type').value;
    const schedulingTaskCount = this.workOrderTasks.filter(t => t.requiresScheduling).length;
    const invalidWorkOrderScheduling = schedulingTaskCount > 0 && this.selectedScheduleFrequency === 7;
    let validWorkOrderType = true;

    if (schedulingTaskCount > 0) {
      validWorkOrderType = selectedType === 4 ? true : false;
    }

    if (invalidWorkOrderScheduling || !validWorkOrderType) {
      this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
        disableClose: false
      });

      let message = '';

      if (invalidWorkOrderScheduling && !validWorkOrderType) { message = 'Some tasks require scheduling and require the Specialist type of the Work Order, do you want to continue the saving process?'; }
      else if (invalidWorkOrderScheduling) { message = 'Some tasks require scheduling, do you want to continue the saving process?'; }
      else if (!validWorkOrderType) { message = 'Some of the tasks associated to this Work Order require scheduling, do you want to continue without scheduling?'; }

      this.confirmDialog.componentInstance.confirmMessage = message;

      this.confirmDialog.afterClosed().subscribe(result => {
        if (result) {
          this.createWorkOrder();
        }
      });
    } else {
      this.createWorkOrder();
    }
  }

  createWorkOrder(): void {
    const setStatusByStandBy: boolean = this.workOrderForm.get('setStatusByStandBy').value;

    const workorderObj = new WorkOrderCreateModel(this.workOrderForm.getRawValue());
    workorderObj.sourceCode = this.woSources.find(w => w.id === workorderObj.workOrderSourceId).code;
    this.unsavedTasks.forEach(t => { t.id = 0; });
    workorderObj.tasks = this.unsavedTasks;
    const scheduleSettings = workorderObj.scheduleSettings ? new WorkOrderScheduleSetting(workorderObj.scheduleSettings) : null;
    if (scheduleSettings) {
      try {
        // Create multiple work Order
        this.loading$.next(true);
        this._woService.create(scheduleSettings, 'AddScheduleSettings')
          .subscribe((settingsResult: any) => {
            const settingsId = settingsResult['body'].id;
            workorderObj.workOrderScheduleSettingId = settingsId;
            let dates: Date[] = [];
            dates = this._woService.calculateScheduleDates(scheduleSettings);

            if (dates.length > 0) {
              const workOrders: any[] = [];
              let index = 0;

              dates.forEach(d => {
                let unscheduled = false;
                if (!scheduleSettings.scheduleDate && (scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6)) {
                  unscheduled = true;
                } else if (index > 0 && (scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6)) {
                  unscheduled = true;
                }

                const newWo = new WorkOrderCreateModel(workorderObj);
                newWo.dueDate = setStatusByStandBy ? d : null;
                newWo.statusId = setStatusByStandBy ? 1 : 0;
                newWo.scheduleDate = d;
                newWo.unscheduled = setStatusByStandBy ? false : unscheduled;
                workOrders.push(newWo);
                index++;
              });
              this.saveWorkOrders(workOrders, settingsId);
            } else {
              this.loading$.next(false);
            }
          }, () => {
            this.loading$.next(false);
            this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      } catch (error) { console.log(error); }

    } else {

      // Create single work Order
      this.loading$.next(true);
      this._woService.createElement(workorderObj)
        .then(
          (response) => {
            const newWorkOrderId = Number(response['body'].id);

            this._route.navigate(
              [],
              {
                relativeTo: this._router,
                queryParams: { action: 'edit', workorder: newWorkOrderId.toString() }
              });

            this._snackBar.open('Service orders created successfully!!!', 'close', { duration: 1000 });
          },
          () => {
            this.loading$.next(false);
            this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          })
        .catch(() => {
          this.loading$.next(false);
          this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

    }
  }

  async saveWorkOrders(workOrders: any[], scheduleSettingId): Promise<void> {
    let workOrderId = 0;
    for (let index = 0; index < workOrders.length; index++) {
      try {
        const result = await this._woService.createFromCalendar(workOrders[index]).toPromise();
        if (result && index === 0) {
          if (result.id) {
            workOrderId = result.id;
          }
        }
      } catch (error) {
      }
    }

    this.loading$.next(false);
    if (workOrders.length > 1) {
      this.displayWorkOrderSequence(scheduleSettingId, workOrderId);
    } else if (workOrders.length === 1) {
      this._route.navigate(
        [],
        {
          relativeTo: this._router,
          queryParams: { action: 'edit', workorder: workOrderId.toString() }
        });
    }
  }
  displayWorkOrderSequence(calendarItemFrequencyId: number, woId: number): void {
    this.workOrderSequencesDialog = this._dialog.open(WorkOrderSequencesDialogComponent, {
      panelClass: 'work-order-sequences-dialog',
      data: {
        calendarItemFrequencyId: calendarItemFrequencyId
      }
    });
    this.workOrderSequencesDialog.afterClosed().subscribe(() => {
      this._route.navigate(
        [],
        {
          relativeTo: this._router,
          queryParams: { action: 'edit', workorder: woId.toString() }
        });
    });
  }

  // Lightbox
  checkForAttachmentsToAlbum(): void {
    const attachments = this.workOrder.attachments;
    if (attachments.length > 0) {
      // Clean images
      this.lightboxAlbum = [];

      // add images to woImages for carousel
      attachments.forEach(attachment => {
        const src = attachment.fullUrl;
        const caption = attachment.description || 'No title';
        const album = {
          src: src,
          caption: caption,
          thumb: ''
        };
        this.lightboxAlbum.push(album);
      });
    }
  }
  openAlbumImage(attatchmentId: number): void {
    this._lightboxSubscription = this._lightboxEvent.lightboxEvent$.subscribe((event: IEvent) => this._onReceivedLightboxEvent(event));

    const index = this.workOrder.attachments.findIndex(el => el.id === attatchmentId);
    console.log(index);
    if (index >= 0) {
      this._lightbox.open(this.lightboxAlbum, index, {
        wrapAround: true,
        showImageNumberLabel: true,
        disableScrolling: true
      });
    }
  }
  private _onReceivedLightboxEvent(event: IEvent): void {
    if (event.id === LIGHTBOX_EVENT.CLOSE) {
      this._lightboxSubscription.unsubscribe();
    }
  }

  // Permissions
  updateViewPermissions(): void {
    if (this._permissionService.permissions.length > 0) {
      const UpdateWorkOrders = this._permissionService.permissions.find(p => p.name === 'UpdateWorkOrders');
      const AddWorkOrdersNotes = this._permissionService.permissions.find(p => p.name === 'AddWorkOrdersNotes');
      const DeleteWorkOrdersAttachment = this._permissionService.permissions.find(p => p.name === 'DeleteWorkOrdersAttachment');

      const ReadWorkOrderTaskBillingInformationFromDetail = this._permissionService.permissions.find(p => p.name === 'ReadWorkOrderTaskBillingInformationFromDetail');
      const AddWorkOrdersTasks = this._permissionService.permissions.find(p => p.name === 'AddWorkOrdersTasks');
      const DeleteWorkOrdersTasks = this._permissionService.permissions.find(p => p.name === 'DeleteWorkOrdersTasks');

      this.viewTaskBilling = ReadWorkOrderTaskBillingInformationFromDetail ? ReadWorkOrderTaskBillingInformationFromDetail.isAssigned : false;
      this.btnAddBillableTask = this.viewTaskBilling;

      this.btnAddTask = AddWorkOrdersTasks ? AddWorkOrdersTasks.isAssigned : false;
      this.btnAddBillableTask = this.btnAddTask && this.viewTaskBilling;
      this.btnDeleteTask = DeleteWorkOrdersTasks ? DeleteWorkOrdersTasks.isAssigned : false;

      this.btnUpdateWorkOrder = UpdateWorkOrders ? UpdateWorkOrders.isAssigned : false;

      this.btnAddWorkOrderNote = AddWorkOrdersNotes ? AddWorkOrdersNotes.isAssigned : false;
      this.btnDeleteWorkOrderAttachment = DeleteWorkOrdersAttachment ? DeleteWorkOrdersAttachment.isAssigned : false;
    }
  }

}
