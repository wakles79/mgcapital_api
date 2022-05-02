import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { WORK_ORDER_ADDITIONAL_CLOSING_NOTES } from '@app/core/models/work-order/work-order-closing-notes.model';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { Subscription } from 'rxjs';
import { WorkOrdersService } from '../../private/work-orders/work-orders.service';

@Component({
  selector: 'app-public-work-order-requester',
  templateUrl: './public-work-order-requester.component.html',
  styleUrls: ['./public-work-order-requester.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PublicWorkOrderRequesterComponent implements OnInit, OnDestroy {

  workOrder: any | undefined;
  workOrderDetailsSubcription: Subscription;

  workOrderStatuses = WORK_ORDER_STATUS;
  workOrderTypes = WORK_ORDER_TYPES;

  additionalNotes = '';

  // attachments
  woImages: Array<IAlbum> = [];
  private _subscription: Subscription;

  get attachments(): any {
    return this.workOrder.attachments;
  }

  get workOrderStatus(): string {
    return this.workOrderStatuses.find(st => st.key === this.workOrder.statusId).value.replace(/-/g, ' ');
  }

  ngClassWorkOrderStatus(statusId: any): string {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value;
  }

  get tasks(): any {
    return this.workOrder.tasks;
  }

  countTasksCheck(): any {
    return this.tasks.filter(el => el.isComplete).length;
  }

  isEmpty(obj): boolean {
    return (obj && (Object.keys(obj).length === 0));
  }

  get workOrderType(): string {
    return this.workOrderTypes.find(t => t.key === this.workOrder.type).value || 'request';
  }

  constructor(
    private fuseConfig: FuseConfigService,
    private woService: WorkOrdersService,
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

    this.workOrderDetailsSubcription = this.woService.onPublicWorkOrderDetailsChange
      .subscribe((response: any) => {
        this.workOrder = response;
        this.formatAdditionalNotes();
      });
  }

  ngOnInit(): void {
    this.checkForAttachemnts();
  }

  /*Attachments */
  checkForAttachemnts(): void {
    if (this.workOrder.attachments.length > 0) {

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

  /* End Attachments */
  formatAdditionalNotes(): void {
    for (const note in WORK_ORDER_ADDITIONAL_CLOSING_NOTES) {
      let flag = Number(WORK_ORDER_ADDITIONAL_CLOSING_NOTES[note]);
      if (flag & this.workOrder.additionalNotes) {
        let noteToAppend = note + ' ';
        if (flag === WORK_ORDER_ADDITIONAL_CLOSING_NOTES.Other) {
          noteToAppend = this.workOrder.closingNotesOther + ' ';
        }
        this.additionalNotes += noteToAppend;
      }
    }
  }

  ngOnDestroy(): void {
    if (this.workOrderDetailsSubcription && !this.workOrderDetailsSubcription.closed) {
      this.workOrderDetailsSubcription.unsubscribe();
    }
  }

}
