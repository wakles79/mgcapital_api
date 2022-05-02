import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { CleaningReportDetailsModel } from '@app/core/models/reports/cleaning-report/cleaning.report.details.model';
import { CleaningReportItemAttachmentModel } from '@app/core/models/reports/cleaning-report/cleaning.report.item.attachment.model';
import { CleaningReportNoteCreateModel } from '@app/core/models/reports/cleaning-report/cleaning.report.note.create.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { CleaningReportDetailsService } from '../../private/reports/cleaning-report/cleaning-report-details/cleaning-report-details.service';

@Component({
  selector: 'app-public-cleaning-report',
  templateUrl: './public-cleaning-report.component.html',
  styleUrls: ['./public-cleaning-report.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PublicCleaningReportComponent implements OnInit {

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
  cleaningItemsDisplayedColumns = ['building', 'location', 'time', 'observances'];
  get cleaningItemsCount(): number { return this.cleaningReportDetailsService.cleaningItems.length; }

  findingItemsDataSource: FindingItemsDataSource | any;
  findingItemsDisplayedColumns = ['building', 'location', 'observances'];
  get findingItemsCount(): number { return this.cleaningReportDetailsService.findingItems.length; }

  notesDataSource: NotesDataSource | any;
  notesDisplayedColums = ['message'];
  noteForm: FormGroup;

  dialogRef: any;

  constructor(
    private fuseConfig: FuseConfigService,
    private cleaningReportDetailsService: CleaningReportDetailsService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private fromBuilder: FormBuilder
  ) {

    this.fuseConfig.config = {
      layout: {
        navbar: {
          hidden: true
        },
        toolbar: {
          hidden: true
        },
        footer: {
          hidden: true
        },
        sidepanel: {
          hidden: true
        }
      }
    };

    this.reportDataChangedSubscription =
      this.cleaningReportDetailsService.onCleaningReportDetailsChange.subscribe((reportData: any) => {
        this.loading$.next(false);
        this.reportDetails = reportData;
        this.checkForAttachments();
        console.log('Attachments');
        console.log(this.attachments);
      });

    this.noteForm = this.createNoteCreateForm();
  }

  ngOnInit(): void {
    this.cleaningItemsDataSource = new CleaningItemsDataSource(this.cleaningReportDetailsService);
    this.findingItemsDataSource = new FindingItemsDataSource(this.cleaningReportDetailsService);
    this.notesDataSource = new NotesDataSource(this.cleaningReportDetailsService);
  }

  /*Attachments */
  checkForAttachments(): void {
    const attachments = this.attachments;
    if (attachments.length > 0) {

      // add images to woImages for carousel
      attachments.forEach(attachment => {
        const src = attachment.fullUrl;
        const caption = attachment.title;

        if (src && caption) {
          const album = {
            src: src,
            caption: caption,
            thumb: ''
          };
          this.images.push(album);
        }

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

  createNoteCreateForm(): FormGroup {
    return this.fromBuilder.group({
      cleaningReportId: [this.reportDetails.id],
      direction: [-1],
      message: ['', Validators.required]
    });
  }

  addNote(): void {
    this.loading$.next(true);

    const note = new CleaningReportNoteCreateModel(this.noteForm.getRawValue());
    const url = 'AddCleaningReportPublicNote/' + this.reportDetails.guid;

    this.cleaningReportDetailsService.createCleaningReportPublicNote(note, url)
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

  disconnect(): void {
  }
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

  disconnect(): void {
  }
}

export class NotesDataSource extends DataSource<any> {
  constructor(private service: CleaningReportDetailsService) {
    super();
  }

  connect(): Observable<any> {
    return this.service.onNotesChange;
  }

  disconnect(): void {

  }

}
