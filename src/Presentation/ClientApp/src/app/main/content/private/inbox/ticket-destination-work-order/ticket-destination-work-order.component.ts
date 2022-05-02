import { AfterViewInit, Component, OnInit, OnDestroy, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TicketBaseModel, TicketDestinationType } from '@app/core/models/ticket/ticket-base.model';
import { WorkOrderSourceCode } from '@app/core/models/work-order/work-order-base.model';
import { WorkOrderCreateModel } from '@app/core/models/work-order/work-order-create.model';
import { WorkOrderScheduleSetting } from '@app/core/models/work-order/work-order-schedule-setting.model';
import { WorkOrderFormTemplateComponent } from '@app/core/modules/work-order-form/work-order-form-template/work-order-form-template.component';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { WorkOrdersService } from '../../work-orders/work-orders.service';
import { TicketsService } from '../tickets.service';
import { WorkOrderCreateFromTicket } from '@app/core/models/work-order/work-order-create-from-ticket.model';
import { ConvertTicketParameters } from '@app/core/models/ticket/object-parameters/convert-ticket.model';

@Component({
  selector: 'app-ticket-destination-work-order',
  templateUrl: './ticket-destination-work-order.component.html',
  styleUrls: ['./ticket-destination-work-order.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class TicketDestinationWorkOrderComponent implements OnInit, AfterViewInit, OnDestroy {

  ticket: TicketBaseModel;
  onCurrentTicketChanged: Subscription;
  dialogRef: any;
  loadingList$ = new BehaviorSubject<boolean>(false);

  public REF = {
    TicketDestinationType: TicketDestinationType,
  };

  @Input() data: any;
  @ViewChild(WorkOrderFormTemplateComponent) workOrderTemplate: WorkOrderFormTemplateComponent;

  constructor(
    private workOrderService: WorkOrdersService,
    private ticketService: TicketsService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
  }

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  ngOnInit(): void {
    this.onCurrentTicketChanged =
      this.ticketService.onCurrentTicketChanged
        .subscribe(currentTicket => {
          // console.log({currentTicket});
          this.ticket = currentTicket;
        });
  }

  ngAfterViewInit(): void {
    this.workOrderTemplate.onWorkOrderTemplateSubmitted.subscribe((payload: FormGroup) => {

      if (payload) {
        const setStatusByStandBy: boolean = payload.get('setStatusByStandBy').value ? payload.get('setStatusByStandBy').value : false;
        const workOrderToCreate = new WorkOrderCreateModel(payload.getRawValue());
        this.addWorkOrder(workOrderToCreate, setStatusByStandBy);
      }
    });
  }

  addWorkOrder(workOrderToCreate: WorkOrderCreateModel, setStatusByStandBy: boolean): void {
    this.loadingList$.next(true);

    workOrderToCreate.sourceCode = WorkOrderSourceCode.Ticket;
    const scheduleSettings = workOrderToCreate.scheduleSettings ? new WorkOrderScheduleSetting(workOrderToCreate.scheduleSettings) : null;

    if (scheduleSettings) {
      try {
        // Create multiple work Order
        this.workOrderService.create(scheduleSettings, 'AddScheduleSettings')
          .subscribe((settingsResult: any) => {
            const settingsId = settingsResult['body'].id;
            workOrderToCreate.workOrderScheduleSettingId = settingsId;
            let dates: Date[] = [];
            dates = this.workOrderService.calculateScheduleDates(scheduleSettings);

            if (dates.length > 0) {
              const workOrders: any[] = [];
              let index = 0;

              dates.forEach(d => {
                let unscheduled = false;
                if (!scheduleSettings.scheduleDate && (scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6)) {
                  unscheduled = true;
                } else if (index > 0 && (scheduleSettings.frequency > 2 && scheduleSettings.frequency < 6)) {
                  unscheduled = true;
                }

                const newWo = new WorkOrderCreateFromTicket(workOrderToCreate);
                newWo.ticketId = this.ticket.id;
                newWo.dueDate = setStatusByStandBy ? d : null;
                newWo.statusId = setStatusByStandBy ? 1 : 0;
                newWo.scheduleDate = d;
                newWo.unscheduled = setStatusByStandBy ? false : unscheduled;
                workOrders.push(newWo);
                index++;
              });

              this.saveWorkOrders(workOrders, settingsId);
            } else {
              this.loadingList$.next(false);
            }
          });
      } catch (error) { console.log(error); }
    }
    else {
      this.createWorkOrder(workOrderToCreate);
    }
  }

  createWorkOrder(workOrderToCreate: any): void {
    this.workOrderService.createElement(workOrderToCreate)
      .then((response: any) => {
        this.loadingList$.next(false);
        this.workOrderTemplate.workOrderForm.disable();
        this.workOrderTemplate.buttonSaveDisabled = true;

        const workOrderId = response.body.id;
        const convertTicketParameters: ConvertTicketParameters = new ConvertTicketParameters(this.ticket.id, workOrderId, this.REF.TicketDestinationType.workOrder);

        this.convertTicket(convertTicketParameters);

        this.snackBar.open('Work Order created successfully!!!', 'close', { duration: 1000 });
      },
        () => {
          this.loadingList$.next(false);
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        })
      .catch((error) => {
        this.loadingList$.next(false);
        // console.log({error});
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  async saveWorkOrders(workOrders: any[], scheduleSettingId): Promise<any> {
    let workOrderId = 0;
    for (let index = 0; index < workOrders.length; index++) {
      try {
        const result = await this.workOrderService.createFromCalendar(workOrders[index], 'AddFromTicket').toPromise();
        if (result && index === 0) {
          if (result.id) {
            workOrderId = result.id;
          }
        }
      } catch (error) {
      }
    }

    this.loadingList$.next(false);
    this.snackBar.open('Work Order sequence created successfully!!!', 'close', { duration: 1000 });
    this.ticketService.onTicketUpdated.next('converted');
  }

  convertTicket(convertTicketParameters: ConvertTicketParameters, scheduleSettingId = 0): void {
    this.ticketService.convertTicket(convertTicketParameters)
      .catch(() =>
        this.snackBar.open('Oops, there was an error changed ticket status', 'close', { duration: 1000 })
      );
  }

  ngOnDestroy(): void {
  }

}
