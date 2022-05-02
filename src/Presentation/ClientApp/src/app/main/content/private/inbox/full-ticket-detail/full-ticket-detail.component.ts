import { Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { TicketBaseModel, TicketDestinationType, TicketSource, TicketStatus, TicketStatusColor } from '@app/core/models/ticket/ticket-base.model';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { TicketReplyFormComponent } from './ticket-reply-form/ticket-reply-form.component';
import { FdTicketBaseModel } from '@app/core/models/freshdesk/fd-ticket-base.model';
import { GmailTicket } from '@app/core/models/gmail/gmail-ticket.interface';
import { FdConversationBaseModel } from '@app/core/models/freshdesk/fd-conversation-base.model';
import { MergeTicketDialogComponent } from './merge-ticket-dialog/merge-ticket-dialog.component';
import { TicketFormDialogComponent } from '@app/core/modules/ticket-form/ticket-form/ticket-form.component';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { FormArray, FormGroup, AbstractControl, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { TicketTagAssignment } from '@app/core/models/tag/ticket-tag-assignment.model';
import { IAlbum, IEvent, Lightbox, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { WorkOrderSequencesDialogComponent } from '@app/core/modules/work-order-dialog/work-order-sequences-dialog/work-order-sequences-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FullTicketDetailService } from './full-ticket-detail.service';
import { ActivatedRoute } from '@angular/router';
import { BuildingsService } from '../../buildings/buildings.service';
import { FilesService } from '@app/core/services/files.service';
import { AuthService } from '@app/core/services/auth.service';
import { TicketsService } from '../tickets.service';
import { UsersBaseService } from '../../users/users-base.service';
import { FromEpochPipe } from '@app/core/pipes/fromEpoch.pipe';
import { TagsService } from '../../tags/tags.service';
import { DatePipe, Location } from '@angular/common';
import { DomSanitizer } from '@angular/platform-browser';
import { takeUntil } from 'rxjs/operators';
import { FdTicketDetailModel } from '@app/core/models/freshdesk/fd-ticket-detail.model';
import { FdAttachmentBaseModel } from '@app/core/models/freshdesk/fd-attachment-base.model';
import { TicketUpdateModel } from '@app/core/models/ticket/ticket-update.model';
import { TicketAttachmentBaseModel } from '@app/core/models/ticket/ticket-attachment-base.model';
import { FdTicketReplyModel } from '@app/core/models/freshdesk/fd-ticket-reply.model';
import { ListBuildingModel } from '@app/core/models/building/list-buildings.model';
import { TicketActivityDialogComponent } from '../full-ticket-detail/ticket-activity-dialog/ticket-activity-dialog.component';

export enum EditorMode {
  FreshdeskReply,
  InternalMessage,
  ForwardTicket,
  ShareTicket
}

@Component({
  selector: 'app-full-ticket-detail',
  templateUrl: './full-ticket-detail.component.html',
  styleUrls: ['./full-ticket-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class FullTicketDetailComponent implements OnInit, OnDestroy {

  loading$ = new BehaviorSubject<boolean>(true);
  replyTicketDialog: MatDialogRef<TicketReplyFormComponent>;

  ticketId: number = 0;
  ticket: TicketBaseModel = null;
  freshdeskTicket: FdTicketBaseModel = null;
  gmailTicket: GmailTicket = null;
  conversations: FdConversationBaseModel[] = [];

  replyData: { to: string, message: string, signature?: string, cc: string[] } = null;

  @ViewChild('replyContainer') replyContainer: ElementRef<HTMLDivElement>;
  @ViewChild('ticketDetail') ticketDetail: ElementRef<HTMLDivElement>;

  // Ticket
  public REF = {
    TicketSource: TicketSource,
    TicketStatus: TicketStatus,
    TicketDestinationType: TicketDestinationType,
    TicketStatusColor: TicketStatusColor
  };
  convertMode = false;

  showDetails = true;
  showTicketReply = false;

  mergeTicketDialog: MatDialogRef<MergeTicketDialogComponent>;

  extensionRegex = new RegExp(/(?:\.([^.]+))?$/);
  imageExtensions: { ext: string, type: string }[] =
    [
      { ext: 'jpeg', type: 'image/jpeg' },
      { ext: 'jpg', type: 'image/jpg' },
      { ext: 'png', type: 'image/png' },
      { ext: 'gif', type: 'image/gif' }
    ];
  fileExtensions: { ext: string, type: string }[] =
    [
      { ext: 'jpeg', type: 'image/jpeg' },
      { ext: 'jpg', type: 'image/jpg' },
      { ext: 'png', type: 'image/png' },
      { ext: 'gif', type: 'image/gif' },
      { ext: 'csv', type: 'text/csv' },
      { ext: 'pdf', type: 'application/pdf' },
      { ext: 'docx', type: 'application/msword' }
    ];

  ticketFormDialog: MatDialogRef<TicketFormDialogComponent>;
  confirmDialog: MatDialogRef<FuseConfirmDialogComponent>;

  // All the emails from freshdesk ticket
  ticketEmails: string[] = [];

  // Ticket Form
  ticketForm: FormGroup;

  get attachments(): FormArray {
    return this.ticketForm.get('attachments') as FormArray;
  }

  // Building
  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings: { id: number, name: string, fullAddress: string }[] = [];

  get buildingId(): AbstractControl {
    return this.ticketForm.get('buildingId');
  }

  buildingSearch: FormControl;

  // Employee
  employees: ListItemModel[] = [];
  filteredEmployees: ListItemModel[] = [];

  get employeeId(): AbstractControl {
    return this.ticketForm.get('assignedEmployeeId');
  }
  get employeeCtrl(): AbstractControl {
    return this.ticketForm.get('employeeCtrl');
  }
  employeeSearch: FormControl;

  // Text Editor Mode
  editorMode: EditorMode;
  threadEmail: string;

  private _onDestroy = new Subject<void>();

  // Tags
  tags: { id: number, name: string, assigned: boolean }[] = [];
  filteredTags: { id: number, name: string, assigned: boolean }[] = [];
  assignedTags: TicketTagAssignment[] = [];

  // Company
  companyName: string = '';

  // Lightbox
  lightboxAlbum: Array<IAlbum> = [];
  private _lightboxSubscription: Subscription;

  // Work Order Sequence
  workOrderSequencesDialog: MatDialogRef<WorkOrderSequencesDialogComponent>;

  private emailRegexp = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);

  dialogRef: any;

  constructor(
    private _location: Location,
    private _dialog: MatDialog,
    public _snackBar: MatSnackBar,
    private _ticketService: FullTicketDetailService,
    private _route: ActivatedRoute,
    private _formBuilder: FormBuilder,
    private _buildingService: BuildingsService,
    private _fileService: FilesService,
    private _authService: AuthService,
    private _mgTicketService: TicketsService,
    private _userBaseService: UsersBaseService,
    private _epochPipe: FromEpochPipe,
    private _tagService: TagsService,
    private _datePipe: DatePipe,
    private sanitized: DomSanitizer,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
  ) {
    this.ticketForm = this.createTicketUpdateForm();

    this.buildingSearch = new FormControl('');
    this.employeeSearch = new FormControl('');

    const user = this._authService.currentUser;
    if (user) {
      this.companyName = user.companyName;
    }
  }

  ngOnInit(): void {
    this.ticketId = Number(this._route.snapshot.data['ticket']);
    this.getTicketDetails(this.ticketId);

    this.getBuildings();
    this.getEmployees();
    this.getTags();
    this.getTicketTags();

    this.buildingSearch.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(value => {
        this.filterBuildings();
      });


    this.employeeSearch.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterEmployees();
      });

    this._mgTicketService.onTicketUpdated.subscribe(status => {
      if (status === 'converted') {
        this.getTicketDetails(this.ticketId);
      }
    });
  }

  ngOnDestroy(): void {
  }

  // Location
  goBack(): void {
    this._location.back();
  }

  // Ticket
  getTicketDetails(id: number): void {
    this.loading$.next(true);
    this.convertMode = false;
    this.ticketEmails = [];
    this._ticketService.get(id, 'GetTicketDetail')
      .subscribe((response: any) => {

        if (!response) {
          return;
        }

        this.ticket = new TicketBaseModel(response);
        this.ticket.snoozeDate = this.convertUTCToLocalTime(this.ticket.snoozeDate, this.ticket.epochSnoozeDate);

        this.setValuesToTicketForm();
        this.checkForAttachemnts();
        this.checkForAttachmentsToAlbum();

        try {
          if (response.gMailTicket) {
            this.gmailTicket = response.gMailTicket;
          }
          if (response.ticketFreshdesk) {
            const freshdeskTicketObj = new FdTicketDetailModel(response.ticketFreshdesk);
            this.freshdeskTicket = freshdeskTicketObj.ticket ? new FdTicketBaseModel(freshdeskTicketObj.ticket) : null;
            this.conversations = [];
            freshdeskTicketObj.conversations.forEach(c => {
              if (!c.private) {
                c.body_html = this.createBodyHTML(c.body);
                c.body_blockquote = this.createBodyBlockquote(c.body);
                this.conversations.push(new FdConversationBaseModel(c));
              }
            });
            this.createFullThread();
          }
        } catch (error) { console.log(error); }

        this.addFdTicketEmail();
        this.loading$.next(false);
      }, (error) => {
        this.loading$.next(false);
        this._snackBar.open('Oops! there was an error', 'close', { duration: 2000 });
      });
  }

  get urlByDestination(): string {
    let url = window.location.protocol + '//' + window.location.host + '/';

    if (this.ticket.destinationType === this.REF.TicketDestinationType.workOrder) {
      url = url + 'app/work-orders?action=edit&woId=' + this.ticket.destinationEntityId;
    }
    else if (this.ticket.destinationType === this.REF.TicketDestinationType.cleaningItem ||
      this.ticket.destinationType === this.REF.TicketDestinationType.findingItem) {

      url = url + 'app/reports/cleaning-report/' + this.ticket.data.cleaningReportId + '?action=edit&itemId=' + this.ticket.destinationEntityId;
    }
    return url;
  }

  addFdTicketEmail(): void {
    if (this.freshdeskTicket) {
      // this.verifyEmailExists(this.freshdeskTicket.email);
      this.freshdeskTicket.cc_emails.forEach(e => {
        this.verifyEmailExists(e);
      });

      this.conversations.forEach(c => {
        if (c.incoming) {
          this.verifyEmailExists(c.support_email);
        }

        c.cc_emails.forEach(e => {
          this.verifyEmailExists(e);
        });
      });
    }
  }

  verifyEmailExists(email: string): void {
    if (email === '' || !email || email === this.freshdeskTicket.email || !this.emailRegexp.test(email)) {
      return;
    }

    const index = this.ticketEmails.findIndex(e => e === email);
    if (index < 0) {
      this.ticketEmails.push(email);
    }
  }

  scroll(el: HTMLElement): void {
    // const topPos = this.ticketDetail.nativeElement.offsetTop;
    // el.scrollIntoView({ behavior: 'smooth' });
    el.scrollIntoView(true);
    // el.scrollTop = topPos - 10;
  }

  // Buttons
  sendTicketReply(): void {
    if (this.showTicketReply) {
      this.showTicketReply = false;
      this.replyData = null;
    } else {
      this.editorMode = EditorMode.FreshdeskReply;
      this.replyData = {
        to: this?.gmailTicket?.replyTo ?? this.freshdeskTicket.email,
        message: '',
        signature: this.ticket.emailSignature,
        cc: []
      };
      this.showTicketReply = true;
    }
    this.scroll(this.replyContainer.nativeElement);
  }

  // Buttons
  sendTicketReplyAll(): void {
    if (this.showTicketReply) {
      this.showTicketReply = false;
      this.replyData = null;
    } else {
      this.editorMode = EditorMode.FreshdeskReply;
      this.replyData = {
        to: this?.gmailTicket?.replyTo ?? this.freshdeskTicket.email,
        message: '',
        signature: this.ticket.emailSignature,
        cc: this.ticketEmails
      };
      this.showTicketReply = true;
    }
    this.scroll(this.replyContainer.nativeElement);
  }

  forwardTicket(): void {
    if (this.showTicketReply) {
      this.showTicketReply = false;
      this.replyData = null;
    } else {
      this.editorMode = EditorMode.ForwardTicket;

      const emailConversations = this.conversations.filter(c => !c.fromActivityLog && c.incoming);
      const lastMessage = emailConversations.length > 0 ? emailConversations[emailConversations.length - 1].body : '';

      // tslint:disable-next-line: max-line-length
      // Please take a look at ticket <a href=\"${window.location.protocol}//${window.location.host}/app/inbox/ticket-detail/${this.ticketId}\" target=\"_blank\">${this.ticket.number}</a> <br/>
      const message: string = `${lastMessage}`;
      this.replyData = {
        to: '',
        message: message,
        signature: this.ticket.emailSignature,
        cc: []
      };
      this.showTicketReply = true;
    }
    this.scroll(this.replyContainer.nativeElement);
  }

  shareTicket(): void {
    if (this.showTicketReply) {
      this.showTicketReply = false;
      this.replyData = null;
    } else {
      this.editorMode = EditorMode.ShareTicket;

      const emailConversations = this.conversations.filter(c => !c.fromActivityLog && c.incoming);
      const lastMessage = emailConversations.length > 0 ? emailConversations[emailConversations.length - 1].body : '';

      // tslint:disable-next-line: max-line-length
      // Please take a look at ticket <a href=\"${window.location.protocol}//${window.location.host}/app/inbox/ticket-detail/${this.ticketId}\" target=\"_blank\">${this.ticket.number}</a> <br/>
      const message: string = `Please take a look at ticket <a href=\"${window.location.protocol}//${window.location.host}/app/inbox/ticket-detail/${this.ticketId}\" target=\"_blank\">${this.ticket.number}</a> <br/> ${lastMessage}`;
      this.replyData = {
        to: '',
        message: message,
        signature: this.ticket.emailSignature,
        cc: []
      };
      this.showTicketReply = true;
    }
    this.scroll(this.replyContainer.nativeElement);
  }

  mergeTicket(): void {
    this.mergeTicketDialog = this._dialog.open(MergeTicketDialogComponent, {
      panelClass: 'merge-ticket-dialog',
      data: {
        ticketId: this.ticket.id,
        ticketNumber: this.ticket.number
      }
    });

    this.mergeTicketDialog.afterClosed()
      .subscribe((result: { parent: number, child: number[] }) => {
        if (!result) {
          return;
        }

        const obj: { ticketId: number, ticketsId: number[] } = { ticketId: result.parent, ticketsId: result.child };
        this._ticketService.mergeTickets(obj)
          .subscribe((response: any) => {
            this.loading$.next(false);
            this._snackBar.open('Ticket merged successfully', 'close', { duration: 2000 });
            this.getTicketDetails(this.ticketId);
          }, (error) => {
            this.loading$.next(false);
            this._snackBar.open('Oops! there was an error', 'close', { duration: 2000 });
          });
      });
  }

  copyImageToTicket(url: string, name: string): void {
    this.loading$.next(true);
    const extension = this.extensionRegex.exec(name)[1];
    const fileType = this.imageExtensions.find(e => e.ext === extension);

    if (!fileType) {
      this.loading$.next(false);
      this._snackBar.open('File not available for copy - ' + extension, 'close', { duration: 2000 });
      return;
    }

    const copyObject = {
      ticketId: this.ticket.id,
      url: url,
      fileType: fileType.type,
      fileName: name
    };

    this._ticketService.copyImageToTicket(copyObject)
      .subscribe((response) => {
        this.loading$.next(false);
        this.getTicketDetails(this.ticketId);
      }, (error) => {
        this.loading$.next(false);
        this._snackBar.open('Oops! there was an error', 'close', { duration: 2000 });
      });
  }

  copyAllFdTicketAttachmentsToTicket(id: number): void {

  }

  copyAllConversationAttachmentsToTicket(id: number): void {
    this.loading$.next(true);
    const index = this.conversations.findIndex(c => c.id === id);
    if (index >= 0) {
      const attachments = this.conversations[index].attachments;
      this.copyAttachmentsToTicket(attachments);
    } else {
      this.loading$.next(false);
    }
  }

  private async copyAttachmentsToTicket(attachments: FdAttachmentBaseModel[]): Promise<any> {
    for (let index = 0; index < attachments.length; index++) {
      const attachment = attachments[index];
      const extension = this.extensionRegex.exec(attachment.name)[1];
      const fileType = this.imageExtensions.find(e => e.ext === extension);

      if (fileType) {
        const copyObject = {
          ticketId: this.ticket.id,
          url: attachment.attachment_url,
          fileType: fileType.type,
          fileName: name
        };

        try {
          await this._ticketService.copyImageToTicket(copyObject).toPromise();
        }
        catch (error) {
          console.log('error' + attachment.id);
        }
      }
    }
    this.getTicketDetails(this.ticketId);
  }

  downloadImage(url: string, name: string): void {
    this.loading$.next(true);

    const extension = this.extensionRegex.exec(name)[1];
    const type = this.fileExtensions.find(e => e.ext === extension);

    if (!type) {
      this.loading$.next(false);
      this._snackBar.open('File not available for download - ' + extension, 'close', { duration: 2000 });
      return;
    }

    this._ticketService.downloadImage(url, name, type.type)
      .subscribe((response) => {
        this.downloadImageBase64(response, type.type, name);
        this.loading$.next(false);
      }, (error) => {
        this.loading$.next(false);
        console.log('error' + JSON.stringify(error));
        this._snackBar.open('Oops! there was an error', 'close', { duration: 2000 });
      });
  }

  downloadImageBase64(data: any, fileType: string, name: string): void {
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    const url = window.URL.createObjectURL(data);
    a.href = url;
    a.download = name;
    a.click();
    document.body.removeChild(a);
  }

  editTicket(): void {
    this.ticketFormDialog = this._dialog.open(TicketFormDialogComponent, {
      panelClass: 'ticket-form-dialog',
      data: {
        action: 'edit',
        ticket: this.ticket
      }
    });

    this.ticketFormDialog.afterClosed()
      .subscribe(response => {
        if (!response) {
          return;
        }
        const actionType: string = response[0];
        const formData: FormGroup = response[1];
        const updatedTicketObj = new TicketUpdateModel(formData.getRawValue());
        switch (actionType) {

          case 'save':
            this.loading$.next(true);
            this._ticketService.updateElement(updatedTicketObj)
              .then(
                () => {
                  this.loading$.next(false);
                  this._snackBar.open('Ticket updated successfully!!!', 'close', { duration: 1000 });
                  this.getTicketDetails(this.ticketId);
                },
                () => {
                  this.loading$.next(false);
                  this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                })
              .catch(() => {
                this.loading$.next(false);
                this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
              });
            break;

          case 'delete':
            this.archiveTicket();
            break;
        }
      });
  }

  resolveTicket(ticketStatus: TicketStatus): void {
    this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialog.componentInstance.confirmMessage = 'Are you sure you want to update the selected ticket status?';

    this.confirmDialog.afterClosed().subscribe(result => {
      if (result) {
        this.loading$.next(true);
        this._ticketService.updateTicketStatus({ id: this.ticket.id, status: ticketStatus })
          .then(
            () => {
              this.loading$.next(false);
              this._snackBar.open('Ticket updated successfully!!!', 'close', { duration: 1000 });
              this.getTicketDetails(this.ticketId);
            },
            (error) => {
              this.loading$.next(false);
              this._snackBar.open(error, 'close', { duration: 1000 });
            }
          ).catch(
            (error) => {
              this.loading$.next(false);
              this._snackBar.open(error, 'close', { duration: 1000 });
            }
          );
      }
      this.confirmDialog = null;
    });
  }

  convertTicket(destination: TicketDestinationType): void {
    this.convertMode = true;
    // console.log(this.ticket, 'this.ticket');
    this._mgTicketService.onCurrentTicketChanged.next(this.ticket);
    this._mgTicketService.onTicketDestinationChanged.next(destination);
  }

  archiveTicket(): void {
    this.confirmDialog = this._dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialog.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialog.afterClosed().subscribe(result => {
      if (result) {
        this.loading$.next(true);
        this._ticketService.deleteTicket(this.ticket.id)
          .then(
            () => {
              this.loading$.next(false);
              this._snackBar.open('Ticket deleted successfully!!!', 'close', { duration: 1000 });
              this.getTicketDetails(this.ticketId);
            },
            (error) => {
              this.loading$.next(false);
              this._snackBar.open(error, 'close', { duration: 1000 });
            }
          ).catch(
            (error) => {
              this.loading$.next(false);
              this._snackBar.open(error, 'close', { duration: 1000 });
            }
          );
      }
      this.confirmDialog = null;
    });
  }

  updateTicket(): void {
    const updatedTicketObj = new TicketUpdateModel(this.ticketForm.getRawValue());
    this.loading$.next(true);
    this._ticketService.updateElement(updatedTicketObj)
      .then(
        () => {
          this.loading$.next(false);
          this._snackBar.open('Ticket updated successfully!!!', 'close', { duration: 1000 });
          this.getTicketDetails(this.ticketId);
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

  fileChange(files: File[]): void {
    if (files.length > 0) {

      this.loading$.next(true);
      this._ticketService.addTicketAttachment(this.ticket.id, files)
        .subscribe(
          (response) => {
            const attachemnt = new TicketAttachmentBaseModel(response.body);
            this.ticket.attachments.push(attachemnt);

            this.checkForAttachemnts();

            const src = attachemnt.fullUrl;
            const caption = attachemnt.description || 'No title';
            const album = {
              src: src,
              caption: caption,
              thumb: ''
            };
            this.lightboxAlbum.push(album);

            this.loading$.next(false);
          }, (error) => {
            this.loading$.next(false);
            this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          }
        );

      // this._fileService.uploadFile(files)
      //   .subscribe((response: any) => {
      //     if (response.status === 200 || response.status === 206) {
      //       for (let i = 0; i < response.body.length; i++) {
      //         // add images to ticketImages for carousel
      //         const src = response.body[i].fullUrl;
      //         const caption = response.body[i].fileName;
      //         if (!src || !caption) {
      //           continue;
      //         }
      //         const album = {
      //           src: src,
      //           caption: caption,
      //           thumb: ''
      //         };
      //         this.addAttachment(response.body[i].fullUrl, response.body[i].fileName, response.body[i].imageTakenDate, response.body[i].blobName);
      //       }
      //     }
      //     if (response.status === 206) {
      //       this._snackBar.open('Oops, there was an error, some images were not uploaded, please try again later', 'close', { duration: 1000 });
      //     }
      //   },
      //     (error) => { this._snackBar.open('Oops, there was an error', 'close', { duration: 1000 }); });
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
    this.attachments.push(attachment);
  }

  // Reply
  replyResponse($event): void {
    if (!$event) {
      this.showTicketReply = false;
      return;
    }

    const reply = new FdTicketReplyModel($event.reply);

    if (this.editorMode === EditorMode.FreshdeskReply) {

      this.loading$.next(true);
      reply.ticket_id = this.freshdeskTicket.id;
      reply.body;// += '<br/>' + this.threadEmail;
      this._ticketService.sendTicketReply(reply, $event.files, this.ticketId)
        .subscribe(() => {
          this.showTicketReply = false;
          this.loading$.next(false);
          this._snackBar.open('Reply successfully', 'close', { duration: 2000 });
          this.getTicketDetails(this.ticketId);
        }, (error) => {
          console.log(error);
          this.loading$.next(false);
          this._snackBar.open('Oops! there was an error', 'close', { duration: 2000 });
        });

    } else if (this.editorMode === EditorMode.ShareTicket) {

      this.loading$.next(true);
      this._ticketService.sendTicketForward(reply, $event.to, this.ticket.id.toString(), $event.files)
        .subscribe(() => {
          this.showTicketReply = false;
          this.loading$.next(false);
          this._snackBar.open('Shared successfully', 'close', { duration: 3000 });
          this.getTicketDetails(this.ticketId);
        }, (error) => {
          console.log(error);
          this.loading$.next(false);
          this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
        });

    } else if (this.editorMode === EditorMode.ForwardTicket) {

      this.loading$.next(true);
      reply.body += '<br/>' + this.threadEmail;
      this._ticketService.sendTicketForward(reply, $event.to, this.ticket.id.toString(), $event.files)
        .subscribe(() => {
          this.showTicketReply = false;
          this.loading$.next(false);
          this._snackBar.open('Forward successfully', 'close', { duration: 3000 });
          this.getTicketDetails(this.ticketId);
        }, (error) => {
          console.log(error);
          this.loading$.next(false);
          this._snackBar.open('Oops! there was an error', 'close', { duration: 3000 });
        });

    }
  }

  createBodyHTML(bodyBase: string): string {
    const divBase = document.createElement('div');
    divBase.innerHTML = bodyBase;
    const sections = divBase.getElementsByClassName('quoted-text');
    const sections2 = divBase.getElementsByClassName('freshdesk-quoted');
    const sections3 = divBase.getElementsByClassName('freshdesk_quote');
    const sections4 = divBase.getElementsByClassName('gmail_extra');
    for (let index = 0; index < sections.length; index++) {
      divBase.getElementsByClassName('quoted-text')[index].innerHTML = '';
    }
    for (let index = 0; index < sections2.length; index++) {
      divBase.getElementsByClassName('freshdesk-quoted')[index].innerHTML = '';
    }
    for (let index = 0; index < sections3.length; index++) {
      divBase.getElementsByClassName('freshdesk_quote')[index].innerHTML = '';
    }
    for (let index = 0; index < sections4.length; index++) {
      divBase.getElementsByClassName('gmail_extra')[index].innerHTML = '';
    }
    bodyBase = divBase.innerHTML;
    return bodyBase;
  }

  createBodyBlockquote(bodyBase: string): string {
    const divBase = document.createElement('div');
    var blockquoteBase = "";
    divBase.innerHTML = bodyBase;
    const sections = divBase.getElementsByClassName('quoted-text');
    const sections2 = divBase.getElementsByClassName('freshdesk-quoted');
    const sections3 = divBase.getElementsByClassName('freshdesk_quote');
    const sections4 = divBase.getElementsByClassName('gmail_extra');
    for (let index = 0; index < divBase.children.length; index++) {

    }
    for (let index = 0; index < sections.length; index++) {
      blockquoteBase = divBase.getElementsByClassName('quoted-text')[index].innerHTML;
    }
    for (let index = 0; index < sections2.length; index++) {
      blockquoteBase = divBase.getElementsByClassName('freshdesk-quoted')[index].innerHTML;
    }
    for (let index = 0; index < sections3.length; index++) {
      blockquoteBase = divBase.getElementsByClassName('freshdesk_quote')[index].innerHTML;
    }
    for (let index = 0; index < sections4.length; index++) {
      blockquoteBase = divBase.getElementsByClassName('gmail_extra')[index].innerHTML;
    }
    bodyBase = blockquoteBase;
    return bodyBase;
  }

  createFullThread(): void {
    const emailConversations = this.conversations.filter(c => !c.fromActivityLog);
    let thread = '<br/>';

    if (this.freshdeskTicket) {
      thread += `${this.freshdeskTicket.description_text}`;
    }

    emailConversations.forEach(c => {

      // remove email thread section
      const div = document.createElement('div');
      div.innerHTML = c.body;
      const sections = div.getElementsByClassName('quoted-text');
      for (let index = 0; index < sections.length; index++) {
        div.getElementsByClassName('quoted-text')[index].innerHTML = '';
      }

      // generate template
      // const title = `On ${this._datePipe.transform(c.created_at, 'MMM d, y, h:mm:ss a')}, &#60;${c.support_email}&#62; wrote:`;
      // const body = div.textContent || div.innerText || '';
      // thread += `<blockquote cite=\"${c.support_email}\"><small>${title}</small><br/><p>${body}</p></blockquote>`;
    });


    this.threadEmail = thread;
  }
  // Utils
  public fileType(name): string {
    const extension = this.extensionRegex.exec(name)[1];
    const type = this.fileExtensions.find(e => e.ext === extension);
    return type ? type.type.split('/')[0] : 'none';
  }

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

  // Forms
  createTicketUpdateForm(): FormGroup {
    return this._formBuilder.group({
      id: 0,
      buildingId: 0,
      buildingCtrl: [''],
      assignedEmployeeId: 0,
      employeeCtrl: [''],
      fullAddress: [''],
      requesterFullName: ['', [Validators.required, Validators.maxLength(128)]],
      requesterEmail: ['', [Validators.required, Validators.maxLength(128)]],
      requesterPhone: ['', [Validators.maxLength(13)]],
      description: ['', [Validators.required]],
      snoozeDate: [''],
      attachments: this._formBuilder.array([]),
      source: [''],
      status: [''],
      location: ['']
    });
  }

  setValuesToTicketForm(): void {
    this.ticketForm = this._formBuilder.group({
      id: this.ticket.id,
      buildingId: [this.ticket.buildingId],
      buildingCtrl: [''],
      assignedEmployeeId: [this.ticket.assignedEmployeeId],
      employeeCtrl: [''],
      fullAddress: [{ value: this.ticket.fullAddress, disabled: false }],
      requesterFullName: [{ value: this.ticket.requesterFullName, disabled: false }, [Validators.required, Validators.maxLength(128)]],
      requesterEmail: [{ value: this.ticket.requesterEmail, disabled: false }, [Validators.required, Validators.maxLength(128)]],
      requesterPhone: [{ value: this.ticket.requesterPhone, disabled: false }, [Validators.maxLength(13)]],
      description: [{ value: this.ticket.description, disabled: false }, [Validators.required]],
      snoozeDate: [{ value: this.ticket.snoozeDate, disabled: false }],
      attachments: this._formBuilder.array([]),
      source: [{ value: this.ticket.source, disabled: false }],
      status: [{ value: this.ticket.status, disabled: false }],
      location: [{ value: this.ticket.location, disabled: false }]
    });
  }

  // Attachments
  checkForAttachemnts(): void {
    if (this.ticket.attachments.length > 0) {
      const attachmentFormGroups = this.ticket.attachments.map(attachemnt => this._formBuilder.group(attachemnt));
      const attachmentFormArray = this._formBuilder.array(attachmentFormGroups);
      this.ticketForm.setControl('attachments', attachmentFormArray);
    }
  }

  // Building
  getBuildings(): void {
    this._buildingService.getAllAsList('readallcbo', '', 0, 200, this.buildingId.value, {})
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings = response.payload;
      },
        (error) => this._snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.buildingSearch.value;
    if (!search) {
      this.filteredBuildings = this.buildings.slice();
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.filteredBuildings =
      this.buildings.filter(building => (building.name.toLowerCase()).indexOf(search) > -1)
      ;
  }

  // Employee
  getEmployees(): void {
    this._userBaseService.getAllAsList('readallcbo', '', 0, 99999)
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
        this.employees = response.payload;
        this.filteredEmployees = response.payload;
      }, (error) => {
        this._snackBar.open('Oops, there was an error fetching employees', 'close', { duration: 1000 });
      });
  }

  private filterEmployees(): void {
    if (!this.employees) {
      return;
    }
    // get the search keyword
    let search = this.employeeSearch.value;
    if (!search) {
      this.filteredEmployees = this.employees.slice();
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.filteredEmployees =
      this.employees.filter(e => (e.name.toLowerCase()).indexOf(search) > -1)
      ;
  }

  // Tags
  getTags(): void {
    this._tagService.getTagsAsList()
      .subscribe((result: ListItemModel[]) => {
        result.forEach(element => {
          this.tags.push({
            id: element.id,
            name: element.name,
            assigned: false
          });

          this.filteredTags.push({
            id: element.id,
            name: element.name,
            assigned: false
          });
        });

        this.refreshTicketTagsAssignment();
      }, (error) => {

      });
  }

  getTicketTags(): void {
    this._tagService.readAllTicketTags(this.ticketId)
      .subscribe((result) => {
        this.assignedTags = result;
        this.refreshTicketTagsAssignment();
      }, (error) => {
        console.log(JSON.stringify(error));
      });
  }

  assignTag(tagId: number): void {
    event.stopPropagation();
    const assignmentIndex = this.assignedTags.findIndex(t => t.tagId === tagId);
    if (assignmentIndex >= 0) {
      this._tagService.removeTicketTag(this.assignedTags[assignmentIndex].ticketTagId)
        .subscribe(() => {
          this.getTicketTags();
          this._snackBar.open('Tag unassigned successfully ', 'close', { duration: 3000 });
        }, () => {
          this._snackBar.open('Oops, there was an error', 'close', { duration: 3000 });
        });
    } else {
      this._tagService.create({ ticketId: this.ticketId, tagId: tagId }, 'AddTicketTag')
        .subscribe(() => {
          this.getTicketTags();
          this._snackBar.open('Tag assigned successfully ', 'close', { duration: 3000 });
        }, () => {
          this._snackBar.open('Oops, Cannot be Assigned', 'close', { duration: 3000 });
        });
    }
  }

  unassignTag(ticketTagId: number): void {
    this._tagService.removeTicketTag(ticketTagId)
      .subscribe(() => {
        this.getTicketTags();
        this._snackBar.open('Tag unassigned successfully ', 'close', { duration: 3000 });
      }, () => {
        this._snackBar.open('Oops, there was an error', 'close', { duration: 3000 });
      });
  }

  refreshTicketTagsAssignment(): void {
    if (this.assignedTags.length > 0) {

      this.filteredTags.forEach(t => t.assigned = false);
      this.tags.forEach(t => t.assigned = false);

      this.assignedTags.forEach(a => {
        const index = this.filteredTags.findIndex(t => t.id === a.tagId);
        if (index >= 0) {
          this.filteredTags[index].assigned = true;
        }
      });
    }
  }

  // Lightbox
  checkForAttachmentsToAlbum(): void {
    const attachments = this.ticket.attachments;
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

    const index = this.ticket.attachments.findIndex(el => el.id === attatchmentId);
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

  // Work Order Sequence
  displayWorkOrderSequence(id: number): void {
    this.workOrderSequencesDialog = this._dialog.open(WorkOrderSequencesDialogComponent, {
      panelClass: 'work-order-sequences-dialog',
      data: {
        calendarItemFrequencyId: id
      }
    });
  }

  // Show ticket activity log dialog
  showActivity(ticket: any): void {
    this.dialogRef = this._dialog.open(TicketActivityDialogComponent, {
      panelClass: 'ticket-activity-dialog',
      data: {
        id: ticket.id,
        number: ticket.number
      }
    });
  }

}
