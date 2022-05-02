import { Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { FormArray, FormGroup, AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { TicketBaseModel, TicketDestinationType, TicketSource, TicketStatus } from '@app/core/models/ticket/ticket-base.model';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { Subject, Subscription } from 'rxjs';
import { TicketsService } from '@app/main/content/private/inbox/tickets.service';
import { FilesService } from '@app/core/services/files.service';
import { BuildingsService } from '@app/main/content/private/buildings/buildings.service';
import { AuthService } from '@app/core/services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { takeUntil } from 'rxjs/operators';
import { ListBuildingModel } from '@app/core/models/building/list-buildings.model';

@Component({
  selector: 'app-ticket-form',
  templateUrl: './ticket-form.component.html',
  styleUrls: ['./ticket-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TicketFormDialogComponent implements OnInit {

  dialogTitle: string;
  ticketForm: FormGroup;
  action: string;
  ticket: TicketBaseModel;

  // attachments
  ticketImages: Array<IAlbum> = [];
  private _subscription: Subscription;

  public REF = {
    TicketSource: TicketSource,
    TicketStatus: TicketStatus,
    TicketDestinationType: TicketDestinationType,
  };

  get attachments(): FormArray {
    return this.ticketForm.get('attachments') as FormArray;
  }

  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings$: Subject<ListItemModel[]> = new Subject<ListItemModel[]>();
  get buildingId(): AbstractControl {
    return this.ticketForm.get('buildingId');
  }
  get buildingCtrl(): AbstractControl {
    return this.ticketForm.get('buildingCtrl');
  }

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  snoozeDate: Date;

  buttonSaveDisabled = false;
  constructor(
    public dialogRef: MatDialogRef<TicketFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private ticketService: TicketsService,
    private fileService: FilesService,
    private buildingService: BuildingsService,
    private authService: AuthService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig,
    public snackBar: MatSnackBar,
    private epochPipe: FromEpochPipe
  ) {
    this.action = data.action;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Ticket';
      this.ticket = data.ticket;
      this.ticket.snoozeDate = this.convertUTCToLocalTime(this.ticket.snoozeDate, this.ticket.epochSnoozeDate);
      this.ticketForm = this.createTicketUpdateForm();
    } else if (this.action === 'editFromCalendar') {
      this.dialogTitle = 'Edit Ticket';
      this.ticket = data.ticket;
      this.ticket.snoozeDate = this.convertUTCToLocalTime(this.ticket.snoozeDate, this.ticket.epochSnoozeDate);
      this.ticketForm = this.createTicketUpdateForm();
      if (new Date() > this.ticket.snoozeDate) {
        this.buttonSaveDisabled = true;
      }
    } else if (this.action === 'fromSource') {
      this.dialogTitle = 'New Ticket';
      this.ticket = data.ticket;
      this.ticketForm = this.createTicketFromSourceForm();
    }
    else {
      this.dialogTitle = 'New Ticket';
      this.ticketForm = this.createTicketCreateForm();
    }
  }

  private _onDestroy = new Subject<void>();

  ngOnInit(): void {
    this.getBuildings();
    this.checkForAttachemnts();

    this.buildingCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });
  }

  getBuildings(): void {
    this.buildingService.getAllAsList('readallcbo', '', 0, 200, this.buildingId.value, {})
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
    let search = this.buildingCtrl.value;
    if (!search) {
      this.filteredBuildings$.next(this.buildings.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.filteredBuildings$.next(
      this.buildings.filter(building => (building.name.toLowerCase() + building.fullAddress.toLowerCase()).indexOf(search) > -1)
    );
  }

  createTicketCreateForm(): FormGroup {
    return this.formBuilder.group({
      buildingId: [''],
      buildingCtrl: [''],
      fullAddress: [''],
      requesterFullName: ['', [Validators.required, Validators.maxLength(128)]],
      requesterEmail: ['', [Validators.required, Validators.maxLength(128), Validators.email]],
      requesterPhone: ['', [Validators.maxLength(13)]],
      description: ['', [Validators.required]],
      snoozeDate: [null],
      attachments: this.formBuilder.array([]),
      source: [this.REF.TicketSource['Internal Ticket']],
      status: [this.REF.TicketStatus.pending],
      location: []
    });
  }

  createTicketUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.ticket.id],
      buildingId: [this.ticket.buildingId],
      buildingCtrl: [''],
      fullAddress: [this.ticket.fullAddress],
      requesterFullName: [this.ticket.requesterFullName, [Validators.required, Validators.maxLength(128)]],
      requesterEmail: [this.ticket.requesterEmail, [Validators.required, Validators.maxLength(128)]],
      requesterPhone: [this.ticket.requesterPhone, [Validators.maxLength(13)]],
      description: [this.ticket.description, [Validators.required]],
      snoozeDate: [this.ticket.snoozeDate],
      attachments: this.formBuilder.array([]),
      source: [this.ticket.source],
      status: [this.ticket.status],
      location: [this.ticket.location]
    });
  }

  createTicketFromSourceForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.ticket.id],
      buildingId: [this.ticket.buildingId],
      buildingCtrl: [''],
      fullAddress: [this.ticket.fullAddress],
      requesterFullName: [this.ticket.requesterFullName, [Validators.required, Validators.maxLength(128)]],
      requesterEmail: [this.ticket.requesterEmail, [Validators.required, Validators.maxLength(128)]],
      requesterPhone: [this.ticket.requesterPhone, [Validators.maxLength(13)]],
      description: [this.ticket.description, [Validators.required]],
      snoozeDate: [this.ticket.snoozeDate],
      attachments: this.formBuilder.array([]),
      source: [this.ticket.source],
      status: [this.ticket.status],
      location: [this.ticket.location]
    });
  }

  /*Attachments */
  checkForAttachemnts(): void {
    if (this.ticket.attachments.length > 0) {
      const attachmentFormGroups = this.ticket.attachments.map(attachemnt => this.formBuilder.group(attachemnt));
      const attachmentFormArray = this.formBuilder.array(attachmentFormGroups);
      this.ticketForm.setControl('attachments', attachmentFormArray);

      // add images to ticketImages for carousel
      this.ticket.attachments.forEach(attachment => {

        const src = attachment.fullUrl;
        const caption = attachment.description;

        if (src && caption) {
          const album = {
            src: src,
            caption: caption,
            thumb: ''
          };
          this.ticketImages.push(album);
        }
      });
      this._lighboxConfig.fadeDuration = 1;
    }
  }

  open(index: number): void {
    this._subscription = this._lightboxEvent.lightboxEvent$.subscribe((event: IEvent) => this._onReceivedEvent(event));

    // override the default config
    this._lightbox.open(this.ticketImages, index, { wrapAround: true, showImageNumberLabel: true });
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
      this.fileService.uploadFile(files)
        .subscribe((response: any) => {
          if (response.status === 200 || response.status === 206) {
            for (let i = 0; i < response.body.length; i++) {
              // add images to ticketImages for carousel
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
              this.ticketImages.push(album);
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
    (this.ticketForm.get('attachments') as FormArray).removeAt(index);
    this.ticketImages.splice(index, 1);
    // If id of deleted attachment was -1, it's necessary deleted it from azure
    if (attachmentToDelete.get('id').value === -1) {
      this.fileService.deleteAttachmentByBlobName(attachmentToDelete.get('blobName').value).subscribe();
    }
  }
  /* End Attachments */

  closeDialog(): void {
    if (this.action === 'fromSource') {
      this.dialogRef.close();
    } else {
      this.dialogRef.close(['close', this.ticketForm]);
    }

    // Delete images uploaded in azure but not saved in data base
    for (let i = 0; i < this.attachments.value.length; i++) {
      if (this.attachments.at(i).get('id').value === -1) {
        this.fileService.deleteAttachmentByBlobName(this.attachments.at(i).get('blobName').value).subscribe();
      }
    }
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
