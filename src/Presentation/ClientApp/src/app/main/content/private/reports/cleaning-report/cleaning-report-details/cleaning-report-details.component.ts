import { Component, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { CleaningReportDetailsModel } from '@app/core/models/reports/cleaning-report/cleaning.report.details.model';
import { CleaningReportItemAttachmentModel } from '@app/core/models/reports/cleaning-report/cleaning.report.item.attachment.model';
import { CLEANING_REPORT_ITEM_TYPES } from '@app/core/models/reports/cleaning-report/item-type.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { CleaningReportDetailsService } from './cleaning-report-details.service';
import { Location } from '@angular/common';
import { CleaningReportNoteCreateModel } from '@app/core/models/reports/cleaning-report/cleaning.report.note.create.model';
import { CleaningReportItemFormComponent } from '../cleaning-report-item-form/cleaning-report-item-form.component';
import { CleaningReportItemBaseModel } from '@app/core/models/reports/cleaning-report/cleaning.report.item.model';
import { CrSendEmailConfirmDialogComponent } from '../cr-send-email-confirm-dialog/cr-send-email-confirm-dialog.component';
import { CleaningReportSendEmailModel } from '@app/core/models/reports/cleaning-report/cleaning.report-send-email.model';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { DataSource } from '@angular/cdk/table';

@Component({
  selector: 'app-cleaning-report-details',
  templateUrl: './cleaning-report-details.component.html',
  styleUrls: ['./cleaning-report-details.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class CleaningReportDetailsComponent implements OnInit {

  @ViewChild('dialogContent') dialogContent: TemplateRef<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  // attachments
  images: Array<IAlbum> = [];
  private _subscription: Subscription;

  get attachments(): CleaningReportItemAttachmentModel[] {
    let attachments: CleaningReportItemAttachmentModel[] = [];

    if (this.reportDetails) {
      // Gets report item attachments
      if (this.reportDetails.cleaningItems) {
        this.reportDetails.cleaningItems.forEach(cI => {
          attachments = attachments.concat(cI.attachments);
        });
      }

      // Gets finding item attachments
      if (this.reportDetails.findingItems) {
        this.reportDetails.findingItems.forEach(fI => {
          attachments = attachments.concat(fI.attachments);
        });
      }
    }

    return attachments;
  }

  reportDataChangedSubscription: Subscription;
  reportDetails: CleaningReportDetailsModel;
  onCleaningItemsChanged: BehaviorSubject<any> = new BehaviorSubject<any>([]);
  loading$ = new BehaviorSubject<boolean>(true);

  cleaningItemsDataSource: CleaningItemsDataSource | any;
  cleaningItemsDisplayedColumns = ['building', 'location', 'time', 'observances', 'buttons'];
  get cleaningItemsCount(): number { return this.cleaningReportDetailsService.cleaningItems.length; }

  findingItemsDataSource: FindingItemsDataSource | any;
  findingItemsDisplayedColumns = ['building', 'location', 'observances', 'buttons'];
  get findingItemsCount(): number { return this.cleaningReportDetailsService.findingItems.length; }

  notesDataSource: NotesDataSource | any;
  notesDisplayedColums = ['message'];
  noteForm: FormGroup;

  activityLog: any;

  dialogRef: any;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  itemTypes: any[] = CLEANING_REPORT_ITEM_TYPES;

  fileUrl: any;
  downloadAvailable = false;
  documentName = 'cleaning-report.pdf';

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private location: Location,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private cleaningReportDetailsService: CleaningReportDetailsService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig,
    private router: Router,
    private route: ActivatedRoute,
    private fromBuilder: FormBuilder,
    private sanitizer: DomSanitizer
  ) {
    this.loading$.next(true);
    this.reportDataChangedSubscription =
      this.cleaningReportDetailsService.onCleaningReportDetailsChange.subscribe((reportData: any) => {
        this.loading$.next(false);
        this.reportDetails = reportData;
        this.documentName = 'cleaning-report-' + this.reportDetails.guid + '.pdf';
        this.checkForAttachments();
      });

    this.noteForm = this.createNoteCreateForm();
  }

  ngOnInit(): void {
    this.cleaningItemsDataSource = new CleaningItemsDataSource(this.cleaningReportDetailsService);
    this.findingItemsDataSource = new FindingItemsDataSource(this.cleaningReportDetailsService);
    this.notesDataSource = new NotesDataSource(this.cleaningReportDetailsService);

    this.cleaningReportDetailsService.getCleaningReportLogActivity(this.reportDetails.id).subscribe((response: { count: number, payload: any }) => {
      this.activityLog = response.payload;
    });

    this.downloadPdf();

    // See https://stackoverflow.com/questions/47455734/how-get-query-parameters-from-url-in-angular-5/51808539#51808539
    this.route.queryParamMap.subscribe((map: any) => {
      const action = map.params['action'];
      if (action === 'add') {
        // IN THE FUTURE add an item :)
      }
      else if (action === 'edit') {
        const cleaningReportItemId = map.params['itemId'];
        if (cleaningReportItemId) {
          this.editElementById(cleaningReportItemId);
        }
      }
    });
  }

  goBack(): void {
    this.location.back();
  }

  get isEditable(): boolean {
    return this.reportDetails.submitted === 0;
  }

  addElementForBuilding(cleaningReportItem): void {
    this.cleaningReportDetailsService.get(cleaningReportItem.id, 'UpdateCleaningReportItem')
      .subscribe((itemData: any) => {
        if (itemData) {

          const buildingId = itemData.buildingId;
          const itemType = this.itemTypes.find(item => item.id === itemData.type);

          this.addElement(itemType.value, buildingId);

        }
      },
        (error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
  }

  createNoteCreateForm(): FormGroup {
    return this.fromBuilder.group({
      cleaningReportId: [this.reportDetails.id],
      direction: [1],
      message: ['', Validators.required]
    });
  }

  addNote(): void {
    this.loading$.next(true);

    const note = new CleaningReportNoteCreateModel(this.noteForm.getRawValue());

    this.cleaningReportDetailsService.createCleaningReportNote(note, 'AddCleaningReportNote')
      .then(
        () => {
          this.loading$.next(false);
          this.snackBar.open('Cleaning Report Note was created successfully!!!', 'close', { duration: 2000 });
          this.noteForm.get('message').setValue('');
        },
        () => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      )
      .catch((error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  addElement(itemTypeValue: any, forBuildingId: any = null): void {
    const itemType = this.itemTypes.find(item => item.value === itemTypeValue);
    this.dialogRef = this.dialog.open(CleaningReportItemFormComponent, {
      panelClass: 'cleaning-item-form-dialog',
      data: {
        action: 'add',
        itemType: itemType,
        customerContactId: this.reportDetails.contactId,
        cleaningReportId: this.reportDetails.id,
        forBuildingId: forBuildingId
      }
    });

    this.dialogRef.afterClosed().subscribe((response: FormGroup) => {
      const action = response[0];
      const formData = response[1];

      if (!response || action === 'close') {
        return;
      }

      this.loading$.next(true);

      const cleaningReportItemToCreate = new CleaningReportItemBaseModel(formData.getRawValue());

      this.cleaningReportDetailsService.createCleaningReportItem(cleaningReportItemToCreate, 'AddCleaningReportItem')
        .then(
          () => {
            this.loading$.next(false);
            this.snackBar.open('Cleaning Report Item was created successfully!!!', 'close', { duration: 2000 });
            this.cleaningReportDetailsService.getCleaningReportLogActivity(this.reportDetails.id).subscribe((responses: { count: number, payload: any }) => {
              this.activityLog = responses.payload;
            });
          },
          () => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          })
        .catch((error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
    });
  }

  viewElement(cleaningReportItem): void {
    this.cleaningReportDetailsService.get(cleaningReportItem.id, 'UpdateCleaningReportItem')
      .subscribe((itemData: any) => {
        if (itemData) {

          const cleaningReportItemToUpdate = new CleaningReportItemBaseModel(itemData);
          const itemType = this.itemTypes.find(item => item.id === cleaningReportItemToUpdate.type);

          this.dialogRef = this.dialog.open(CleaningReportItemFormComponent, {
            panelClass: 'cleaning-item-form-dialog',
            data: {
              action: 'viewDetails',
              itemType: itemType,
              customerContactId: this.reportDetails.contactId,
              cleaningReportItem: cleaningReportItemToUpdate
            }
          });

          this.dialogRef.afterClosed().subscribe((response: FormGroup) => {
            return;
          });
        }
      });
  }

  editElement(cleaningReportItem): void {
    this.editElementById(cleaningReportItem.id);
  }


  editElementById(cleaningReportItemId): void {
    this.cleaningReportDetailsService.get(cleaningReportItemId, 'UpdateCleaningReportItem')
      .subscribe((itemData: any) => {
        if (itemData) {

          const cleaningReportItemToUpdate = new CleaningReportItemBaseModel(itemData);
          const itemType = this.itemTypes.find(item => item.id === cleaningReportItemToUpdate.type);

          this.dialogRef = this.dialog.open(CleaningReportItemFormComponent, {
            panelClass: 'cleaning-item-form-dialog',
            data: {
              action: 'edit',
              itemType: itemType,
              customerContactId: this.reportDetails.contactId,
              cleaningReportItem: cleaningReportItemToUpdate
            }
          });

          this.dialogRef.afterClosed().subscribe((response: FormGroup) => {

            const action = response[0];
            const formData = response[1];

            if (!response || action === 'close') {
              return;
            }
            // console.log('reponse', response);
            this.loading$.next(true);
            const cleaningReportItemUpdated = new CleaningReportItemBaseModel(formData.getRawValue());

            switch (action) {
              /**
               * Save
               */
              case 'save':

                this.cleaningReportDetailsService.updateCleaningReportItem(cleaningReportItemUpdated, 'UpdateCleaningReportItem')
                  .then(
                    () => {
                      this.loading$.next(false);
                      this.snackBar.open('Cleaning Report Item was updated successfully!!!', 'close', { duration: 2000 });
                    },
                    () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
                  .catch((error) => {
                    this.loading$.next(false);
                    this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                  });

                break;

              /**
               * Delete
               */
              case 'delete':
                this.removeElement(cleaningReportItemUpdated);
                break;
            }
          });
        }
      },
        (error) => {
          this.loading$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        });
  }

  removeElement(cleaningReportItem): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.cleaningReportDetailsService.deleteCleaningReportItem(cleaningReportItem, 'DeleteCleaningReportItem')
          .then(
            () => {
              this.loading$.next(false);
              this.snackBar.open('Cleaning Report Item was deleted successfully!!!', 'close', { duration: 2000 });
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch((error) => {
            this.loading$.next(false);
            this.snackBar.open(error, 'close', { duration: 1000 });
          });
      }
      this.confirmDialogRef = null;
    });
  }

  get urlToCopy(): string {
    return window.location.protocol + '//' + window.location.host + '/reports/cleaning-report/' +
      this.reportDetails.guid;
  }

  sendByEmailCleaningReportLink(): void {

    const recipients: { id: number, fullName: string, email: string }[] = [];
    if (this.reportDetails.toEmail.length > 0) {
      recipients.push({ id: 0, fullName: this.reportDetails.to, email: this.reportDetails.toEmail });
    }

    this.confirmDialogRef = this.dialog.open(CrSendEmailConfirmDialogComponent, {
      disableClose: false,
      data: {
        cleaningReportId: this.reportDetails.id,
        recipients: recipients,
        confirmMessage: 'Are you sure you want to send this document public view link by email? Feel free to use the fields below to add additional recipients if necessary'
      }
    });

    this.confirmDialogRef.afterClosed().subscribe((formData: any) => {
      if (!formData) {
        return;
      }

      this.loading$.next(true);
      const cleaningReportToSend = new CleaningReportSendEmailModel(formData.getRawValue());
      this.cleaningReportDetailsService.sendByEmailCleaningReportLink(cleaningReportToSend)
        .then(
          () => {
            this.loading$.next(false);
            this.snackBar.open('Cleaning Report was send by email successfully!!!', 'close', { duration: 2000 });
            this.cleaningReportDetailsService.getCleaningReportLogActivity(this.reportDetails.id).subscribe((response: { count: number, payload: any }) => {
              this.activityLog = response.payload;
            });
          },
          (error) => {
            this.loading$.next(false);
            this.snackBar.open(error, 'close', { duration: 1000 });
          })
        .catch((error) => {
          this.loading$.next(false);
          this.snackBar.open(error, 'close', { duration: 1000 });
        });

      this.confirmDialogRef = null;
    });
  }

  shareDocument(): void {
    this.dialogRef = this.dialog.open(ShareUrlDialogComponent, {
      panelClass: 'share-url-form-dialog',
      data: {
        urlToCopy: this.urlToCopy
      }
    });
  }

  openPublicDocumentViewNewTap(): void {
    window.open(this.urlToCopy, '_blank');
  }

  downloadPdf(): void {
    // this.loading$.next(true);
    this.cleaningReportDetailsService.get(this.reportDetails.id, 'GetPDFReportBase64').subscribe(
      (response: string) => {
        // this.loading$.next(false);
        const bytes = atob(response);

        const byteNumbers = new Array(bytes.length);
        for (let i = 0; i < bytes.length; i++) {
          byteNumbers[i] = bytes.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);

        const blob = new Blob([byteArray], { type: 'application/pdf' });

        const blobUrl = URL.createObjectURL(blob);

        this.fileUrl = this.sanitizer.bypassSecurityTrustResourceUrl(window.URL.createObjectURL(blob));
        this.downloadAvailable = true;
      }, (error) => {
        // this.loading$.next(false);
        this.snackBar.open('Oops, there was an error to download document', 'close', { duration: 1000 });
        this.downloadAvailable = false;
      }
    );
  }

  /*Attachments */
  checkForAttachments(): void {
    const attachments = this.attachments;
    if (attachments.length > 0) {
      // Clean images
      this.images = [];

      // add images to woImages for carousel
      attachments.forEach(attachment => {
        const src = attachment.fullUrl;
        const caption = attachment.title;
        const album = {
          src: src,
          caption: caption,
          thumb: ''
        };
        this.images.push(album);
      });
      this._lighboxConfig.fadeDuration = 1;
    }
  }

  open(attatchmentId: number): void {
    this._subscription = this._lightboxEvent.lightboxEvent$.subscribe((event: IEvent) => this._onReceivedEvent(event));
    const index = this.attachments.findIndex(el => el.id === attatchmentId);

    // override the default config
    this._lightbox.open(this.images, index, { wrapAround: true, showImageNumberLabel: true });
  }

  private _onReceivedEvent(event: IEvent): void {
    if (event.id === LIGHTBOX_EVENT.CLOSE) {
      this._subscription.unsubscribe();
    }
  }

  /* End Attachments */

  messageAlign(direction: number): string {
    if (direction < 0) {
      return 'incoming';
    }

    return 'outgoing';
  }

  messageBg(direction: number): string {
    if (direction < 0) {
      return 'comment-bubble-incoming';
    }

    return 'comment-bubble-outgoing';
  }

  /* Log */
  activityType(activity: any): any {
    if (activity === 0) {
      return 'Field Updates';
    } else if (activity === 1) {
      return 'Item Updates';
    } else if (activity === 2) {
      return 'Sent an email';
    }
  }

  cleaningReportItemType(itemType: any): any {
    if (itemType === 0) {
      return 'Report Item';
    } else if (itemType === 1) {
      return 'Findings';
    }
  }
}

export class CleaningItemsDataSource extends DataSource<any>
{
  constructor(private service: CleaningReportDetailsService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.service.onCleaningReportItemsChange;
  }

  disconnect(): void { }
}


export class FindingItemsDataSource extends DataSource<any>
{
  constructor(private service: CleaningReportDetailsService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.service.onCleaningReportFindingsChange;
  }

  disconnect(): void { }
}

export class NotesDataSource extends DataSource<any> {
  constructor(private service: CleaningReportDetailsService) {
    super();
  }

  connect(): Observable<any> {
    return this.service.onNotesChange;
  }

  disconnect(): void { }
}
