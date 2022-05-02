// import { InboxConverterTicketViewComponent } from '@app/main/content/private/inbox/inbox-ticket-converter-view/inbox-ticket-converter-view.component';
import { TicketDestinationType } from '@app/core/models/ticket/ticket-base.model';
// import { WorkOrderFormTemplateComponent } from '@app/main/content/components/work-order-form-template/work-order-form-template.component';
import { Component, Directive, Input } from '@angular/core';
// tslint:disable-next-line:max-line-length
// import { TicketDestinationCleaningReportItemComponent } from '@app/main/content/private/inbox/ticket-destination-cleaning-report-item/ticket-destination-cleaning-report-item.component';
import { TicketDestinationWorkOrderComponent } from '@app/main/content/private/inbox/ticket-destination-work-order/ticket-destination-work-order.component';

export class TicketDestination {

  componentToLoad: any;
  destinationType: TicketDestinationType;

  constructor(destinationType: any = null, componentToLoad: any = null) {
    this.componentToLoad = componentToLoad;
    this.destinationType = destinationType;
  }
}

export const DynamicTicketComponent = {
  'workOrder' : TicketDestinationWorkOrderComponent,
  // 'cleaningItem': TicketDestinationCleaningReportItemComponent,
  // 'findingItem': TicketDestinationCleaningReportItemComponent,
};

export interface DynamicComponent {
    data: any;
}

