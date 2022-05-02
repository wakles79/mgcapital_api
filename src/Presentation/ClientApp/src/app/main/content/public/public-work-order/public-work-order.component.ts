import { Component, OnInit, ViewEncapsulation, OnDestroy } from '@angular/core';
import { WORK_ORDER_ADDITIONAL_CLOSING_NOTES } from '@app/core/models/work-order/work-order-closing-notes.model';
import { WORK_ORDERS_PRIORITIES } from '@app/core/models/work-order/work-order-priorities.model';
import { WORK_ORDER_STATUS } from '@app/core/models/work-order/work-order-status.model';
import { WORK_ORDER_TYPES } from '@app/core/models/work-order/work-order-type.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';
import { IAlbum, IEvent, Lightbox, LightboxConfig, LightboxEvent, LIGHTBOX_EVENT } from 'ngx-lightbox';
import { BehaviorSubject, Subscription } from 'rxjs';
import { WorkOrdersService } from '../../private/work-orders/work-orders.service';

@Component({
  selector: 'app-public-work-order',
  templateUrl: './public-work-order.component.html',
  styleUrls: ['./public-work-order.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PublicWorkOrderComponent implements OnInit, OnDestroy {

  workOrder: any | undefined;
  workOrderDetailsSubcription: Subscription;
  statusLog$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  activityLog$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

  workOderPriorities: { id: number, name: string }[] = [];
  workOrderTypes = WORK_ORDER_TYPES;
  workOrderStatuses = WORK_ORDER_STATUS;

  additionalNotes = '';
  config: any;

  // attachments
  woImages: Array<IAlbum> = [];
  private _subscription: Subscription;

  get attachments(): any {
    return this.workOrder.attachments;
  }

  get workOrderType(): string {
    return this.workOrderTypes.find(t => t.key === this.workOrder.type).value || 'request';
  }

  get workOrderStatus(): string {
    return this.workOrderStatuses.find(st => st.key === this.workOrder.statusId).value.replace(/-/g, ' ');
  }

  ngClassWorkOrderStatus(statusId: any): string {
    return WORK_ORDER_STATUS.find(item => item.key === statusId).value;
  }

  getStatusName(slug: string): string {
    return slug.replace(/-/g, ' ');
  }

  get workOrderPriority(): string {
    return this.workOderPriorities.find(pr => pr.id === this.workOrder.priority).name;
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

  get notes(): any {
    return this.workOrder.notes;
  }

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
        this.getStatusLog();
      });
  }

  getPriorities(): void {
    /*Covert enum WORK_ORDERS_PRIORITIES to array workOrderPriorities*/
    for (const n in WORK_ORDERS_PRIORITIES) {
      if (typeof WORK_ORDERS_PRIORITIES[n] === 'number') {
        this.workOderPriorities.push({ id: WORK_ORDERS_PRIORITIES[n] as any, name: n.replace(/_/g, ' ') });
      }
    }
    this.workOderPriorities.push({ id: 0, name: 'low' });
  }

  getStatusLog(): void {
    this.woService.getStatusLog(null, this.workOrder.id)
      .subscribe((response: { payload: any[], count: number }) => {
        this.statusLog$.next(response.payload);
      },
        (error) => console.log('Error fetching status log.'));
  }

  getActivityLog(): void {
    this.woService.getActivityLog(null, this.workOrder.id)
      .subscribe((response: { payload: any[], count: number }) => {
        this.activityLog$.next(response.payload);
      },
        (error) => console.log('Error fetching activity log.'));
  }

  ngOnInit(): void {
    this.getPriorities();
    this.checkForAttachemnts();

    this.getActivityLog();

    // Subscribe to config change
    this.fuseConfig.config
      .subscribe((config) => {
        this.config = config;
      });
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

  splitStringByUppercase(stringToSplit: any): any {
    return stringToSplit.match(/[A-Z][a-z]+|[0-9]+/g).join(' ');
  }

  ngOnDestroy(): void {
    if (this.workOrderDetailsSubcription && !this.workOrderDetailsSubcription.closed) {
      this.workOrderDetailsSubcription.unsubscribe();
    }
  }
}
