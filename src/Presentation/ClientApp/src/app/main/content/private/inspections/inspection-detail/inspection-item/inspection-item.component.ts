import { Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSelect } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { InspectionItemBaseModel } from '@app/core/models/inspection-item/inspection-item-base.model';
import { InspectionDetailModel } from '@app/core/models/inspections/inspection-detail.model';
import { InspectionItemAttachmentModel } from '@app/core/models/inspections/inspection-Item-Attachment.model';
import { TicketBaseModel, TicketDestinationType, TicketSource, TicketStatus } from '@app/core/models/ticket/ticket-base.model';
import { WORK_ORDERS_PRIORITIES } from '@app/core/models/work-order/work-order-priorities.model';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import { TicketFormDialogComponent } from '@app/core/modules/ticket-form/ticket-form/ticket-form.component';
import { AuthService } from '@app/core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { IAlbum, IEvent, Lightbox, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { BehaviorSubject, ReplaySubject, Subject, Subscription } from 'rxjs';
import { InspectionDetailService } from '../inspection-detail.service';

@Component({
  selector: 'app-inspection-item',
  templateUrl: './inspection-item.component.html',
  styleUrls: ['./inspection-item.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class InspectionItemComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  action: string;
  InspectionItem: any;
  // Indicate the type of the cleaning report item, could be 'cleaningItem' or 'findingItem'
  itemType: any;
  InspectionId = 0;


  // customerId (Management Co.) to filter the buildings to display
  customerContactId: any;

  buildingsListSubscription: Subscription;
  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings$: ReplaySubject<ListItemModel[]> = new ReplaySubject<ListItemModel[]>(1);
  get buildingCtrl(): any { return this.InspectionItemForm.get('buildingCtrl'); }
  get selectedBuildingId(): any { return this.InspectionItemForm.get('buildingId'); }
  @ViewChild('buildingSelect') buildingSelect: MatSelect;

  // Show or hide attachments section
  showAttachments: boolean;
  selectedFile: File;

  private _subscription: Subscription;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  // wildcard to disable the form from a component parent
  buttonSaveDisabled = false;

  disableButtons = false;

  // Attachment
  iiImages: Array<IAlbum> = [];
  InspectionItemForm: FormGroup;

  inspectionDataChangedSubscription: Subscription;

  loading$ = new BehaviorSubject<boolean>(false);

  inspectionDetail: InspectionDetailModel;

  ticketDialogRef: MatDialogRef<TicketFormDialogComponent>;

  // Task
  taskDescription: any;

  // Priority
  workOdersPriorities: { id: number, name: string }[] = [];
  workOrderTypes: { key: number, value: string }[] = [];


  // notes
  textNote: any;

  constructor(
    public dialogRef: MatDialogRef<InspectionItemComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private inspectionDetailService: InspectionDetailService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private authService: AuthService,
  ) {
    this.getPrioritiesWO();
    this.getTypeWO();
    this.inspectionDataChangedSubscription = this.inspectionDetailService.onInspectionDetailChanged
      .subscribe((inspectionDetailData) => {
        this.loading$.next(false);
        this.inspectionDetail = inspectionDetailData;
        this.checkForAttachments();
      });

    if (this.data !== null) {
      this.action = this.data.action;
    } else {
      this.action = 'Add';
    }
    if (this.action === 'Add') {
      this.InspectionItemForm = this.createInspectionItemCreateForm();
    } else {
      this.InspectionItem = this.data.inspectionItem;
      this.InspectionId = this.InspectionItem.InspectionId;
      this.InspectionItemForm = this.InspectionItemUpdateForm();
    }
  }

  ngOnInit(): void {
    if (this.InspectionItemForm.get('type').value === 5) {

    } else {
      this.onChangePriority(this.InspectionItemForm.get('type').value);
    }
    if (this.action === 'edit') {
      this.checkForAttachemnts();
      this.checkTask();
      this.checkNotes();
    }
  }

  checkForAttachemnts(): void {
    if (this.InspectionItem.attachments.length > 0) {
      const attachmentFormGroups = this.InspectionItem.attachments.map(attachemnt => this.formBuilder.group(attachemnt));
      const attachmentFormArray = this.formBuilder.array(attachmentFormGroups);
      this.InspectionItemForm.setControl('attachments', attachmentFormArray);
    }
  }
  checkTask(): void {
    if (this.InspectionItem.tasks.length > 0) {
      const taskFormGroups = this.InspectionItem.tasks.map(task => this.formBuilder.group(task));
      const taskFormArray = this.formBuilder.array(taskFormGroups);
      this.InspectionItemForm.setControl('tasks', taskFormArray);
    }
  }

  checkNotes(): void {
    if (this.InspectionItem.notes.length > 0) {
      const noteFormGroups = this.InspectionItem.notes.map(note => this.formBuilder.group(note));
      const noteFormArray = this.formBuilder.array(noteFormGroups);
      this.InspectionItemForm.setControl('notes', noteFormArray);
    }
  }

  ngOnDestroy(): void {
    throw new Error('Method not implemented.');
  }

  InspectionItemUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.InspectionItem.id],
      position: [this.InspectionItem.position, [Validators.required]],
      description: [this.InspectionItem.description, [Validators.required]],
      attachments: this.formBuilder.array([]),
      number: [this.InspectionItem.number],
      tasks: this.formBuilder.array([]),
      type: [this.InspectionItem.type],
      priority: [this.InspectionItem.priority],
      notes: this.formBuilder.array([])
    });
  }

  createInspectionItemCreateForm(): FormGroup {
    return this.formBuilder.group({
      position: ['', [Validators.required]],
      longitude: [''],
      latitude: [''],
      description: ['', [Validators.required]],
      attachments: this.formBuilder.array([]),
      location: [],
      priority: [WORK_ORDERS_PRIORITIES.Low],
      type: [5],
      tasks: this.formBuilder.array([]),
      notes: this.formBuilder.array([]),
    });
  }

  openInputFile(): void {
    document.getElementById('fileInput').click();
  }

  onNoClick(): void {
    if (this.action === 'edit') {
      const InspectionItemToCreate = new InspectionItemBaseModel(this.InspectionItemForm.getRawValue());
      InspectionItemToCreate.InspectionId = this.inspectionDetail.id;
      this.inspectionDetailService.updateInspectionItem(InspectionItemToCreate, 'UpdateInspectionItem');
      this.dialogRef.close(this.InspectionItemForm.value);
    } else {
      const InspectionItemToCreate = new InspectionItemBaseModel(this.InspectionItemForm.getRawValue());
      // this.cleaningReportDetailsService.createCleaningReportItem(cleaningReportItemToCreate, 'AddCleaningReportItem');
      InspectionItemToCreate.InspectionId = this.inspectionDetail.id;
      this.inspectionDetailService.createInspectionItem(InspectionItemToCreate, 'AddInspectionItem');
      this.dialogRef.close(this.InspectionItemForm.value);
    }
  }

  fileChange(files: File[]): void {
    if (files.length > 0) {
      this.inspectionDetailService.uploadFile(files)
        .subscribe((response: any) => {
          if (response.status === 200 || response.status === 206) {
            for (let i = 0; i < response.body.length; i++) {
              // add images to iiImages for carousel
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
              this.iiImages.push(album);
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

  onFileChanged(event: { target: { files: File[]; }; }): void {
    this.selectedFile = event.target.files[0];
  }

  addAttachment(fullUrl: any, tittle: any, imageTakenDate: any, blobName: any): void {
    const attachment = this.formBuilder.group({
      id: [-1],
      fullUrl: [fullUrl],
      title: [tittle],
      imageTakenDate: [imageTakenDate],
      blobName: [blobName]
    });
    this.
      attachments.push(attachment);
  }

  get attachments(): FormArray {
    return this.InspectionItemForm.get('attachments') as FormArray;
  }

  removeAttachment(index): void {
    const attachmentToDelete = this.attachments.at(index);
    (this.InspectionItemForm.get('attachments') as FormArray).removeAt(index);
    this.iiImages.splice(index, 1);
    // If id of deleted attachment was -1, it's necessary deleted it from azure
    if (attachmentToDelete.get('id').value === -1) {
      this.inspectionDetailService.deleteAttachmentByBlobName(attachmentToDelete.get('blobName').value).subscribe();
    }
  }

  closeDialog(): void {
    this.dialogRef.close(['close', this.InspectionItemForm]);
    // Delete images uploaded in azure but not saved in data base
    for (let i = 0; i < this.attachments.value.length; i++) {
      if (this.attachments.at(i).get('id').value === -1) {
        this.inspectionDetailService.deleteAttachmentByBlobName(this.attachments.at(i).get('blobName').value).subscribe();
      }
    }
  }

  addTicket(inspectionItem: InspectionItemBaseModel): void {

    const ticket: TicketBaseModel = {
      id: null,
      guid: null,
      number: null,
      source: TicketSource.Inspection,
      status: TicketStatus.pending,
      destinationType: TicketDestinationType.undefined,
      destinationEntityId: null,
      description: 'From Inspection #' + this.inspectionDetail.number,
      fullAddress: '',
      buildingId: this.inspectionDetail.buildingId,
      requesterFullName: this.inspectionDetail.employeeName,
      requesterEmail: this.inspectionDetail.employeeName,
      requesterPhone: this.inspectionDetail.employeeName,
      snoozeDate: null,
      attachments: inspectionItem.attachments,
      attachmentsCount: inspectionItem.attachments.length,

      entityNumber: null,
      buildingName: this.inspectionDetail.buildingName,
      epochSnoozeDate: null,
      epochCreatedDate: null,
      epochUpdatedDate: null,

      data: null,
      location: inspectionItem.position,
      tasks: inspectionItem.tasks,
      priority: inspectionItem.priority,
      type: inspectionItem.type,
      freshdeskTicketId: null,
      assignedEmployeeId: null,
      pendingReview: false,
      assignedEmployeeName: null,
      newRequesterResponse: false,
      messageId: null
    };

    this.ticketDialogRef = this.dialog.open(TicketFormDialogComponent, {
      panelClass: 'ticket-form-dialog',
      data: {
        action: 'edit',
        ticket: new TicketBaseModel(ticket)
      }
    });
  }

  open(index: number): void {
    this._subscription = this._lightboxEvent.lightboxEvent$.subscribe((event: IEvent) => this._onReceivedEvent(event));

    // override the default config
    this._lightbox.open(this.iiImages, index, { wrapAround: true, showImageNumberLabel: true });
  }

  private _onReceivedEvent(event: IEvent): void {
    if (event.id === LIGHTBOX_EVENT.CLOSE) {
      this._subscription.unsubscribe();
    }
  }

  get indexattachments(): InspectionItemAttachmentModel[] {
    let attachments: InspectionItemAttachmentModel[] = [];

    if (this.inspectionDetail) {
      // Gets report item attachments
      if (this.inspectionDetail.inspectionItem) {
        this.inspectionDetail.inspectionItem.forEach(cI => {
          attachments = attachments.concat(cI.attachments);
        });
      }
    }
    return attachments;
  }

  checkForAttachments(): void {
    const attachments = this.indexattachments;
    if (attachments.length > 0) {
      // Clean images
      this.iiImages = [];

      // add images to woImages for carousel
      attachments.forEach(attachment => {
        const src = attachment.fullUrl;
        const caption = attachment.title;
        const album = {
          src: src,
          caption: caption,
          thumb: ''
        };
        this.iiImages.push(album);
      });
    }
  }


  getPrioritiesWO(): void {
    /*Covert enum WORK_ORDERS_PRIORITIES to array workOdersPriorities*/
    for (const n in WORK_ORDERS_PRIORITIES) {
      if (typeof WORK_ORDERS_PRIORITIES[n] === 'number') {
        this.workOdersPriorities.push({ id: WORK_ORDERS_PRIORITIES[n] as any, name: n.replace(/_/g, ' ') });
      }
    }
  }

  getTypeWO(): void {
    // tslint:disable-next-line: forin
    for (const n in WORK_ORDER_TYPES) {
      const NumberType = WORK_ORDER_TYPES[n].key;
      if (NumberType === 1 ||
        NumberType === 2 ||
        NumberType === 5) {
        this.workOrderTypes.push({ key: WORK_ORDER_TYPES[n].key as any, value: WORK_ORDER_TYPES[n].value as any });
      }
    }
  }

  onChangePriority(value: number): void {
    switch (value) {
      case 1:
        this.InspectionItemForm.patchValue({
          priority: 1
        });
        this.InspectionItemForm.get('priority').disable();
        break;
      case 2:
        this.InspectionItemForm.patchValue({
          priority: 1
        });
        this.InspectionItemForm.get('priority').disable();
        break;
      default:
        this.InspectionItemForm.patchValue({
          priority: 3
        });
        this.InspectionItemForm.get('priority').enable();
        break;
    }
  }

  // task
  get tasks(): FormArray {
    return this.InspectionItemForm.get('tasks') as FormArray;
  }

  addTask(description: any): void {

    if (!description) {
      this.snackBar.open('Oops, Invalid task.', 'close', { duration: 1000 });
      return;
    }

    const task = this.createTask(description);
    this.tasks.push(task);
    this.taskDescription = '';
  }

  createTask(description: any): FormGroup {
    return this.formBuilder.group({
      description: [description],
      isComplete: [false]
    });
  }

  deleteTask(index: any): void {
    (this.InspectionItemForm.get('tasks') as FormArray).removeAt(index);
  }

  // notes

  addNote(textNote: any): void {
    const loggedUser = this.authService.currentUser;
    const note = this.createNote(textNote, loggedUser.employeeId, loggedUser.employeeFullName);
    this.notes.push(note);
    this.textNote = '';
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
    return this.InspectionItemForm.get('notes') as FormArray;
  }

}
