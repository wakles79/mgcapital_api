import { Component, OnInit, OnDestroy, ViewEncapsulation, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { ContractOfficeSpaceModel } from '@app/core/models/contract/contract-office-space.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { OfficeTypesService } from '../../office-types/office-types.service';

@Component({
  selector: 'app-office-space-form',
  templateUrl: './office-space-form.component.html',
  styleUrls: ['./office-space-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class OfficeSpaceFormComponent implements OnInit, OnDestroy {

  action: string;
  dialogTitle: string;
  officeSpaceForm: FormGroup;
  addedTypes: string;
  officeSpace: ContractOfficeSpaceModel;
  isLoading = true;
  officeTypeService: any;
  officeServiceTypes: ListItemModel[] = [];
  filteredOfficeServiceTypes: Subject<any[]> = new Subject<any[]>();
  listOfficeServiceTypesSubscription: Subscription;

  constructor(
    public dialogRef: MatDialogRef<OfficeSpaceFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private officeServiceTypeService: OfficeTypesService,
    private snackBar: MatSnackBar
  ) {
    this.action = data.action;
    this.addedTypes = data.addedTypes;

    if (this.action === 'new') {
      this.dialogTitle = 'New Office Space';
      this.officeSpaceForm = this.createForm();
    } else {
      this.dialogTitle = 'Edit Office Space';
      this.officeSpace = data.officeSpace;
      this.officeSpaceForm = this.updateForm();
    }
  }

  ngOnInit(): void {
    this.isLoading = true;
    this.getAllOfficeServiceTypes();
  }

  ngOnDestroy(): void {
    if (this.listOfficeServiceTypesSubscription && !this.listOfficeServiceTypesSubscription.closed) {
      this.listOfficeServiceTypesSubscription.unsubscribe();
    }
  }

  /** Form */
  createForm(): FormGroup {
    return this.formBuilder.group({
      officeTypeId: [null, [Validators.required]],
      officeTypeName: [''],
      squareFeet: [null, [Validators.required]]
    });
  }

  updateForm(): FormGroup {
    return this.formBuilder.group({
      id: this.officeSpace.id,
      officeTypeId: [this.officeSpace.officeTypeId, [Validators.required]],
      officeTypeName: [this.officeSpace.officeTypeName],
      squareFeet: [this.officeSpace.squareFeet, [Validators.required]]
    });
  }

  // Office Service Type
  getAllOfficeServiceTypes(): void {

    if (this.listOfficeServiceTypesSubscription && !this.listOfficeServiceTypesSubscription.closed) {
      this.listOfficeServiceTypesSubscription.unsubscribe();
    }

    this.listOfficeServiceTypesSubscription = this.officeServiceTypeService
      .getAllAsList('ReadAllCbo', '', 0, 999, 1, { 'status': '1', 'rateType': '3', 'exclude': this.addedTypes })
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.officeServiceTypes = response.payload;
        this.filteredOfficeServiceTypes.next(this.officeServiceTypes);
        this.isLoading = false;

        if (this.officeServiceTypes.length === 0) {
          this.snackBar.open('No records available!!!', 'Close', { duration: 1500 });
        }
      });
  }

  typeChanged(id: number): void {
    this.officeSpaceForm
      .patchValue({
        officeTypeName: this.officeServiceTypes.filter(i => i.id === id)[0].name
      });
  }

}
