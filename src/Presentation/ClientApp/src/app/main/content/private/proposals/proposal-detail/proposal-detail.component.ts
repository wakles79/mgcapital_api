import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProposalDetailModel } from '@app/core/models/proposal/proposal-detail.model';
import { fuseAnimations } from '@fuse/animations';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { ProposalDetailService } from './proposal-detail.service';
import { Location } from '@angular/common';
import { ProposalServiceFormComponent } from './proposal-service-form/proposal-service-form.component';
import { FormGroup } from '@angular/forms';
import { ProposalServiceBaseModel } from '@app/core/models/proposal-service/proposal-service-base.model';
import { PSendEmailConfirmDialogComponent } from '../p-send-email-confirm-dialog/p-send-email-confirm-dialog.component';
import { ProposalSendEmailModel } from '@app/core/models/proposal/proposal-send-email.model';
import { ShareUrlDialogComponent } from '@app/core/modules/share-url-dialog/share-url-dialog/share-url-dialog.component';
import { DataSource } from '@angular/cdk/table';

@Component({
  selector: 'app-proposal-detail',
  templateUrl: './proposal-detail.component.html',
  styleUrls: ['./proposal-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ProposalDetailComponent implements OnInit {

  dialogRef: any;
  sendEmailDialogRef: any;
  loading$ = new BehaviorSubject<boolean>(false);

  proposalDetail: ProposalDetailModel;

  proposalDataChangedSubscription: Subscription;

  dataSource: ProposalDetailServicesDataSource | any;
  columnsToDisplay = ['quantity', 'description', 'building', 'location', 'rate', 'total', 'buttons'];

  get urlToCopy(): string {
    return window.location.protocol + '//' + window.location.host + '/proposals/proposal-detail/' + this.proposalDetail.guid;
  }

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private proposalDetailService: ProposalDetailService,
    private location: Location,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
  ) {
    this.loading$.next(true);

    this.proposalDataChangedSubscription = this.proposalDetailService.onProposalDetailChanged
      .subscribe((proposalDetailData) => {
        this.loading$.next(false);

        this.proposalDetail = proposalDetailData;
      });
  }

  ngOnInit(): void {
    this.dataSource = new ProposalDetailServicesDataSource(this.proposalDetailService);
  }

  goBack(): void {
    this.location.back();
  }

  get status(): any { return this.proposalDetail.status === 1 ? 'Approved' : 'Declined'; }

  get billTo(): any {
    if (this.proposalDetail.billTo === 1) {
      return 'Tenant';
    } else if (this.proposalDetail.billTo === 2) {
      return 'Management Company';
    } else {
      return 'Undefined';
    }
  }

  /** BUTTONS */
  newProposalService(): void {
    this.dialogRef = this.dialog.open(ProposalServiceFormComponent, {
      panelClass: 'proposal-service-form-dialog',
      data: {
        action: 'new',
        proposalId: this.proposalDetail.id
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        this.loading$.next(true);
        const proposalService = new ProposalServiceBaseModel(response.getRawValue());
        this.proposalDetailService.addProposalService(proposalService)
          .then(
            () => {
              this.loading$.next(false);
              this.snackBar.open('Proposal Service added successfully!!!', 'close', { duration: 1000 });
            },
            () => {
              this.loading$.next(false);
              this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            })
          .catch(() => {
            this.loading$.next(false);
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          });
      });
  }

  editProposalService(id: number): void {
    this.loading$.next(true);
    this.proposalDetailService.getProposalService(id).subscribe(
      (proposalServiceData: any) => {
        this.loading$.next(false);
        if (proposalServiceData) {
          const proposaServiceToUpdate = new ProposalServiceBaseModel(proposalServiceData);
          this.dialogRef = this.dialog.open(ProposalServiceFormComponent, {
            panelClass: 'proposal-service-form-dialog',
            data: {
              action: 'edit',
              proposalId: this.proposalDetail.id,
              proposalService: proposaServiceToUpdate
            }
          });

          this.dialogRef.afterClosed()
            .subscribe((proposalServiceForm: FormGroup) => {
              if (!proposalServiceForm) {
                return;
              }

              this.loading$.next(true);
              const proposalService = new ProposalServiceBaseModel(proposalServiceForm.getRawValue());
              this.proposalDetailService.updateProposalService(proposalService)
                .then(
                  () => {
                    this.loading$.next(false);
                    this.snackBar.open('Proposal Service updated successfully!!!', 'close', { duration: 1000 });
                  },
                  () => {
                    this.loading$.next(false);
                    this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                  })
                .catch(() => {
                  this.loading$.next(false);
                  this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                });
            });
        } else {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      }, (error) => {
        this.loading$.next(false);
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

  sendByEmailProposalLink(): void {
    this.sendEmailDialogRef = this.dialog.open(PSendEmailConfirmDialogComponent, {
      disableClose: false,
      data: {
        proposalId: this.proposalDetail.id,
        confirmMessage: 'Are you sure you want to send this document public view link by email? Feel free to use the fields below to add additional recipients if necessary'
      }
    });

    this.sendEmailDialogRef.afterClosed().subscribe((formData: any) => {
      if (!formData) {
        return;
      }

      this.loading$.next(true);
      const proposalToSend = new ProposalSendEmailModel(formData.getRawValue());

      this.proposalDetailService.sendByEmailProposalLink(proposalToSend)
        .then(
          () => {
            this.loading$.next(false);
            this.snackBar.open('Proposal was send by email successfully!!!', 'close', { duration: 2000 });
          },
          (error) => {
            this.loading$.next(false);
            this.snackBar.open(error, 'close', { duration: 1000 });
          })
        .catch((error) => {
          this.loading$.next(false);
          this.snackBar.open(error, 'close', { duration: 1000 });
        });

      this.sendEmailDialogRef = null;
    });
  }

  shareProposalDocument(): void {
    this.dialogRef = this.dialog.open(ShareUrlDialogComponent, {
      panelClass: 'share-url-form-dialog',
      data: {
        urlToCopy: this.urlToCopy
      }
    });
  }

  openPublicProposalViewNewTab(): void {
    window.open(this.urlToCopy, '_blank');
  }

  /** MATH */
  get total(): number {
    let total = 0;
    // return this.dsContractItems.map(i => i.dailyRate).reduce((acc, value) => acc + value, 0);
    this.proposalDetail.proposalServices.forEach(x => total += (x.rate * x.quantity));
    return total;
  }

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
