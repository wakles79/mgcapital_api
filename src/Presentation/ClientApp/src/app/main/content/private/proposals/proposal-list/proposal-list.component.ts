import { AfterViewInit, Component, Input, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ProposalBaseModel } from '@app/core/models/proposal/proposal-base.model';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfirmDialogComponent } from '@fuse/components/confirm-dialog/confirm-dialog.component';
import { ProposalGridModel } from '@app/core/models/proposal/proposal-grid.model';
import { merge, Observable, Subscription } from 'rxjs';
import { FormControl, FormGroup } from '@angular/forms';
import { ProposalsService } from '../proposals.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ProposalFormComponent } from '../proposal-form/proposal-form.component';
import { DataSource } from '@angular/cdk/table';

@Component({
  selector: 'app-proposal-list',
  templateUrl: './proposal-list.component.html',
  styleUrls: ['./proposal-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ProposalListComponent implements OnInit, AfterViewInit {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() readOnly: boolean;

  loading$ = this.proposalService.loadingSubject.asObservable();

  dialogRef: any;

  confirmDialogRef: MatDialogRef<FuseConfirmDialogComponent>;

  get proposalsCount(): any { return this.proposalService.elementsCount; }
  proposals: ProposalGridModel[] = [];
  proposal: ProposalBaseModel;
  dataSource: ProposalDataSource | null;
  columnsToDisplay = ['number', 'createdOn', 'preparedFor', 'mgmtCompany', 'lineItems', 'value', 'status', 'buttons'];

  onProposalsChangedSubscription: Subscription;
  onProposalsDataChangedSubscription: Subscription;

  searchInput: FormControl;

  constructor(
    private proposalService: ProposalsService,
    public dialog: MatDialog,
    public snackBar: MatSnackBar
  ) {

    this.searchInput = new FormControl(this.proposalService.searchText);

    this.onProposalsChangedSubscription = this.proposalService.allElementsChanged
      .subscribe(proposals => {
        this.proposals = proposals;
      });

    this.onProposalsDataChangedSubscription = this.proposalService.elementChanged
      .subscribe(proposal => {
        this.proposal = proposal;
      });
  }

  ngOnInit(): void {
    this.dataSource = new ProposalDataSource(this.proposalService);

    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.proposalService.searchTextChanged.next(searchText);
      });
  }

  ngAfterViewInit(): void {
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.proposalService.getElements(
          'readall', '',
          this.sort.active,
          this.sort.direction,
          this.paginator.pageIndex,
          this.paginator.pageSize
        ))
      )
      .subscribe();
  }

  editProposal(proposalId: number): void {
    this.proposalService.get(proposalId)
      .subscribe((response: any) => {
        if (!response) {
          return;
        }

        const proposalData = new ProposalBaseModel(response);
        this.dialogRef = this.dialog.open(ProposalFormComponent, {
          panelClass: 'proposals-form-dialog',
          data: {
            action: 'edit',
            proposal: proposalData
          }
        });

        this.dialogRef.afterClosed()
          .subscribe((proposalToUpdate: FormGroup) => {
            if (!proposalToUpdate) {
              return;
            }

            const proposal = new ProposalBaseModel(proposalToUpdate.getRawValue());

            this.proposalService.updateElement(proposal)
              .then(
                () => this.snackBar.open('Proposal updated successfully!!!', 'close', { duration: 1000 }),
                () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
              .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
          });
      }, (error) => { this.snackBar.open('Oops, there was an error retreiving proposal', 'close', { duration: 1000 }); }
      );
  }

  deleteProposal(proposal): void {
    this.confirmDialogRef = this.dialog.open(FuseConfirmDialogComponent, {
      disableClose: false
    });
    this.confirmDialogRef.componentInstance.confirmMessage = 'Are you sure you want to delete?';

    this.confirmDialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.proposalService.delete(proposal).then(
          () => this.snackBar.open('Proposal delete successfully!!!', 'close', { duration: 3000 }),
          () => this.snackBar.open('Oops,  Cannot be deleted, this item contains more than 1 service', 'close', { duration: 3000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 3000 }));
      }
      this.confirmDialogRef = null;
    });
  }

}

export class ProposalDataSource extends DataSource<any>{

  constructor(private proposalService: ProposalsService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.proposalService.allElementsChanged;
  }

  disconnect(): void {
  }

}
