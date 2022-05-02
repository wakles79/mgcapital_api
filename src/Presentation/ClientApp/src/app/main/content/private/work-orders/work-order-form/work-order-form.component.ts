import { AfterViewInit, Component, OnInit, ViewEncapsulation, OnDestroy, ViewChild, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { WorkOrderFormTemplateComponent } from '@app/core/modules/work-order-form/work-order-form-template/work-order-form-template.component';
import { AuthService } from '@app/core/services/auth.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-work-order-form',
  templateUrl: './work-order-form.component.html',
  styleUrls: ['./work-order-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class WorkOrderFormComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(WorkOrderFormTemplateComponent) workOrderTemplate: WorkOrderFormTemplateComponent;
  dataWO: any;

  constructor(
    public dialogRef: MatDialogRef<WorkOrderFormComponent>,
    private authService: AuthService,
    @Inject(MAT_DIALOG_DATA) private data: any,
  ) {
    this.dataWO = data;
  }

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.workOrderTemplate.onWorkOrderTemplateSubmitted.subscribe((payload: any) => {
      this.dialogRef.close(payload);
    });
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

}
