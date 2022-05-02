import { Component, OnInit, OnDestroy, Inject, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BuildingUpdateModel } from '@app/core/models/building/building-update.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { InspectionBaseModel } from '@app/core/models/inspections/inspection-base.model';
import { PreCalendarBaseModel } from '@app/core/models/pre-calendar/pre-calendar-base.model';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { BuildingsService } from '@app/main/content/private/buildings/buildings.service';
import { UsersBaseService } from '@app/main/content/private/users/users-base.service';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-inspections-form',
  templateUrl: './inspections-form.component.html',
  styleUrls: ['./inspections-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class InspectionsFormComponent implements OnInit, OnDestroy {

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  roleLevelLoggedUser: number;

  inspectionForm: FormGroup;
  inspection: InspectionBaseModel;

  action: string;
  dialogTitle: string;
  private _onDestroy = new Subject<void>();

  get selectedStars(): any { return this.inspectionForm.get('stars').value; }
  stars = [5, 4, 3, 2, 1];

  get isClosed(): boolean { return this.inspection ? this.inspection.status === 5 : false; }

  get getBuildingFilter(): AbstractControl { return this.inspectionForm.get('buildingFilter'); }
  buildings: ListItemModel[] = [];
  filteredBuildings: Subject<any[]> = new Subject<any[]>();
  listBuildingsSubscription: Subscription;

  get getEmployeeFilter(): AbstractControl { return this.inspectionForm.get('employeeFilter'); }
  employees: ListItemModel[] = [];
  filteredEmployees: Subject<any[]> = new Subject<any[]>();
  listEmployeesSubscription: Subscription;

  statusList: any[] = [];

  isInspectorAssigned: boolean = false;

  currentInspectionStatus = 0;
  snoozeDate: Date;
  // preCalendar
  preCalendar: PreCalendarBaseModel;

  buttonSaveDisabled = false;
  constructor(
    public dialogRef: MatDialogRef<InspectionsFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private buildingService: BuildingsService,
    private userService: UsersBaseService,
    private snackBar: MatSnackBar,
    private epochPipe: FromEpochPipe,
  ) {
    this.action = data.action;
    this.isInspectorAssigned = data.isInspector;
    this.roleLevelLoggedUser = data.loggedUserLevel;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Inspection';
      this.inspection = data.inspection;
      this.snoozeDate = this.convertUTCToLocalTime(this.inspection.snoozeDate, this.inspection.epochSnoozeDate);
      this.currentInspectionStatus = this.inspection.status;
      this.inspectionForm = this.updateInspectionForm();
    } else if (this.action === 'newPreCalendarInspection') {
      this.dialogTitle = ' Add inspection from pre-calendar';
      this.preCalendar = data.preCalendar;
      this.currentInspectionStatus = 0;
      this.inspectionForm = this.createInspectionPreCalendarForm();
    } else if (this.action === 'editFromCalendar') {
      this.dialogTitle = 'Edit Inspection';
      this.inspection = data.inspection;
      this.snoozeDate = this.convertUTCToLocalTime(this.inspection.snoozeDate, this.inspection.epochSnoozeDate);
      this.currentInspectionStatus = this.inspection.status;
      this.inspectionForm = this.updateInspectionForm();
      if (new Date() > this.snoozeDate) {
        this.buttonSaveDisabled = true;
      }
    } else {
      this.dialogTitle = 'Add Inspection';
      this.currentInspectionStatus = 0;
      this.inspectionForm = this.createInspectionForm();
    }
  }

  ngOnInit(): void {
    this.getBuildings();

    this.getEmployees();

    this.getBuildingFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });

    this.getEmployeeFilter.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterEmployees();
      });

    this.setDialogByRole();
  }

  ngOnDestroy(): void {
    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    if (this.listEmployeesSubscription && this.listEmployeesSubscription.closed) {
      this.listEmployeesSubscription.unsubscribe();
    }
  }

  /** FORM */
  createInspectionForm(): FormGroup {
    return this.formBuilder.group({
      buildingFilter: [''],
      employeeFilter: [''],
      buildingId: ['', [Validators.required]],
      employeeId: ['', [Validators.required]],
      snoozeDate: [null],
      dueDate: [null],
      status: [0],
      beginNotes: [''],
      closingNotes: [''],
      allowPublicView: [true]
    });
  }

  updateInspectionForm(): FormGroup {
    return this.formBuilder.group({
      buildingFilter: [''],
      employeeFilter: [''],
      id: [this.inspection.id],
      buildingId: [{ value: this.inspection.buildingId, disabled: false }, [Validators.required]],
      employeeId: [{ value: this.inspection.employeeId, disabled: false }, [Validators.required]],
      snoozeDate: [{ value: this.snoozeDate, disabled: false }],
      dueDate: [{ value: this.inspection.dueDate, disabled: false }],
      beginNotes: [{ value: this.inspection.beginNotes, disabled: false }],
      closeDate: [{ value: this.inspection.closeDate, disabled: this.inspection.status === 5 }],
      closingNotes: [{ value: this.inspection.closingNotes, disabled: this.inspection.status === 5 }],
      status: [this.inspection.status],
      stars: [this.inspection.stars],
      allowPublicView: [{ value: this.inspection.allowPublicView, disabled: false }]
    });
  }

  createInspectionPreCalendarForm(): FormGroup {
    return this.formBuilder.group({
      buildingFilter: [''],
      employeeFilter: [''],
      buildingId: [this.preCalendar.buildingId],
      employeeId: [this.preCalendar.employeeId],
      snoozeDate: [this.preCalendar.snoozeDate],
      dueDate: [null],
      status: [0],
      beginNotes: [this.preCalendar.description],
      closingNotes: ['']
    });
  }

  /** BUILDINGS */
  getBuildings(): void {
    if (this.listBuildingsSubscription && this.listBuildingsSubscription.closed) {
      this.listBuildingsSubscription.unsubscribe();
    }

    this.buildingService.getAllAsList('ReadAllCbo', '', 0, 999, null, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings.next(this.buildings);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get buildings', 'Close');
      });
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.getBuildingFilter.value;
    if (!search) {
      this.filteredBuildings.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredBuildings.next(
      this.buildings.filter(building => building.name.toLowerCase().indexOf(search) > -1)
    );
  }

  buildingChanged(id: number): void {
    this.buildingService.get(id, 'getDetail')
      .subscribe((buildingData: BuildingUpdateModel) => {
        if (!buildingData) {
          return;
        }

        this.buildingService.loadingSubject.next(false);
        const buildingUpdateObj = buildingData;
        const inspector = buildingUpdateObj.employees.filter(e => e.type === 8)[0] || null;

        if (inspector) {
          this.inspectionForm.patchValue({
            employeeId: inspector.id
          });
        }
      });
  }

  /** EMPLOYEES */
  getEmployees(): void {
    if (this.listEmployeesSubscription && this.listEmployeesSubscription.closed) {
      this.listEmployeesSubscription.unsubscribe();
    }

    this.userService.getAllAsList('readallcbo', '', 0, 99999, null)
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.employees = response.payload;
        this.filteredEmployees.next(this.employees);
      }, (error) => {
        this.snackBar.open('Ops! Error when trying to get employees', 'Close');
      });
  }

  private filterEmployees(): void {
    if (!this.employees) {
      return;
    }
    // get the search keyword
    let search = this.getEmployeeFilter.value;
    if (!search) {
      this.filteredEmployees.next(this.employees.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the customers
    this.filteredEmployees.next(
      this.employees.filter(employee => employee.name.toLowerCase().indexOf(search) > -1)
    );
  }

  /** STARS */
  selectStar(stars: number): void {

    if (this.inspection.status === 5) {
      return;
    }

    this.inspectionForm.patchValue({
      stars: stars
    });
  }

  /** DIALOG */
  setDialogByRole(): void {

    // General
    // if (this.roleLevelLoggedUser > 20) {
    //   this.inspectionForm.get('beginNotes').disable();
    //   this.inspectionForm.get('snoozeDate').disable();
    // }

    // if (this.inspection) {
    //   if (this.inspection.status === 4) {
    //     this.inspectionForm.get('dueDate').disable();
    //     this.inspectionForm.get('closeDate').disable();
    //     this.inspectionForm.get('status').disable();
    //     this.inspectionForm.get('closingNotes').disable();
    //     return;
    //   }
    // }

    if (this.roleLevelLoggedUser <= 20) {

      this.statusList = [
        { 'id': 0, 'name': 'Pending' },
        { 'id': 1, 'name': 'Scheduled' }];

    } else if (this.roleLevelLoggedUser === 30 || this.roleLevelLoggedUser === 35) {

      this.statusList = [
        { 'id': 0, 'name': 'Pending' },
        { 'id': 1, 'name': 'Scheduled' }];

    } else if (this.isInspectorAssigned) {

      this.statusList = [
        { 'id': 1, 'name': 'Scheduled' },
        { 'id': 2, 'name': 'Inspector Approved' },
        { 'id': 3, 'name': 'Active' }];

    } else if (this.roleLevelLoggedUser === 40) {

      this.statusList = [
        { 'id': 3, 'name': 'Active' },
        { 'id': 4, 'name': 'Closed' }];

    }
  }

  /** DATES */
  dueDateChanged(event: string): void {
    const scheduleStatus = this.statusList.find(s => s.name === 'Scheduled');
    this.inspectionForm.patchValue({ status: scheduleStatus.id });
  }

  closeDateChanged(event: string): void {
    const scheduleStatus = this.statusList.find(s => s.name === 'Closed');
    this.inspectionForm.patchValue({ status: scheduleStatus.id });
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

}
