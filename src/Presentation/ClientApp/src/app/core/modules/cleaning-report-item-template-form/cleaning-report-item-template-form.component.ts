import { Component, OnInit, ViewEncapsulation, OnDestroy, Input, ViewChild } from '@angular/core';
import { FormGroup, AbstractControl, FormBuilder, Validators, FormArray } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSelect } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListBuildingModel } from '@app/core/models/building/list-buildings.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { BuildingsService } from '@app/main/content/private/buildings/buildings.service';
import { CleaningReportDetailsService } from '@app/main/content/private/reports/cleaning-report/cleaning-report-details/cleaning-report-details.service';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { ReplaySubject, Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';


@Component({
  selector: 'app-cleaning-report-item-template-form',
  templateUrl: './cleaning-report-item-template-form.component.html',
  styleUrls: ['./cleaning-report-item-template-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CleaningReportItemTemplateFormComponent implements OnInit, OnDestroy {

  dialogTitle: string;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;
  cleaningReportItemForm: FormGroup;
  action: string;
  cleaningReportItem: any;
  // Indicate the type of the cleaning report item, could be 'cleaningItem' or 'findingItem'
  itemType: any;
  cleaningReportId = 0;

  @Input() data: any;

  // customerId (Management Co.) to filter the buildings to display
  customerContactId: any;

  buildingsListSubscription: Subscription;
  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings$: ReplaySubject<ListItemModel[]> = new ReplaySubject<ListItemModel[]>(1);
  get buildingCtrl(): AbstractControl { return this.cleaningReportItemForm.get('buildingCtrl'); }
  get selectedBuildingId(): AbstractControl { return this.cleaningReportItemForm.get('buildingId'); }
  @ViewChild('buildingSelect') buildingSelect: MatSelect;

  // Show or hide attachments section
  showAttachments: boolean;
  woImages: Array<IAlbum> = [];

  private _subscription: Subscription;

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  onCleaningReportItemTemplateSubmitted: Subject<any> = new Subject<any>();
  // wildcard to disable the form from a component parent
  buttonSaveDisabled = false;

  disableButtons = false;

  showCloseButton: boolean = true;

  constructor(
    // @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private cleaningReportService: CleaningReportDetailsService,
    private buildingService: BuildingsService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig
  ) {
  }

  ngOnInit(): void {
    this.action = this.data.action;
    this.itemType = this.data.itemType;
    this.customerContactId = this.data.customerContactId;
    this.cleaningReportItem = {};

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Cleaning Report Item';
      this.initData();
    }
    else if (this.action === 'newFromTicket') {
      this.dialogTitle = 'New Cleaning Report Item From Ticket';
      this.initData();
      this.cleaningReportItemForm.removeControl('id');
    }
    else if (this.action === 'viewDetails') {
      this.dialogTitle = 'Cleaning Report Item Details';
      this.initData();
      this.cleaningReportItemForm.disable();
      this.disableButtons = true;
    }
    else {
      this.dialogTitle = 'New Cleaning Report Item';
      this.cleaningReportId = this.data.cleaningReportId;
      this.cleaningReportItemForm = this.createCleaningReportItemCreateForm();
    }

    if (this.data.forBuildingId) {
      this.selectedBuildingId.setValue(this.data.forBuildingId);
      this.selectedBuildingId.disable();
    }

    this.getBuildings();

    this.data.hasOwnProperty('showCloseButton') ? this.showCloseButton = this.data.showCloseButton : this.showCloseButton = true;

    this.buildingCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterBuildings();
      });
  }

  initData(): void {
    this.cleaningReportItem = this.data.cleaningReportItem;
    this.cleaningReportId = this.cleaningReportItem.cleaningReportId;
    this.cleaningReportItemForm = this.createCleaningReportItemUpdateForm();

    // Check if there are attachments to display
    this.checkForAttachemnts();
  }

  private filterElements(
    els: any[],
    ctrl: AbstractControl,
    subj$: ReplaySubject<any[]>,
    itemsSelect: MatSelect,
    selectedElement: number,
    filterFunc): void {
    // get the search keyword
    const search = (ctrl.value || '').toLowerCase();
    if (search === '' && selectedElement) {
      return;
    }
    // make another request
    filterFunc(search);
  }

  // Read this https://github.com/Microsoft/TypeScript/wiki/FAQ#why-does-this-get-orphaned-in-my-instance-methods
  getBuildings = (filter = '') => {

    if (this.buildingsListSubscription && !this.buildingsListSubscription.closed) {
      this.buildingsListSubscription.unsubscribe();
    }

    this.buildingService.getAllAsList('readAllCboByContact', '', 0, 200, this.selectedBuildingId.value, { 'contactId': this.customerContactId })
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings$.next(response.payload);
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  private filterBuildings(): void {
    this.filterElements(this.buildings, this.buildingCtrl, this.filteredBuildings$, this.buildingSelect, this.selectedBuildingId.value, this.getBuildings);
  }

  createCleaningReportItemCreateForm(): FormGroup {
    return this.formBuilder.group({
      cleaningReportId: [this.cleaningReportId],
      type: [this.itemType.id],
      observances: ['', Validators.required],
      time: [''],
      location: ['', Validators.required],
      buildingId: ['', Validators.required],
      buildingCtrl: [''],
      attachments: this.formBuilder.array([]),
    });
  }

  createCleaningReportItemUpdateForm(): FormGroup {
    return this.formBuilder.group({
      id: [this.cleaningReportItem.id],
      type: [this.cleaningReportItem.type],
      observances: [this.cleaningReportItem.observances],
      time: [this.cleaningReportItem.time],
      location: [this.cleaningReportItem.location],
      buildingId: [this.cleaningReportItem.buildingId],
      buildingCtrl: [''],
      cleaningReportId: [this.cleaningReportItem.cleaningReportId],
      attachments: this.formBuilder.array([]),
    });
  }

  /*
  * Attachments section
  */
  get attachments(): FormArray {
    return this.cleaningReportItemForm.get('attachments') as FormArray;
  }

  checkForAttachemnts(): void {
    if (this.cleaningReportItem.attachments.length > 0) {
      this.showAttachments = true;
      const attachmentFormGroups = this.cleaningReportItem.attachments.map(attachemnt => this.formBuilder.group(attachemnt));
      const attachmentFormArray = this.formBuilder.array(attachmentFormGroups);
      this.cleaningReportItemForm.setControl('attachments', attachmentFormArray);

      // add images to woImages for carousel
      this.cleaningReportItem.attachments.forEach(attachment => {
        const src = attachment.fullUrl;
        const caption = attachment.title;

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
      this.cleaningReportService.uploadFile(files)
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
    const attachment = this.formBuilder.group({
      id: [-1],
      fullUrl: [fullUrl],
      title: [description],
      imageTakenDate: [imageTakenDate],
      blobName: [blobName]
    });
    this.attachments.push(attachment);
  }

  removeAttachment(index): void {
    const attachmentToDelete = this.attachments.at(index);
    (this.cleaningReportItemForm.get('attachments') as FormArray).removeAt(index);
    this.woImages.splice(index, 1);
    // If id of deleted attachment was -1, it's necessary deleted it from azure
    if (attachmentToDelete.get('id').value === -1) {
      this.cleaningReportService.deleteAttachmentByBlobName(attachmentToDelete.get('blobName').value).subscribe();
    }
  }

  /*
  * End Attachments section
  */

  submit(action: string): void {
    this.onCleaningReportItemTemplateSubmitted.next([action, this.cleaningReportItemForm]);
  }

  closeDialog(): void {
    this.onCleaningReportItemTemplateSubmitted.next(['close', this.cleaningReportItemForm]);
    // Delete images uploaded in azure but not saved in data base
    for (let i = 0; i < this.attachments.value.length; i++) {
      if (this.attachments.at(i).get('id').value === -1) {
        this.cleaningReportService.deleteAttachmentByBlobName(this.attachments.at(i).get('blobName').value).subscribe();
      }
    }
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();

    if (this.buildingsListSubscription && !this.buildingsListSubscription.closed) {
      this.buildingsListSubscription.unsubscribe();
    }
  }

}
