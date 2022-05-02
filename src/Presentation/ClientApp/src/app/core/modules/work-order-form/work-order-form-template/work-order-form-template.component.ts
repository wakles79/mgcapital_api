import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup, AbstractControl, FormControl, FormBuilder, Validators, FormArray } from '@angular/forms';
import { CalendarEvent } from 'angular-calendar';
import { Subject, Subscription } from 'rxjs';
import { WORK_ORDER_TYPES } from '../../../models/work-order/work-order-type.model';
import { WOSBillingDateTypeMOdel } from '../../../models/work-order/wo-billing-date-type.model';
import { DatePipe } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { WorkOrdersService } from '@app/main/content/private/work-orders/work-orders.service';
import { BuildingsService } from '@app/main/content/private/buildings/buildings.service';
import { ContactsService } from '@app/main/content/private/contacts/contacts.service';
import { AuthService } from '@app/core/services/auth.service';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { ScheduleSettingsCategoryService } from '@app/main/content/private/schedule-settings-category/schedule-settings-category.service';
import { UsersBaseService } from '@app/main/content/private/users/users-base.service';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ListScheduleCategoryModel } from '../../../models/schedule-category/list-schedule-category.model';
import { ScheduleSubcategoryBaseModel } from '@app/core/models/schedule-subcategory/schedule-subcategory-base.model';
import { ContactGridModel } from '../../../models/contact/contact-grid.model';
import { ListWOSourcesModel } from '../../../models/work-order/list-wo-sources.model';
import { WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { PreCalendarBaseModel } from '../../../models/pre-calendar/pre-calendar-base.model';
import { Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT, IEvent, IAlbum } from 'ngx-lightbox';
import { WorkOrderTaskFormComponent } from '../work-order-task-form/work-order-task-form.component';
import { WORK_ORDERS_PRIORITIES } from '../../../models/work-order/work-order-priorities.model';
import { ListBuildingModel } from '../../../models/building/list-buildings.model';
import { debounceTime, takeUntil } from 'rxjs/operators';
import { WorkOrderTaskModel } from '@app/core/models/work-order/work-order-task.model';
import { WoTaskBillingFormComponent } from '../wo-task-billing-form/wo-task-billing-form.component';
import { WorkOrderTaskUpdateModel } from '../../../models/work-order-task/work-order-task-update.model';

import { WoTaskFormConfirmCloseComponent } from '../../work-order-dialog/wo-task-form-confirm-close/wo-task-form-confirm-close.component';
import { WoConfirmDialogComponent } from '../../work-order-dialog/wo-confirm-dialog/wo-confirm-dialog.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { MessageDialogComponent } from '../../message-dialog/message-dialog/message-dialog.component';
import { CALENDAR_MONTH } from '@app/core/models/calendar/calendar-periodicity-enum';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatSelectChange } from '@angular/material/select';
import { DynamicComponent } from '@app/core/models/ticket/ticketDestination';

@Component({
  selector: 'app-work-order-form-template',
  templateUrl: './work-order-form-template.component.html',
  styleUrls: ['./work-order-form-template.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class WorkOrderFormTemplateComponent implements OnInit, OnDestroy, DynamicComponent {

  event: CalendarEvent;
  dialogTitle: string;
  workOrderForm: FormGroup;
  action: string;

  @Input() data: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  onWorkOrderTemplateClosed: Subject<any> = new Subject<any>();
  onWorkOrderTemplateSubmitted: Subject<any> = new Subject<any>();
  // wildcard to disable the form from a component parent
  buttonSaveDisabled = false;

  workOdersPriorities: { id: number, name: string }[] = [];
  workOrderTypes: { key: number, value: string }[] = WORK_ORDER_TYPES;
  workBillingType: { key: number, value: string }[] = WOSBillingDateTypeMOdel;
  roleLevelLoggedUser: any;

  workOrder: any;
  workOrderNumber: number;
  dueDate: Date = null;

  // To indicate if the work order will be create as a clone and show a toggle to indicate if the reference (oringinWorkOrderId) must be saved
  workOrderfromClone: any;
  keepOriginWoReference = true;

  // To Indicate when send notifications
  sendNotifications: boolean;

  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  buildingSelected = 0;

  get buildingCtrl(): AbstractControl {
    return this.workOrderForm.get('buildingCtrl');
  }

  scheduleStartDate: Date = new Date();
  scheduleDate: Date = null;
  excludedScheduleDates: Date[] = [];
  scheduleDates: Date[] = [];
  scheduleQuantityCtrl: FormControl;
  scheduleEndDate: Date = new Date(this.scheduleStartDate.getFullYear(), this.scheduleStartDate.getMonth(), this.scheduleStartDate.getDate() + 1);
  selectedScheduleFrequency: number = 7;
  scheduleFrequency: { id: number; name: string }[] = [
    { id: 0, name: 'Daily' },
    { id: 1, name: 'Weekly' },
    { id: 2, name: 'Monthly' },
    { id: 3, name: 'Quarterly' },
    { id: 4, name: 'Semi-Annually' },
    { id: 5, name: 'Annually' },
    { id: 6, name: 'Does not repeat' },
    { id: 7, name: 'None' }
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
  selectedScheduleOrdinal: number = 1;
  scheduleOrdinal: ListItemModel[] = [
    { id: 1, name: 'First' },
    { id: 2, name: 'Second' },
    { id: 3, name: 'Third' },
    { id: 4, name: 'Last' }
  ];
  scheduleStartMonth = 1;
  scheduleEndMonth = 1;
  selectedScheduleMonth: number = 1;
  scheduleStartYear: number;
  scheduleMonths: ListItemModel[] = [];

  scheduleCategories: { id: number, description: string }[] = [];
  filteredscheduleCategories$: Subject<ListScheduleCategoryModel[]> = new Subject<ListScheduleCategoryModel[]>();
  scheduleCategorySelected = 0;

  get scheduleCategoryCtrl(): AbstractControl {
    return this.workOrderForm.get('scheduleCategoryCtrl');
  }

  scheduleSubCategories: ScheduleSubcategoryBaseModel[] = [];
  filteredscheduleSubCategories$: Subject<ScheduleSubcategoryBaseModel[]> = new Subject<ScheduleSubcategoryBaseModel[]>();
  scheduleSubCategorySelected = 0;

  get scheduleSubCategoryCtrl(): AbstractControl {
    return this.workOrderForm.get('scheduleSubCategoryCtrl');
  }

  contactsRequester: ContactGridModel[] = [];
  requesterSelected: number;
  preRequesterSelected: any;

  addCheckList: boolean = true;
  taskDescription: any;

  addNotes: boolean = true;
  showScheduleSettings: boolean = true;

  textNote: any;

  setExtNotifications: boolean = true;

  showAttachments: boolean = true;
  woImages: Array<IAlbum> = [];

  showBillingInformation: boolean = true;
  woStatus: any;
  woSources: ListWOSourcesModel[] = [];
  workOrderSource: WorkOrderSourceCode; // = WorkOrderSourceCode.Email; MG-23
  disableSourceControl: boolean = true; // MG-121

  private _subscription: Subscription;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  confirmDialogRef: MatDialogRef<any>;
  dialogTaskBillingRef: any;

  get workOrderDueDate(): AbstractControl { return this.workOrderForm.get('dueDate'); }

  private today = new Date();

  // preCalendar
  private preCalendar: PreCalendarBaseModel;

  get showSnoozeDate(): any { return this.workOrderForm.get('defineDate').value; }

  fromCalendar = false;

  showCategoryAndSubCategory: boolean;

  unscheduledStatus: boolean = false;

  showCloseButton: boolean = true;

  workOrderStatus: { id: number, name: string, disabled: boolean }[] = [];

  // employees
  employees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  availableEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  buildingEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  addedEmployees: { id: number, name: string, roleName: string, email: string, type: number }[] = [];
  get selectedEmployees(): AbstractControl { return this.workOrderForm.get('assignedEmployees'); }

  // tasks
  taskFormDialog: MatDialogRef<WorkOrderTaskFormComponent>;
  closeWorkOrderTaskDialog: MatDialogRef<WoTaskFormConfirmCloseComponent>;

  constructor(
    private datePipe: DatePipe,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private woService: WorkOrdersService,
    private buildingService: BuildingsService,
    private contactService: ContactsService,
    private authService: AuthService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig,
    private epochPipe: FromEpochPipe,
    private schedueSettingsCategoryService: ScheduleSettingsCategoryService,
    private _userService: UsersBaseService
  ) {
    // super();
    this.scheduleStartYear = new Date().getFullYear();
    this.scheduleQuantityCtrl = new FormControl('');
  }

  ngOnInit(): void {
    try {
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
    } catch (error) {
      console.log(error);
    }

    this.getMonths();

    this.action = this.data.action;
    this.workOrder = {};
    this.keepOriginWoReference = true;
    this.data.hasOwnProperty('workOrderFromClone') ? this.workOrderfromClone = this.data.workOrderFromClone : this.workOrderfromClone = false;
    this.data.hasOwnProperty('fromCalendar') ? this.fromCalendar = this.data.fromCalendar : this.fromCalendar = false;
    this.data.hasOwnProperty('sendNotifications') ? this.sendNotifications = this.data.sendNotifications : this.sendNotifications = true;
    this.data.hasOwnProperty('showCloseButton') ? this.showCloseButton = this.data.showCloseButton : this.showCloseButton = true;
    this.data.hasOwnProperty('source') ? this.workOrderSource = this.data.source : this.workOrderSource = WorkOrderSourceCode.Other; //EMail MG-23

    if (this.action === 'edit' || this.workOrderfromClone) {
      this.dialogTitle = 'Edit Work Order ';
      this.initData();
      if(this.workOrderfromClone)
      {
        this.workOrderSource = null;
        this.disableSourceControl = false; // MG-121
      }
      this.workOrderForm = this.createWOUpdateForm();

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
        });

        this.verifySelectedBuildingEmployees();
      }

      this.checkForTasks();
      this.checkForNotes();
      this.checkForAttachemnts();
      this.checkForBillingInformation();
      this.checkForScheduleInformation();
    } else if (this.action === 'newFromTicket') {
      this.dialogTitle = 'New Work Order From Ticket';
      this.initData();
      this.dueDate = new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0);
      this.workOrder.dueDate = this.dueDate;
      this.workOrder.priority = WORK_ORDERS_PRIORITIES.Low;
      this.woStatus = 1;
      this.workOrderSource = WorkOrderSourceCode.Ticket;
      this.workOrderForm = this.createWOUpdateForm();
      this.workOrderForm.removeControl('id');
      this.checkForAttachemnts();

      if (this.workOrder.tasks.length > 0) {
        this.workOrder.tasks.forEach(task => {
          const taskForm = this.createTask({ description: task.description });
          this.tasks.push(taskForm);
          // this.addTask(task.description);
        });
        this.AddChecklist();
      }
    } else if (this.action === 'newFromPreCalendar') {
      this.preCalendar = this.data.values;
      this.woStatus = 1;
      this.dialogTitle = 'New Work Order From Pre-Calendar';
      this.buildingSelected = this.preCalendar.buildingId;
      this.workOrderSource = WorkOrderSourceCode.Other; // MG-23
      this.workOrderForm = this.createWOCreateFormPreCalendar();
      this.buildingSelected = this.preCalendar.buildingId;
      if (this.preCalendar.tasks.length > 0) {
        const taskFormGroups = this.preCalendar.tasks.map(task => this.formBuilder.group(task));
        const taskFormArray = this.formBuilder.array(taskFormGroups);
        this.workOrderForm.setControl('tasks', taskFormArray);
        this.AddChecklist();
      }
      this.getTicketStatus();
    } else if (this.action === 'editFromPreCalendar') {
      this.dialogTitle = 'Edit Work Order ';
      this.initData();
      this.workOrderForm = this.createWOUpdateFormFromCalendar();
      this.checkForTasks();
      this.checkForNotes();
      this.checkForAttachemnts();
      this.checkForBillingInformation();

      // if (new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate()) >
      //   new Date(this.workOrder.snoozeDate))
      if (new Date() >
        this.convertUTCToLocalTime(this.workOrder.snoozeDate, this.workOrder.epochSnoozeDate)) {
        this.buttonSaveDisabled = true;
      }
    } else {
      this.woStatus = 1;
      this.dialogTitle = 'New Work Order';
      this.workOrderForm = this.createWOCreateForm();
      this.workOrderForm.get('assignedEmployees').setValue([]);
      this.getTicketStatus();
    }

    if (this.workOrder.statusId === 3 || this.workOrder.statusId === 4) {
      this.workOrderForm.disable();
    }

    this.roleLevelLoggedUser = this.authService.currentUser.roleLevel;

    this.setPermissions();
    this.getPriorities();
    this.getBuildings();
    this.getWOSources();
    this.getScheduleCategory();

    this.buildingCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    if (this.workOrder.sendRequesterNotifications === false ||
      this.workOrder.sendPropertyManagersNotifications === false) {
      this.setExtNotifications = true;
    }
    this.woTypeChanged(this.workOrderForm.controls['type'].value);

    this.getEmployees();
    this.workOrderForm.controls['buildingId'].valueChanges
      .subscribe(value => {
        this.getBuildingEmployees(value);
      });
  }

  initData(): void {
    this.workOrder = this.data.workOrder;
    this.unscheduledStatus = this.workOrder.unscheduled;

    if (this.workOrder.dueDate) {
      this.dueDate = this.convertUTCToLocalTime(this.workOrder.dueDate, this.workOrder.epochDueDate);
    }

    if (this.workOrder.snoozeDate) {
      this.workOrder.snoozeDate = this.convertUTCToLocalTime(this.workOrder.snoozeDate, this.workOrder.epochSnoozeDate);
    }
    this.buildingSelected = this.workOrder.buildingId;
    this.woStatus = this.workOrder.statusId;

    if (this.workOrder.scheduleDate) {
      this.workOrder.scheduleDate = this.convertUTCToLocalTime(this.workOrder.scheduleDate, this.workOrder.epochScheduleDate);
    }

    this.getTicketStatus();
  }

  setPermissions(): void {
    if (this.roleLevelLoggedUser >= 30) {
      this.workOrderForm.get('type').disable();
      this.workOrderForm.get('sourceCode').disable();
      this.workOrderForm.get('priority').disable();
      this.workOrderForm.get('dueDate').disable();
      this.workOrderForm.get('buildingId').disable();
      this.workOrderForm.get('location').disable();
      this.workOrderForm.get('description').disable();
      this.workOrderForm.get('requesterFullName').disable();
      this.workOrderForm.get('requesterEmail').disable();

      // get all tasks and disabled all descriptions
      const tasks = this.workOrderForm.value.tasks;
      // convert array tasks to formGroups
      const taskFormGroups = tasks.map(task => this.formBuilder.group({
        description: new FormControl({ value: task.description, disabled: true }),
        isComplete: [task.isComplete],
        serviceId: [task.serviceId],
        unitPrice: [task.unitPrice],
        quantity: [task.quantity],
        discountPercentage: [task.discountPercentage],
        total: [task.total],
        unitFactor: [task.unitFactor],
        serviceName: [task.serviceName]
      }));
      // create a new formArray with the array of formGroup
      const taskFormArray = this.formBuilder.array(taskFormGroups);
      // replace formArray
      this.workOrderForm.setControl('tasks', taskFormArray);
    }
  }

  getPriorities(): void {
    /*Covert enum WORK_ORDERS_PRIORITIES to array workOdersPriorities*/
    for (const n in WORK_ORDERS_PRIORITIES) {
      if (typeof WORK_ORDERS_PRIORITIES[n] === 'number') {
        this.workOdersPriorities.push({ id: WORK_ORDERS_PRIORITIES[n] as any, name: n.replace(/_/g, ' ') });
      }
    }
  }

  getWOSources(): void {
    this.woService.getAllAsList('readallwosourcescbo', '', 0, 20, null, {})
      .subscribe((response: { count: number, payload: ListWOSourcesModel[] }) => {

        if (this.action === 'edit' || this.action === 'newFromTicket') {
          this.woSources = response.payload;
        }
        else {
          this.woSources = response.payload.filter(source => (source.code > 0 && source.code < 4) || (source.code > 4 && source.code < 10) || source.code == 14 || source.code == 16);
        }
      },
        (error) => this.snackBar.open('Oops, there was an error fetching work order sources', 'close', { duration: 1000 }),
        () => {
          // For default work order source should be "Email" : code = 0
          if (this.action !== 'edit' && this.action !== 'newFromTicket') {
            this.workOrderForm.get('workOrderSourceId').setValue(this.woSources.find(w => w.code === 0).id);
          }

          if (this.action === 'newFromTicket' && this.workOrder.workOrderSourceCode >= 0) {
            const woSource = this.woSources.find(source => source.code === this.workOrder.workOrderSourceCode);
            if (woSource) {
              this.workOrderForm.get('workOrderSourceId').patchValue(woSource.id);
            }
          }

        }
      );
  }


  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  getBuildings(filter = ''): void {
    this.buildingService.getAllAsList('readallcbo', filter, 0, 20, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
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

  onChangeBuilding(): void {
    if (this.buildingSelected && this.buildingSelected !== -1) {
      // Clean contacts array
      this.contactsRequester = [];

      // Get contacts of bulding, requesters and properties manager
      this.contactService.getAll(`readallcontacts/${String(this.buildingSelected)}?contactType=building`)
        .subscribe((response: ContactGridModel[]) => {
          for (const contact of response) {
            if (contact.type.toLowerCase() === 'requester') {
              this.contactsRequester.push(contact);
            }
          }
        },
          (error) => this.snackBar.open('Oops, there was an error fetching building contacts', 'close', { duration: 1000 }),
          () => this.findContactByEmail()
        );
    }
  }

  getContactsRequesters(): void {
    if (!this.buildingSelected || this.buildingSelected === -1) {
      return;
    }
    this.contactsRequester = [];

    this.contactService.getAllContactsByEntity(this.buildingSelected, 'building')
      .subscribe((response: ContactGridModel[]) => {
        for (const contact of response) {
          this.contactsRequester.push(contact);
          this.contactsRequester.push(contact);
        }
      },
        (error) => this.snackBar.open('Oops, there was an error fetching building contacts', 'close', { duration: 1000 }));
  }

  /*
    Filter requester list (this.contactsRequester) by `WorkOrder.RequesterEmail` and in case one matches,
    pre-select it. In case no contact matches pre-populate contact form fields with `WorkOrder.Requester*` fields
  */
  findContactByEmail(): void {
    this.preRequesterSelected = {};
    if (this.workOrder.statusId === 0) {
      const requester = this.contactsRequester.find(c => c.email === this.workOrder.requesterEmail);

      if (requester != null) {
        this.requesterSelected = requester.id;
      }
      else {
        this.preRequesterSelected = {
          firstName: this.workOrder.requesterFirstName,
          middleName: '',
          lastName: this.workOrder.requesterLastName,
          fullName: this.workOrder.requesterFirstName + this.workOrder.requesterLastName,
          phone: this.workOrder.requesterPhone,
          email: this.workOrder.requesterEmail,
        };
      }
    }
  }

  createWOCreateForm(): FormGroup {
    return this.formBuilder.group({
      dueDate: [new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0)],
      buildingId: [''],
      buildingCtrl: [''],
      location: [''],
      locationCtrl: [''],
      description: [''],
      assignedEmployeeId: [''],
      assignedEmployees: [],
      requesterFullName: [''],
      requesterEmail: [''],
      priority: [WORK_ORDERS_PRIORITIES.Low],
      sendRequesterNotifications: [false],
      sendPropertyManagersNotifications: [false],
      tasks: this.formBuilder.array([]),
      notes: this.formBuilder.array([]),
      attachments: this.formBuilder.array([]),
      administratorId: [this.authService.currentUser.employeeId],
      statusId: [this.woStatus],
      type: [0],
      workOrderSourceId: [0],
      sourceCode: [this.workOrderSource],
      billingDateType: [this.workOrder.billingDateType],
      billingName: [''],
      billingEmail: [''],
      billingNote: [''],
      closingNotes: [''],
      additionalNotes: [0],
      closingNotesOther: [''],
      originWorkOrderId: [''],
      followUpOnClosingNotes: [],
      defineDate: [false],
      snoozeDate: [null],

      clientApproved: [false],
      scheduleDateConfirmed: [false],
      setStatusByStandBy: [true],
      scheduleDate: [this.fromCalendar ? new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0) : null],
      scheduleCategoryId: [''],
      scheduleSubCategoryId: [''],
      sendNotifications: [this.sendNotifications],
      unscheduled: [false],
      workOrderScheduleSettingId: [null]
    });
  }

  createWOUpdateForm(): FormGroup {
    // let defineSnoozeDate = true;
    // if (this.workOrder.epochSnoozeDate === 0) {
    //   defineSnoozeDate = false;
    // } else {
    //   defineSnoozeDate = true;
    // }
    return this.formBuilder.group({
      id: [this.workOrder.id],
      dueDate: [this.dueDate ? this.dueDate : null],
      buildingId: [this.workOrder.buildingId],
      buildingCtrl: [''],
      location: [this.workOrder.location],
      description: [this.workOrder.description],
      assignedEmployees: [],
      requesterFullName: [this.workOrder.requesterFullName],
      requesterEmail: [this.workOrder.requesterEmail],
      priority: [this.workOrder.priority],
      sendRequesterNotifications: [this.workOrder.sendRequesterNotifications],
      sendPropertyManagersNotifications: [this.workOrder.sendPropertyManagersNotifications],
      tasks: this.formBuilder.array([]),
      notes: this.formBuilder.array([]),
      attachments: this.formBuilder.array([]),
      administratorId: [this.workOrder.administratorId],
      statusId: [this.workOrder.statusId],
      type: [this.workOrder.type],
      workOrderSourceId: [this.workOrder.workOrderSourceId],
      sourceCode: [this.workOrderSource],
      billingDateType: [this.workOrder.billingDateType],
      billingName: [this.workOrder.billingName],
      billingEmail: [this.workOrder.billingEmail],
      billingNote: [this.workOrder.billingNote],
      closingNotes: [this.workOrder.closingNotes],
      additionalNotes: [this.workOrder.additionalNotes],
      closingNotesOther: [this.workOrder.closingNotesOther],
      originWorkOrderId: [this.workOrder.originWorkOrderId],
      keepCloningReference: [true],
      followUpOnClosingNotes: [this.workOrder.followUpOnClosingNotes],
      defineDate: [false],
      snoozeDate: [this.workOrder.snoozeDate],

      clientApproved: [this.workOrder.clientApproved],
      scheduleDateConfirmed: [this.workOrder.scheduleDateConfirmed],
      setStatusByStandBy: [true],
      scheduleDate: [this.workOrder.scheduleDate],
      scheduleCategoryId: [this.workOrder.scheduleCategoryId],
      scheduleSubCategoryId: [this.workOrder.scheduleSubCategoryId],
      unscheduled: [this.workOrder.unscheduled],
      workOrderScheduleSettingId: [0]
    });
  }

  createWOUpdateFormFromCalendar(): FormGroup {
    // let defineSnoozeDate = true;
    // if (this.workOrder.epochSnoozeDate === 0) {
    //   defineSnoozeDate = false;
    // } else {
    //   defineSnoozeDate = true;
    // }
    return this.formBuilder.group({
      id: [this.workOrder.id],
      dueDate: [this.dueDate],
      buildingId: [this.workOrder.buildingId],
      buildingCtrl: [''],
      location: [this.workOrder.location],
      description: [this.workOrder.description],
      requesterFullName: [this.workOrder.requesterFullName],
      requesterEmail: [this.workOrder.requesterEmail],
      priority: [this.workOrder.priority],
      sendRequesterNotifications: [this.workOrder.sendRequesterNotifications],
      sendPropertyManagersNotifications: [this.workOrder.sendPropertyManagersNotifications],
      tasks: this.formBuilder.array([]),
      notes: this.formBuilder.array([]),
      attachments: this.formBuilder.array([]),
      administratorId: [this.workOrder.administratorId],
      statusId: [],
      type: [this.workOrder.type],
      workOrderSourceId: [this.workOrder.workOrderSourceId],
      workOrderBillingTypeDateId: [0],
      billingName: [this.workOrder.billingName],
      billingEmail: [this.workOrder.billingEmail],
      billingNote: [this.workOrder.billingNote],
      closingNotes: [this.workOrder.closingNotes],
      additionalNotes: [this.workOrder.additionalNotes],
      closingNotesOther: [this.workOrder.closingNotesOther],
      originWorkOrderId: [this.workOrder.originWorkOrderId],
      keepCloningReference: [true],
      followUpOnClosingNotes: [this.workOrder.followUpOnClosingNotes],
      defineDate: [true],
      snoozeDate: [this.workOrder.snoozeDate],

      clientApproved: [this.workOrder.clientApproved],
      scheduleDateConfirmed: [this.workOrder.scheduleDateConfirmed],
      scheduleDate: [this.workOrder.scheduleDate],
      scheduleCategoryId: [this.workOrder.scheduleCategoryId],
      scheduleSubCategoryId: [this.workOrder.scheduleSubCategoryId],
      unscheduled: [this.workOrder.unscheduled]
    });
  }

  createWOCreateFormPreCalendar(): FormGroup {
    return this.formBuilder.group({
      dueDate: [this.preCalendar.snoozeDate],
      buildingId: [this.preCalendar.buildingId],
      buildingCtrl: [''],
      location: [''],
      locationCtrl: [''],
      description: [this.preCalendar.description],
      assignedEmployeeId: [''],
      requesterFullName: [''],
      requesterEmail: [''],
      priority: [WORK_ORDERS_PRIORITIES.Low],
      sendRequesterNotifications: [false],
      sendPropertyManagersNotifications: [false],
      tasks: this.formBuilder.array([]),
      notes: this.formBuilder.array([]),
      attachments: this.formBuilder.array([]),
      administratorId: [this.authService.currentUser.employeeId],
      statusId: [this.workOrder.statusId],
      type: [0],
      workOrderSourceId: [0],
      sourceCode: [this.workOrderSource],
      billingDateType: [this.workOrder.billingDateType],
      billingName: [''],
      billingEmail: [''],
      billingNote: [''],
      closingNotes: [''],
      additionalNotes: [0],
      closingNotesOther: [''],
      originWorkOrderId: [''],
      followUpOnClosingNotes: [],
      defineDate: [{
        value: true,
        disabled: true
      }],
      snoozeDate: [this.preCalendar.snoozeDate],


      clientApproved: [false],
      scheduleDateConfirmed: [false],
      scheduleDate: [new Date(this.today.getFullYear(), this.today.getMonth(), this.today.getDate(), 18, 0, 0), Validators.required],
      scheduleCategoryId: [''],
      scheduleSubCategoryId: [''],
      unscheduled: [false]
    });
  }

  /**
   * Remove Due date
   */
  removeDueDate(): void {
    this.dueDate = null;
    this.workOrderForm.get('dueDate').setValue(null);
  }

  convertUTCToLocalTime(dateToValidate: any, epochDate: number): any {
    const possibleDate: any = new Date(dateToValidate);
    const dateToCompare = new Date('2000-01-01');

    if (possibleDate < dateToCompare) {
      return null;
    }
    else {
      return new Date(this.epochPipe.transform(epochDate));
    }
  }

  /**
   * Checklist section
   */

  checkForTasks(): void {
    // Check if there are tasks into work order to show section "Tasks List"
    if (this.workOrder.tasks.length > 0) {
      const taskFormGroups = this.workOrder.tasks.map(task => this.formBuilder.group(task));
      const taskFormArray = this.formBuilder.array(taskFormGroups);
      this.workOrderForm.setControl('tasks', taskFormArray);
      this.AddChecklist();
    }
  }

  AddChecklist(): void {
    this.addCheckList = true;
  }

  removeChecklist(): void {
    while (this.tasks.length) {
      this.tasks.removeAt(0);
    }
    this.addCheckList = false;
  }

  createTask(newTask: any): FormGroup {
    return this.formBuilder.group({
      description: [newTask.description || ''],
      isComplete: [newTask.isComplete || false],
      serviceId: [null],
      unitPrice: [newTask.unitPrice || 0],
      quantity: [newTask.quantity || 0],
      discountPercentage: [newTask.discountPercentage || 0],
      note: [newTask.note || ''],
      workOrderServiceCategoryId: [newTask.workOrderServiceCategoryId || null],
      workOrderServiceId: [newTask.workOrderServiceId || null],
      unitFactor: [newTask.unitFactor || ''],
      frequency: [newTask.frequency || 0],
      rate: [newTask.rate || 0],
      total: [0],
      serviceName: [newTask.serviceName || ''],
      location: [newTask.location || ''],
      attachments: this.formBuilder.array([]),
      requiresScheduling: [newTask.requiresScheduling || false],
      hoursRequiredAtClose: [newTask.hoursRequiredAtClose || false],
      quantityRequiredAtClose: [newTask.quantityRequiredAtClose || false],
      quantityExecuted: [newTask.quantityExecuted || 0],
      hoursExecuted: [newTask.hoursExecuted || 0],
      completedDate: [newTask.completedDate || null],
      generalNote: [newTask.generalNote || ''],
    });
  }

  get tasks(): FormArray {
    return this.workOrderForm.get('tasks') as FormArray;
  }

  get woTasks(): WorkOrderTaskModel[] {
    return this.tasks.controls.map(
      c => new WorkOrderTaskModel(c.value)
    );
  }

  addTask(description: any): void {
    const task = this.createTask({ description: description });
    this.tasks.push(task);
    this.taskDescription = '';
  }

  updateTaskCheck(index: any, value: any): void {
    const task = this.workOrderForm.value.tasks[index];

    if (value) {
      this.closeWorkOrderTaskDialog = this.dialog.open(WoTaskFormConfirmCloseComponent, {
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
            return;
          }
          const formObject = dialogResult.getRawValue();

          this.workOrderForm.value.tasks[index].isComplete = true;
          this.workOrderForm.value.tasks[index].completedDate = formObject.completedDate;
          this.workOrderForm.value.tasks[index].hoursExecuted = formObject.hours;
          this.workOrderForm.value.tasks[index].quantityExecuted = formObject.quantity;
        });
    } else {
      this.workOrderForm.value.tasks[index].isComplete = false;
      this.workOrderForm.value.tasks[index].completedDate = null;
      this.workOrderForm.value.tasks[index].hoursExecuted = 0;
      this.workOrderForm.value.tasks[index].quantityExecuted = 0;
    }
  }

  deleteTask(index: any): void {
    (this.workOrderForm.get('tasks') as FormArray).removeAt(index);
  }

  countTasksCheck(): any {
    const tasks = this.workOrderForm.value.tasks;
    return tasks.filter(el => el.isComplete).length;
  }

  addTaskBilling(index: any): void {
    this.dialogTaskBillingRef = this.dialog.open(WoTaskBillingFormComponent, {
      panelClass: 'task-billing-form-dialog',
      data: {
        task: this.workOrderForm.get('tasks').value[index],
        action: 'new'
      }
    });

    this.dialogTaskBillingRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }
        // Save task with billing data into tasks controls
        this.tasks.removeAt(index);
        this.tasks.insert(index, response);
      });
  }

  newTask(billable: boolean): void {
    this.taskFormDialog = this.dialog.open(WorkOrderTaskFormComponent, {
      panelClass: 'wo-task-form-dialog',
      data: {
        action: 'new',
        newWo: this.action === 'new' ? true : false,
        hideAttachments: true,
        showBilling: billable
      }
    });

    this.taskFormDialog.afterClosed()
      .subscribe((dialogResult: { form: FormGroup, files: any[] }) => {
        if (!dialogResult) {
          return;
        }

        if (this.action === 'new' || this.action === 'newFromTicket') {
          const task = this.createTask(dialogResult.form.getRawValue());
          this.tasks.push(task);
        }

      });
  }

  updateTask(index: number): void {
    const task = this.workOrderForm.get('tasks').value[index];
    if (task) {
      if (this.action === 'new' || this.action === 'newFromTicket') {
        const taskUpdate = new WorkOrderTaskUpdateModel(task);
        const showBilling = taskUpdate.workOrderServiceId !== null || taskUpdate.workOrderServiceCategoryId !== null;

        this.taskFormDialog = this.dialog.open(WorkOrderTaskFormComponent, {
          panelClass: 'wo-task-form-dialog',
          data: {
            action: 'edit',
            task: taskUpdate,
            newWo: this.action === 'new' ? true : false,
            hideAttachments: true,
            showBilling: showBilling
          }
        });

        this.taskFormDialog.afterClosed()
          .subscribe((dialogResult: { form: FormGroup, files: any[] }) => {
            if (!dialogResult) {
              return;
            }
            const updatedTask = this.createTask(dialogResult.form.getRawValue());

            // Save task with billing data into tasks controls
            this.tasks.removeAt(index);
            this.tasks.insert(index, updatedTask);
          });
      }
    }
  }

  /**
   * Notes section
   */
  checkForNotes(): void {
    // Check if there are notes into work order to show section "Notes"
    if (this.workOrder.notes.length > 0) {
      const noteFormGroups = this.workOrder.notes.map(note => {

        // Assign created day form UTC to Local
        const timeZone = new Date().getTimezoneOffset();
        // const createdDateTimeLocal: string = this.datePipe.transform(note.createdDate, 'yyyy-MM-dd HH:mm Z');
        // note.createdDate = new Date(createdDateTimeLocal);

        return this.formBuilder.group(note);
      });
      const noteFormArray = this.formBuilder.array(noteFormGroups);
      this.workOrderForm.setControl('notes', noteFormArray);
      this.AddNotes();
    }
  }

  AddNotes(): void {
    this.addNotes = true;
  }

  createNote(textNote: any, employeeId: any, employeeFullName: any): FormGroup {
    const now = new Date();
    return this.formBuilder.group({
      note: [textNote],
      employeeId: [employeeId],
      employeeFullName: [employeeFullName],
      createdDate: [now],
      // HACK: Forcing UTC date
      epochCreatedDate: [Math.floor(now.getTime() / 1000) + now.getTimezoneOffset() * 60],
    });
  }

  get notes(): FormArray {
    return this.workOrderForm.get('notes') as FormArray;
  }

  addNote(textNote: any): void {
    const loggedUser = this.authService.currentUser;
    const note = this.createNote(textNote, loggedUser.employeeId, loggedUser.employeeFullName);
    this.notes.push(note);
    this.textNote = '';
  }

  /*
  * Attachments section
  */
  ShowAttachments(): void {
    this.showAttachments = true;
  }

  get attachments(): FormArray {
    return this.workOrderForm.get('attachments') as FormArray;
  }

  checkForAttachemnts(): void {
    // Check if there are attachments into woto show section "Attachments"
    if (this.workOrder.attachments.length > 0) {
      this.showAttachments = true;
      const attachmentFormGroups = this.workOrder.attachments.map(attachemnt => this.formBuilder.group(attachemnt));
      const attachmentFormArray = this.formBuilder.array(attachmentFormGroups);
      this.workOrderForm.setControl('attachments', attachmentFormArray);

      // add images to woImages for carousel
      this.workOrder.attachments.forEach(attachment => {

        const src = attachment.fullUrl;
        const caption = attachment.description;

        if (src && caption) {
          const album = {
            src: src,
            caption: caption,
            thumb: ''
          };
          this.woImages.push(album);
        }
      });
      this._lighboxConfig.fadeDuration = 1;
    }
  }

  open(index: number): void {
    this._subscription = this._lightboxEvent.lightboxEvent$.subscribe((event: IEvent) => this._onReceivedEvent(event));

    // override the default config
    this._lightbox.open(this.woImages, index, { wrapAround: true, showImageNumberLabel: true });
  }

  private _onReceivedEvent(event: IEvent): void {
    if (event.id === LIGHTBOX_EVENT.CLOSE) {
      this._subscription.unsubscribe();
    }
  }

  openInputFile(): void {
    document.getElementById('fileInput').click();
  }

  fileChange(files: File[]): void {
    if (files.length > 0) {
      this.woService.uploadFile(files)
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
              this.woImages.push(album);
              this.addAttachment(response.body[i].fullUrl, response.body[i].fileName, response.body[i].imageTakenDate, response.body[i].blobName);
            }
          }
          if (response.status === 206) {
            this.snackBar.open('Oops, there was an error, some images were not uploaded, please try again later', 'close', { duration: 1000 });
          }
        },
          (error) => { this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }); });
    }
  }

  addAttachment(fullUrl: any, description: any, imageTakenDate: any, blobName: any): void {
    const loggedUser = this.authService.currentUser;
    const attachment = this.formBuilder.group({
      id: [-1],
      fullUrl: [fullUrl],
      description: [description],
      imageTakenDate: [imageTakenDate],
      employeeId: [loggedUser.employeeId],
      blobName: [blobName]
    });
    this.attachments.push(attachment);
  }

  removeAttachment(index): void {
    const attachmentToDelete = this.attachments.at(index);
    (this.workOrderForm.get('attachments') as FormArray).removeAt(index);
    this.woImages.splice(index, 1);
    // If id of deleted attachment was -1, it's necessary deleted it from azure
    if (attachmentToDelete.get('id').value === -1) {
      this.woService.deleteAttachmentByBlobName(attachmentToDelete.get('blobName').value).subscribe();
    }
  }

  /*
  * End Attachments section
  */

  /*
  * Billing Information
  */
  ShowBillingInformation(): void {
    this.showBillingInformation = true;
  }

  checkForBillingInformation(): void {
    this.showBillingInformation = false;
    if (this.workOrder.billingEmail || this.workOrder.billingName || this.workOrder.billingNote) {
      this.showBillingInformation = true;
    }
  }

  checkForScheduleInformation(): void {
    this.showScheduleSettings = false;
    if (this.workOrder.scheduleCategoryId || this.workOrder.scheduleSubCategoryId || this.workOrder.scheduleDate
      || this.workOrder.clientApproved || this.workOrder.scheduleDateConfirmed) {
      this.showScheduleSettings = true;
    }
  }
  /*
  * End Billing Information
  */

  isDisabled(value: any): boolean {
    return !value || value === '';
  }

  setExternalNotifications(): void {
    this.setExtNotifications = true;
  }

  submit(): void {
    if (this.woStatus === 0) {
      this.workOrderForm.get('statusId').setValue(1);
    }

    if (!this.workOrderForm.controls['defineDate'].value) {
      this.workOrderForm.patchValue({
        snoozeDate: null,
        epochSnoozeDate: 0
      });
    }

    if ((this.action === 'new' || this.action === 'newFromTicket') && this.selectedScheduleFrequency !== 7) {
      const selectedDays: number[] = [];
      this.scheduleDays.forEach(d => {
        if (d.isSelected) {
          selectedDays.push(d.id);
        }
      });

      if (this.selectedScheduleFrequency === 0) {
        if (!this.scheduleStartDate || !this.scheduleEndDate) {
          this.snackBar.open('Invalid dates on schedule the settings', 'close', { duration: 1000 });
          return;
        }
      } else if (this.selectedScheduleFrequency === 1) {
        if (!this.scheduleStartDate) {
          this.snackBar.open('Invalid start date on schedule the settings', 'close', { duration: 1000 });
          return;
        }
        if (!this.scheduleEndDate) {
          this.snackBar.open('Invalid end date on schedule the settings', 'close', { duration: 1000 });
          return;
        }
        if (selectedDays.length === 0) {
          this.snackBar.open('Select at least one day in schedule settings', 'close', { duration: 1000 });
          return;
        }
      } else if (this.selectedScheduleFrequency === 2) {
        if (selectedDays.length === 0) {
          this.snackBar.open('Select at least one day in schedule settings', 'close', { duration: 1000 });
          return;
        }
        if (selectedDays.length > 1) {
          this.snackBar.open('Select only one day in schedule settings', 'close', { duration: 1000 });
          return;
        }
      } else if (this.selectedScheduleFrequency > 2 && this.selectedScheduleFrequency < 6) {
        if (!this.scheduleStartDate || !this.scheduleEndDate) {
          this.snackBar.open('Invalid dates on schedule the settings', 'close', { duration: 1000 });
          return;
        }

        // if (!this.scheduleDate) {
        //   this.snackBar.open('Invalid schedule date on schedule the settings', 'close', { duration: 1000 });
        //   return;
        // }
      } else if (this.selectedScheduleFrequency === 6) {
        if (!this.scheduleDate) {
          this.snackBar.open('Invalid dates on schedule the settings', 'close', { duration: 1000 });
          return;
        }
      }

      const startValue = this.selectedScheduleFrequency === 2 ? this.scheduleStartMonth : this.selectedScheduleMonth;
      const endValue = this.selectedScheduleFrequency === 2 ? this.scheduleEndMonth : this.scheduleStartYear;

      this.workOrderForm.setControl('scheduleSettings',
        this.formBuilder.group({
          workOrderId: [0],
          frequency: [this.selectedScheduleFrequency],
          startDate: [this.scheduleStartDate],
          endDate: [this.scheduleEndDate],
          ordinal: [this.selectedScheduleOrdinal],
          startValue: [startValue],
          endValue: [endValue],
          days: this.formBuilder.array(selectedDays),
          scheduleDate: [this.scheduleDate],
          excludedScheduleDates: this.formBuilder.array(this.excludedScheduleDates)
        }));
    }

    this.workOrderForm.patchValue({
      sendNotifications: this.sendNotifications
    });

    const dialogPayload = this.action === 'edit' || this.action === 'editFromPreCalendar' ? ['save', this.workOrderForm] : this.workOrderForm;

    if (this.woStatus === '3') {

      const today = new Date();

      if (this.workOrderDueDate.value > today) {
        this.displayInformationDialog('WO Not Yet Due!',
          'This Work Order cannot be closed because it is not due yet. Please wait until its Due Date to change its status to Closed.');
      }
      else if (this.countTasksCheck() !== this.tasks.length) {
        this.displayInformationDialog('WO with uncompleted tasks!',
          'This Work Order cannot be closed because it has uncompleted tasks.');
      }
      else {
        this.confirmDialogRef = this.dialog.open(WoConfirmDialogComponent, {
          disableClose: false,
          data: {
            roleLevelLoggedUser: this.roleLevelLoggedUser
          }
        });

        this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to close this Work Order?' + ' ' +
          'Feel free to use the field below to add any Closing Notes if necessary.';

        this.confirmDialogRef.afterClosed().subscribe(result => {
          if (result[0]) {
            const formData = result[1].getRawValue();

            this.workOrderForm.get('closingNotes').setValue(formData.closingNotes);
            this.workOrderForm.get('additionalNotes').setValue(formData.additionalNotes);
            this.workOrderForm.get('closingNotesOther').setValue(formData.closingNotesOther);
            this.workOrderForm.get('followUpOnClosingNotes').setValue(formData.followUpOnClosingNotes);

            if (this.countTasksCheck() < this.tasks.length) {
              // get all tasks
              const tasks = this.workOrderForm.value.tasks;
              // change to complete all tasks
              tasks.map(task => task.isComplete = true);
              // convert array tasks to formGroups
              const taskFormGroups = tasks.map(task => this.formBuilder.group(task));
              // create a new formArray with the array of formGroup
              const taskFormArray = this.formBuilder.array(taskFormGroups);
              // replace formArray
              this.workOrderForm.setControl('tasks', taskFormArray);
            }

            this.onWorkOrderTemplateSubmitted.next(dialogPayload);
          }
        });
      }
    }
    else {
      // Cancelled status
      if (this.woStatus === '4') {
        this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
          disableClose: false
        });

        this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to cancel this Work Order?';

        this.confirmDialogRef.afterClosed().subscribe(result => {
          if (result) {
            this.submitDialogPayload(dialogPayload);
          }
          this.confirmDialogRef = null;
        });
      } else {
        // Any other status
        this.submitDialogPayload(dialogPayload);
      }
    }
  }

  private submitDialogPayload(dialogPayload: any): void {
    this.workOrderForm.get('additionalNotes').setValue(0);
    // this.workOrderForm.get('sendNotifications').setValue(this.sendNotifications);
    this.onWorkOrderTemplateSubmitted.next(dialogPayload);
  }

  delete(): void {
    this.onWorkOrderTemplateSubmitted.next(['delete', this.workOrderForm]);
  }

  closeDialog(): void {
    this.onWorkOrderTemplateSubmitted.next(['close', this.workOrderForm]);

    // Delete images uploaded in azure but not saved in data base
    for (let i = 0; i < this.attachments.value.length; i++) {
      if (this.attachments.at(i).get('id').value === -1) {
        this.woService.deleteAttachmentByBlobName(this.attachments.at(i).get('blobName').value).subscribe();
      }
    }
  }

  displayInformationDialog(dialogTitle: any, message: any): void {
    this.confirmDialogRef = this.dialog.open(MessageDialogComponent, {
      disableClose: false,
    });
    this.confirmDialogRef.componentInstance.dialogTitle = dialogTitle;
    this.confirmDialogRef.componentInstance.message = message;
    this.confirmDialogRef.afterClosed().subscribe(() => {
      return;
    });
  }

  concatAdditionalNotesToDescription(additionalNotes: any[] = null): void {
    if (additionalNotes) {
      additionalNotes.forEach(note => {
        const description = this.workOrderForm.get('description').value;
        this.workOrderForm.get('description').setValue(description + '\n' + note);
      });
    }
  }

  getScheduleCategory(filter = ''): void {
    this.schedueSettingsCategoryService.getAllAsList('readallcbo', filter, 0, 20, this.buildingSelected, {})
      .subscribe((response: { count: number, payload: ListScheduleCategoryModel[] }) => {
        this.scheduleCategories = response.payload;
        this.filteredscheduleCategories$.next(response.payload);
        if (this.workOrderForm.controls['scheduleCategoryId'].value) {
          this.categoryChanged(this.workOrderForm.controls['scheduleCategoryId'].value);
        }
      },
        (error) => this.snackBar.open('Oops, there was an error fetching schedule categories', 'close', { duration: 1000 }));
  }

  categoryChanged(id: number): void {
    this.schedueSettingsCategoryService.getAllAsListByCategory(id)
      .subscribe((response: { count: number, payload: ScheduleSubcategoryBaseModel[] }) => {
        this.scheduleSubCategories = response.payload;
        this.filteredscheduleSubCategories$.next(this.scheduleSubCategories);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });

  }

  woTypeChanged(buildingId: number): void {
    if (buildingId === 4) {
      this.showCategoryAndSubCategory = true;
    } else {
      this.showCategoryAndSubCategory = false;
    }
  }

  // SCHEDULE
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

  ShowScheduleSettings(): void {
    this.showScheduleSettings = true;
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
      this.snackBar.open('Select at least one day in schedule settings', 'close', { duration: 1000 });
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
  getEmployees(): void {
    this._userService.getAllAsList('readallcbo', '', 0, 99999)
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

      }, (error) => {
        this.snackBar.open('Oops, there was an error fetching employees', 'close', { duration: 1000 });
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
      }, (error) => {
        this.snackBar.open('Oops, there was an error fetching employees', 'close', { duration: 1000 });
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
      });
    }
  }

}
