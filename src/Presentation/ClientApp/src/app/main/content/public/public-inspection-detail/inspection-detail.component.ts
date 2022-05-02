import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { InspectionDetailModel } from '@app/core/models/inspections/inspection-detail.model';
import { CleaningReportItemAttachmentModel } from '@app/core/models/reports/cleaning-report/cleaning.report.item.attachment.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { BehaviorSubject, Subscription } from 'rxjs';
import { InspectionItemsDataSource } from '../../private/inspections/inspection-detail/inspection-detail.component';
import { InspectionDetailService } from '../../private/inspections/inspection-detail/inspection-detail.service';

@Component({
  selector: 'app-inspection-detail',
  templateUrl: './inspection-detail.component.html',
  styleUrls: ['./inspection-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class InspectionDetailComponent implements OnInit {

  inspectionDetail: InspectionDetailModel;
  inspectionDataChangedSubscription: Subscription;

  inspectionItemsDataSource: InspectionItemsDataSource;
  columnsToDisplay = ['number', 'position', 'description', 'attachments'];

  loading$ = new BehaviorSubject<boolean>(false);

  images: Array<IAlbum> = [];
  private _subscription: Subscription;
  get attachments(): CleaningReportItemAttachmentModel[] {
    let attachments: CleaningReportItemAttachmentModel[] = [];

    if (this.inspectionDetail) {
      // Gets inspection item attachments
      if (this.inspectionDetail.inspectionItem) {
        this.inspectionDetail.inspectionItem.forEach(cI => {
          attachments = attachments.concat(cI.attachments);
        });
      }
    }

    return attachments;
  }

  constructor(
    private fuseConfig: FuseConfigService,
    private inspectionDetailService: InspectionDetailService,
    private _lightbox: Lightbox,
    private _lightboxEvent: LightboxEvent,
    private _lighboxConfig: LightboxConfig,
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

    this.loading$.next(true);
    this.inspectionDataChangedSubscription = this.inspectionDetailService.onInspectionDetailChanged
      .subscribe((inspectionDetailData) => {
        this.loading$.next(true);
        this.inspectionDetail = inspectionDetailData;
        this.checkForAttachments();
      });
  }

  ngOnInit(): void {
    this.inspectionItemsDataSource = new InspectionItemsDataSource(this.inspectionDetailService);
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

}
