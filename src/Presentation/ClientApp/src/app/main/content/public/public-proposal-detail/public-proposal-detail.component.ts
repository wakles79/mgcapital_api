import { DataSource } from '@angular/cdk/table';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProposalDetailModel } from '@app/core/models/proposal/proposal-detail.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { FuseConfigService } from '@fuse/services/config.service';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { ProposalDetailService } from '../../private/proposals/proposal-detail/proposal-detail.service';
import { ApprovedProposalConfirmDialogComponent } from './approved-proposal-confirm-dialog/approved-proposal-confirm-dialog.component';

@Component({
  selector: 'app-public-proposal-detail',
  templateUrl: './public-proposal-detail.component.html',
  styleUrls: ['./public-proposal-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class PublicProposalDetailComponent implements OnInit {

  confirmProposalDialogRef: MatDialogRef<ApprovedProposalConfirmDialogComponent>;
  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  loading$ = new BehaviorSubject<boolean>(false);

  proposalDetail: ProposalDetailModel;

  proposalDataChangedSubscription: Subscription;

  dataSource: ProposalDetailServicesDataSource | any;
  columnsToDisplay = ['quantity', 'description', 'building', 'location', 'rate', 'total'];

  isChecked = false;

  get billTo(): AbstractControl { return this.billToForm.get('billTo'); }
  get billToName(): AbstractControl { return this.billToForm.get('billToName'); }
  get billToEmail(): AbstractControl { return this.billToForm.get('billToEmail'); }

  billToForm: FormGroup;
  formBuilder: FormBuilder;
  config: any;

  constructor(
    private fuseConfig: FuseConfigService,
    private proposalDetailService: ProposalDetailService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar
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

    this.formBuilder = new FormBuilder();
    this.billToForm = this.formBuilder.group({
      billTo: ['1'],
      billToName: ['', [Validators.required]],
      billToEmail: ['', [Validators.required, Validators.email]]
    });

    this.proposalDataChangedSubscription = this.proposalDetailService.onProposalDetailChanged
      .subscribe((proposalDetailData) => {
        this.loading$.next(false);

        this.proposalDetail = proposalDetailData;

      });
  }

  ngOnInit(): void {
    this.dataSource = new ProposalDetailServicesDataSource(this.proposalDetailService);

    // Subscribe to config change
    this.fuseConfig.config
      .subscribe((config) => {
        this.config = config;
      });
  }

  get status(): any { return this.proposalDetail.status === 1 ? 'Approved' : 'Declined'; }

  get billToValue(): any {
    if (this.proposalDetail.billTo === 1) {
      return 'Tenant';
    } else if (this.proposalDetail.billTo === 2) {
      return 'Management Company';
    } else {
      return 'Undefined';
    }
  }

  /** MATH */
  get total(): number {
    let total = 0;
    // return this.dsContractItems.map(i => i.dailyRate).reduce((acc, value) => acc + value, 0);
    this.proposalDetail.proposalServices.forEach(x => total += (x.rate * x.quantity));
    return total;
  }

  /** BUTTONS */
  approveProposal(): void {
    if (!this.isChecked) {
      this.snackBar.open('Slide the toggle!!!', 'close', { duration: 1000 });
      return;
    }

    if (this.billToForm.invalid) {
      this.snackBar.open('Oops! Invalid name or email', 'close', { duration: 1000 });
      return;
    }

    if (!this.validateEmail(this.billToForm.get('billToEmail').value)) {
      this.snackBar.open('Oops! Invalid email.', 'close', { duration: 1000 });
      return;
    }

    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to approve this proposal?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.proposalDetailService.setStatus(this.proposalDetail.id, 1, this.billToName.value, this.billToEmail.value, this.billTo.value)
          .then(
            () => {
              this.loading$.next(false);
              this.snackBar.open('Proposal was approved successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      }
      this.confirmDialogRef = null;
    });
  }

  declineProposal(): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });

    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to decline this proposal?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.proposalDetailService.setStatus(this.proposalDetail.id, 2, '', '')
          .then(
            () => {
              this.loading$.next(false);
              this.snackBar.open('Proposal was declined successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      }
      this.confirmDialogRef = null;
    });
  }

  /** Helper */
  validateEmail(email: string): boolean {
    const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
  }

  editProposalService(): void { }

}

export class ProposalDetailServicesDataSource extends DataSource<any>{
  constructor(private service: ProposalDetailService) {
    super();
  }

  /** Connect function called by the table to retrieve one stream containing the data to render. */
  connect(): Observable<any[]> {
    return this.service.onProposalServicesChanged;
  }

  disconnect(): void { }
}

