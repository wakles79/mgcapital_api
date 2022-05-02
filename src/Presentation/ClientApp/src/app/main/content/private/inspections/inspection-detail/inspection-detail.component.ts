import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { InspectionDetailModel } from '@app/core/models/inspections/inspection-detail.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { BehaviorSubject, forkJoin, Observable, Subscription } from 'rxjs';
import { InspectionDetailService } from './inspection-detail.service';
import { InspectionItemBaseModel } from '@app/core/models/inspection-item/inspection-item-base.model';
import { FormBuilder, FormGroup } from '@angular/forms';
import { TicketFormDialogComponent } from '@app/core/modules/ticket-form/ticket-form/ticket-form.component';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { ISendEmailConfirmDialogComponent } from '../i-send-email-confirm-dialog/i-send-email-confirm-dialog.component';
import { InspectionActivityLogGridModel } from '@app/core/models/inspections/inspection-activity-log-grid.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { TicketsService } from '../../inbox/tickets.service';
import { DatePipe, Location } from '@angular/common';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { InspectionItemComponent } from './inspection-item/inspection-item.component';
import { InspectionBaseModel, InspectionStatus } from '@app/core/models/inspections/inspection-base.model';
import { ICloseInspectionDialogComponent } from '../i-close-inspection-dialog/i-close-inspection-dialog.component';
import { InspectionItemAttachmentModel } from '@app/core/models/inspections/inspection-Item-Attachment.model';
import { TicketBaseModel, TicketDestinationType, TicketSource, TicketStatus } from '@app/core/models/ticket/ticket-base.model';
import { HttpResponse } from '@angular/common/http';
import { InspectionItemTicketModel } from '@app/core/models/inspections/inspection-item-ticket.model';
import { InspectionSendEmailModel } from '@app/core/models/inspections/inspection-send-email.model';
import { InspectionNoteModel } from '@app/core/models/inspections/inspection-note.model';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-inspection-detail',
  templateUrl: './inspection-detail.component.html',
  styleUrls: ['./inspection-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class InspectionDetailComponent implements OnInit {

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  loading$ = new BehaviorSubject<boolean>(false);


  inspectionDetail: InspectionDetailModel;

  inspectionDataChangedSubscription: Subscription;

  inspectionItemsDataSource: InspectionItemsDataSource | null;
  inspectionItemsColumnsToDisplay = [];
  columnsToDisplay = ['number', 'locationName', 'description', 'attachments', 'status', 'buttons'];
  displayedColumns: string[] = ['locationName', 'Description', 'Attachment'];
  itemData: any;
  stars = [5, 4, 3, 2, 1];
  selectStar = 2;
  dataSource: InspectionItemsDataSource | any;


  // open attachment
  iiImages: Array<IAlbum> = [];
  private _subscription: Subscription;

  InspectionIetm: InspectionItemBaseModel[] = [];
  InspectionItemAttachmentItemForm: FormGroup;

  itemFormDialog: any;
  confirmDialog: any;
  ticketDialogRef: MatDialogRef<TicketFormDialogComponent>;
  item: any;

  shareUrlDialog: MatDialogRef<ShareUrlDialogComponent>;

  sendEmailDialog: MatDialogRef<ISendEmailConfirmDialogComponent>;

  currentLevelUser: number;
  currentUserId: number;

  inspectionActivityLog: InspectionActivityLogGridModel[];

  // note
  textNote: any;
  formBuilderAttachment: any;

  inspectionNotes: any[];

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private inspectionDetailService: InspectionDetailService,
    private location: Location,
    private formBuilder: FormBuilder,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private authService: AuthService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig,
    private ticketService: TicketsService,
    private datePipe: DatePipe,
    private epochPipe: FromEpochPipe
  ) {
    this.loading$.next(true);

    this.currentLevelUser = this.authService.currentUser.roleLevel;
    this.inspectionDataChangedSubscription = this.inspectionDetailService.onInspectionDetailChanged
      .subscribe((inspectionDetailData) => {
        this.loading$.next(false);
        this.inspectionDetail = inspectionDetailData;
        this.checkForAttachments();
        this.getActivityLog();
        this.checkNotes();
      });
  }

  ngOnInit(): void {
    this.currentUserId = this.authService.currentUser.employeeId;
    this.inspectionItemsDataSource = new InspectionItemsDataSource(this.inspectionDetailService);
  }

  goBack(): void {
    this.location.back();
  }

  checkNotes(): void {
    if (!this.inspectionDetail) {
      return;
    }

    this.inspectionNotes = [];

    this.inspectionDetailService.getInspectionNotes(this.inspectionDetail.id)
      .subscribe(
        (response: { count: number, payload: InspectionActivityLogGridModel[] }) => {
          this.inspectionNotes = response.payload;
          console.log(this.inspectionNotes);
        }, (error) => {
          this.snackBar.open('Oops, there was an error, can´t load notes', 'close', { duration: 1000 });
        });
  }

  addAttachment(fullUrl: any, description: any, imageTakenDate: any, dataSource: any): void {
    const attachment = this.formBuilderAttachment.group({
      id: [-1],
      fullUrl: [fullUrl],
      description: [description],
      imageTakenDate: [imageTakenDate]
    });
    this.attachments.push(attachment);
  }

  newInspectionItemService(): void {
    this.itemFormDialog = this.dialog.open(InspectionItemComponent, {
      panelClass: 'inspection-service-form-dialog'
    });
  }

  editInspectionItem(itemId: number): void {

    this.inspectionDetailService.get(itemId, 'GetInspectionItem')
      .subscribe((itemData: any) => {
        if (itemData) {
          // tslint:disable-next-line: no-shadowed-variable
          const InspectionItemToUpdate = new InspectionItemBaseModel(itemData);
          this.itemFormDialog = this.dialog.open(InspectionItemComponent, {
            panelClass: 'inspection-service-form-dialog',
            data: {
              action: 'edit',
              inspectionItem: InspectionItemToUpdate
            }
          });
        }
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  closeInspection(): void {

    this.loading$.next(true);
    this.inspectionDetailService.get(this.inspectionDetail.id).subscribe((inspectionData: InspectionBaseModel) => {

      this.loading$.next(false);
      this.confirmDialog = this.dialog.open(ICloseInspectionDialogComponent, {
        panelClass: 'inspection-confirm-dialog',
        data: {
          action: 'close',
          inspection: inspectionData
        }
      });

      this.confirmDialog.afterClosed().subscribe((form: FormGroup) => {
        if (!form) {
          return;
        }

        this.loading$.next(true);
        inspectionData.closingNotes = form.get('closingNotes').value;
        inspectionData.closeDate = form.get('closeDate').value;
        inspectionData.status = InspectionStatus.WalkthroughComplete;
        inspectionData.stars = form.get('stars').value;

        this.inspectionDetailService.updateElement(inspectionData)
          .then(() => {
            this.inspectionDetailService.getDetails(this.inspectionDetail.id);
          },
            () => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });
    });
  }

  evaluateInspection(): void {

    this.loading$.next(true);
    this.inspectionDetailService.get(this.inspectionDetail.id)
      .subscribe((inspectionData: InspectionBaseModel) => {

        this.loading$.next(false);
        this.confirmDialog = this.dialog.open(ICloseInspectionDialogComponent, {
          panelClass: 'inspection-confirm-dialog',
          data: {
            action: 'evaluate',
            inspection: inspectionData
          }
        });

        this.confirmDialog.afterClosed().subscribe((form: FormGroup) => {
          if (!form) {
            return;
          }

          this.loading$.next(true);
          inspectionData.stars = form.get('stars').value;

          this.inspectionDetailService.updateElement(inspectionData)
            .then(() => {
              this.loading$.next(false);
              this.inspectionDetailService.getDetails(this.inspectionDetail.id);
            },
              () => {
                this.loading$.next(false);
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              })
            .catch(() => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            });
        });
      });
  }

  removeElement(InspectionItem): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });
    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.inspectionDetailService.deleteInspectionItem(InspectionItem, 'DeleteInspectionItem');
      }
      this.confirmDialogRef = null;
    });
  }

  getItemType(type: number): string {
    let status = '';
    switch (type) {
      case 0:
        status = 'Request';
        break;
      case 1:
        status = 'Complaint';
        break;
      case 2:
        status = 'Important';
        break;
      case 3:
        status = 'Other';
        break;
      case 4:
        status = 'Specialist';
        break;
      case 5:
        status = 'Observation';
        break;
    }
    return status;
  }

  // View Attachments
  open(attatchmentId: number): void {
    this._subscription = this._lightboxEvent.lightboxEvent$.subscribe((event: IEvent) => this._onReceivedEvent(event));

    const index = this.attachments.findIndex(el => el.id === attatchmentId);
    this._lightbox.open(this.iiImages, index, {
      wrapAround: true,
      showImageNumberLabel: true,
      disableScrolling: true
    });
  }

  private _onReceivedEvent(event: IEvent): void {
    if (event.id === LIGHTBOX_EVENT.CLOSE) {
      this._subscription.unsubscribe();
    }
  }

  checkForAttachments(): void {
    const attachments = this.attachments;
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

  get attachments(): InspectionItemAttachmentModel[] {
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

  // create ticket
  addTicket(inspectionItem: InspectionItemBaseModel): void {

    const ticket: TicketBaseModel = {
      id: 0,
      guid: null,
      number: null,
      source: TicketSource.Inspection,
      status: TicketStatus.pending,
      destinationType: TicketDestinationType.undefined,
      destinationEntityId: null,
      description: inspectionItem.description + '\n(From Inspection #' + this.inspectionDetail.number + ', Item #' + inspectionItem.number + ')',
      fullAddress: '',
      buildingId: this.inspectionDetail.buildingId,
      requesterFullName: this.inspectionDetail.employeeName,
      requesterEmail: this.inspectionDetail.employeeEmail,
      requesterPhone: this.inspectionDetail.employeePhone,
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

      type: inspectionItem.type,
      priority: inspectionItem.priority,
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
        action: 'fromSource',
        ticket: new TicketBaseModel(ticket)
      }
    });

    this.ticketDialogRef.afterClosed()
      .subscribe((ticketForm: FormGroup) => {

        if (!ticketForm) {
          return;
        }

        this.loading$.next(true);
        const ticketToCreate = new TicketBaseModel(ticketForm.getRawValue());

        this.ticketService.create(ticketToCreate, 'add')
          .subscribe((response: HttpResponse<any>) => {

            const inspectionTicket: InspectionItemTicketModel = {
              id: 0,
              inspectionItemId: inspectionItem.id,
              ticketId: response['body']['id'],
              destinationType: null,
              entityId: null,
            };

            this.inspectionDetailService.create(inspectionTicket, 'addTicketFromInspectionItem')
              .subscribe(() => {

                this.snackBar.open('Ticket created successfully!!!', 'close', { duration: 1000 });
                this.inspectionDetailService.getDetails(this.inspectionDetail.id);

              }, (error) => {
                this.loading$.next(false);
                this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              });

          }, (error) => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });
  }

  get urlToCopy(): string {
    return window.location.protocol + '//' + window.location.host + '/inspections/inspection-detail/' + this.inspectionDetail.guid;
  }

  displayPdf(): void {
    if (!this.inspectionDetail) {
      this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      return;
    }

    this.loading$.next(true);
    this.inspectionDetailService.getPdfReportUrl(this.inspectionDetail.id)
      .subscribe((response: string) => {
        this.loading$.next(false);
        window.open(response, '_blank');
      },
        (error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
  }

  sendUrlByEmail(): void {
    this.sendEmailDialog = this.dialog.open(ISendEmailConfirmDialogComponent, {
      disableClose: false,
      data: {
        inspectionId: this.inspectionDetail.id,
        confirmMessage: 'Are you sure you want to send this document public view link by email? Feel free to use the fields below to add additional recipients if necessary'
      }
    });

    this.sendEmailDialog.afterClosed().subscribe((formData: any) => {
      if (!formData) {
        return;
      }

      this.loading$.next(true);
      const proposalToSend = new InspectionSendEmailModel(formData.getRawValue());

      this.inspectionDetailService.sendInspectionByEmail(proposalToSend)
        .then(
          () => {
            this.loading$.next(false);
            this.snackBar.open('Inspection was send by email successfully!!!', 'close', { duration: 2000 });
            this.getActivityLog();
          },
          (error) => {
            this.loading$.next(false);
            this.snackBar.open(error, 'close', { duration: 1000 });
          })
        .catch((error) => {
          this.loading$.next(false);
          this.snackBar.open(error, 'close', { duration: 1000 });
        });

    });
  }

  shareDocument(): void {
    this.shareUrlDialog = this.dialog.open(ShareUrlDialogComponent, {
      panelClass: 'share-url-form-dialog',
      data: {
        urlToCopy: this.urlToCopy
      }
    });
  }

  openPublicDocumentViewNewTap(): void {
    window.open(this.urlToCopy, '_blank');
  }

  /** Activity Log */
  getActivityLog(): void {
    if (!this.inspectionDetail) {
      return;
    }

    this.inspectionActivityLog = [];

    this.inspectionDetailService.getInspectionActivityLog(this.inspectionDetail.id)
      .subscribe(
        (response: { count: number, payload: InspectionActivityLogGridModel[] }) => {
          this.inspectionActivityLog = response.payload;
        }, (error) => {
          this.snackBar.open('Oops, there was an error, can´t load activity log', 'close', { duration: 1000 });
        });
  }

  /** Items */
  closeInspectionItem(inspectionItem: InspectionItemBaseModel): void {
    this.loading$.next(true);
    this.inspectionDetailService.get(inspectionItem.id, 'GetInspectionItem')
      .subscribe((item: InspectionItemBaseModel) => {
        item.status = 0;

        this.inspectionDetailService.closeInspectionItem(item.id).subscribe(
          () => {
            this.snackBar.open('Inspection item closed', 'close', { duration: 1000 });
            this.inspectionDetailService.getDetails(this.inspectionDetail.id);
          }, (error) => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });

      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  countTasksCheck(itemId: number): number {
    // 100 * countTasksCheck() / tasks.length
    const itemIndex = this.inspectionDetail.inspectionItem.findIndex(i => i.id === itemId);
    if (itemIndex >= 0) {
      const completed = this.inspectionDetail.inspectionItem[itemIndex].tasks.filter(t => t.isComplete).length;
      const total = this.inspectionDetail.inspectionItem[itemIndex].tasks.length;
      if (total === 0) {
        return 0;
      }

      return 100 * completed / total;
    }

    return 0;
  }
  completedTasks(itemId: number): any {
    const itemIndex = this.inspectionDetail.inspectionItem.findIndex(i => i.id === itemId);
    if (itemIndex >= 0) {
      const completed = this.inspectionDetail.inspectionItem[itemIndex].tasks.filter(t => t.isComplete).length;
      return completed;
    }
  }

  enableToCloseTasks(itemId: number): void {
    const itemIndex = this.inspectionDetail.inspectionItem.findIndex(i => i.id === itemId);
    if (itemIndex >= 0) {
      this.inspectionDetail.inspectionItem[itemIndex].enableToCheckTasks = true;
    }
  }

  disableToCloseTasks(itemId: number): void {
    const itemIndex = this.inspectionDetail.inspectionItem.findIndex(i => i.id === itemId);
    if (itemIndex >= 0) {
      this.inspectionDetail.inspectionItem[itemIndex].enableToCheckTasks = false;
      this.inspectionDetailService.getDetails(this.inspectionDetail.id);
    }
  }

  saveTaskChanges(itemId: number): void {
    const itemIndex = this.inspectionDetail.inspectionItem.findIndex(i => i.id === itemId);
    if (itemIndex >= 0) {
      const httpRequests: any[] = [];
      this.inspectionDetail.inspectionItem[itemIndex].tasks.forEach(task => {
        httpRequests.push(
          this.inspectionDetailService.updateCompletedStatusToTask(task.id, task.isComplete)
        );
      });

      forkJoin(httpRequests).subscribe(
        (response: any[]) => {
          this.inspectionDetail.inspectionItem[itemIndex].enableToCheckTasks = false;
          this.snackBar.open('Inspection Item Tasks was updated successfully!!!', 'close', { duration: 2000 });
        }, (error) => {
          this.inspectionDetail.inspectionItem[itemIndex].enableToCheckTasks = false;
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      );
    }
  }

  addNote(textNote: any): void {
    const loggedUser = this.authService.currentUser;

    this.createNote(textNote, loggedUser.employeeId, loggedUser.employeeFullName);
    const form = this.createNote(textNote, loggedUser.employeeId, loggedUser.employeeFullName);
    const note = new InspectionNoteModel(form.getRawValue());
    note.inspectionId = this.inspectionDetail.id;
    this.inspectionDetailService.createInspectionNote(note, 'AddNote');
    this.textNote = '';
  }

  createNote(textNote: any, employeeId: any, employeeFullName: any): FormGroup {
    const now = new Date();
    return this.formBuilder.group({
      note: [textNote],
      employeeId: [employeeId],
      employeeFullName: [employeeFullName],
      createdDate: [now],
      // InspectionId: [this.inspectionDetail.id],
      // HACK: Forcing UTC date
      epochCreatedDate: [Math.floor(now.getTime() / 1000) + now.getTimezoneOffset() * 60],
    });
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

  /** PUBLIC VIEW */
  changeView(event: MatSlideToggleChange): void {
    this.loading$.next(true);
    this.inspectionDetailService.get(this.inspectionDetail.id)
      .subscribe((inspection: InspectionBaseModel) => {
        inspection.allowPublicView = event.checked;

        this.inspectionDetailService.update(inspection).subscribe(() => {
          this.loading$.next(false);
          this.snackBar.open('Public view access update successfully!', 'close', { duration: 1000 });
        }, (error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });

      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

}

export class InspectionItemsDataSource extends DataSource<any>{
  constructor(private service: InspectionDetailService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.service.onInspectionItemsChanged;
  }

  disconnect(): void { }
}
