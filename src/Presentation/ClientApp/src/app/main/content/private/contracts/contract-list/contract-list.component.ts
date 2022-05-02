import { Component, OnInit, ViewEncapsulation, AfterViewInit, OnDestroy, ViewChild, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { ContractBaseModel } from '@app/core/models/contract/contract-base.model';
import { ContractGridModel } from '@app/core/models/contract/contract-grid.model';
import { fuseAnimations } from '@fuse/animations';
import { merge, Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ContractConfirmDialogComponent } from '../contract-confirm-dialog/contract-confirm-dialog.component';
import { ContractsService } from '../contracts.service';
import { EditContractFormComponent } from '../edit-contract-form/edit-contract-form.component';
import { ContractSummaryModel } from '@app/core/models/contract/contract-summary.model';
import { DataSource } from '@angular/cdk/table';

@Component({
  selector: 'app-contract-list',
  templateUrl: './contract-list.component.html',
  styleUrls: ['./contract-list.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractListComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  @Input() marginProfit: number;
  @Input() readOnly: boolean;

  dialogRef: any;
  dialogPrintRef: any;

  displayedColumns = [
    'status',
    'contractNumber',
    'buildingCode',
    'buildingName',
    'occupiedSqft',
    'profit',
    'profitAmount',
    'effectiveDate',
    'lastChangeDate',
    'buttons'];

  loading$ = this.contractService.loadingSubject.asObservable();

  dataSource: ContractDataSource | null;

  get contractsCount(): any {
    return this.contractService.elementsCount;
  }
  contracts: ContractGridModel[];
  contract: ContractGridModel;

  onContractChangedSubscription: Subscription;
  onContractDataChangedSubscription: Subscription;

  searchInput: FormControl;

  contractConfirmDialog: MatDialogRef<ContractConfirmDialogComponent>;

  constructor(
    private contractService: ContractsService,
    public matDialog: MatDialog,
    public snackBar: MatSnackBar,
    private router: Router
  ) {

    this.searchInput = new FormControl(this.contractService.searchText);

    this.onContractChangedSubscription = this.contractService
      .selectedElementsChanged
      .subscribe(contracts => {
        this.contracts = contracts;
      });

    this.onContractDataChangedSubscription = this.contractService
      .elementChanged
      .subscribe(contract => {
        this.contract = contract;
      });

  }

  ngOnInit(): void {
    this.dataSource = new ContractDataSource(this.contractService);

    this.searchInput.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged())
      .subscribe(searchText => {
        this.paginator.pageIndex = 0;
        this.contractService.searchTextChanged.next(searchText);
      });
  }

  ngAfterViewInit(): void {

    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        tap(() => this.contractService
          .getElements(
            'readall',
            '',
            this.sort.active,
            this.sort.direction,
            this.paginator.pageIndex,
            this.paginator.pageSize))
      )
      .subscribe();
  }

  ngOnDestroy(): void {
    this.onContractChangedSubscription.unsubscribe();
    this.onContractDataChangedSubscription.unsubscribe();
  }

  editContract(contractId: any): void {
    this.contractService.get(contractId)
      .subscribe(
        (contract: any) => {
          if (contract) {
            const contractUpdate = new ContractBaseModel(contract);
            this.dialogRef = this.matDialog.open(EditContractFormComponent,
              {
                panelClass: 'edit-contract-form-dialog',
                data: {
                  contract: contractUpdate,
                  action: 'edit'
                }
              });

            this.dialogRef.afterClosed()
              .subscribe((result: FormGroup) => {
                if (!result) {
                  return;
                }

                this.contractService.loadingSubject.next(true);
                const contractToUpdate = new ContractBaseModel(result.getRawValue());
                this.contractService.updateElement(contractToUpdate)
                  .then(
                    () => {
                      this.contractService.loadingSubject.next(false);
                      this.snackBar.open('Contract updated successfully!!!', 'Close', { duration: 1000 });
                    },
                    () => {
                      this.contractService.loadingSubject.next(false);
                      this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                    })
                  .catch(() => {
                    this.contractService.loadingSubject.next(false);
                    this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                  });
              });

            // this.dialogRef.afterClosed()
            //   .subscribe((response: string) => {
            //     if (!response) {
            //       return;
            //     }

            //     if (response === 'success') {
            //       this.snackBar.open('Contract updated successfully!!!', 'Close', { duration: 1000 });
            //     } else {
            //       this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
            //     }
            //   });
          } else {
            this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
          }
        }, (error) => {
          this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
        }
      );
  }

  displayBudgetDetail(id: number, guid: string): void {
    this.contractService.get(id)
      .subscribe((contractUpdate: ContractBaseModel) => {
        if (!contractUpdate.editionCompleted) {
          this.snackBar.open('Oops, contract edition not completed.', 'close', { duration: 2000 });
          return;
        }

        this.router.navigateByUrl('/app/budgets/budget-report/' + id);
      });
  }

  displayBudgetTracking(id: number, guid: string): void {
    this.contractService.get(id)
      .subscribe((contractUpdate: ContractBaseModel) => {
        if (!contractUpdate.editionCompleted) {
          this.snackBar.open('Oops, contract edition not completed.', 'close', { duration: 2000 });
          return;
        }

        this.router.navigateByUrl('/app/budgets/budget-tracking/' + id);
      });
  }

  printContractReport(): void {

  }

  deleteContract(id: number): void {
    this.contractService.get(id, 'GetContractSummary')
      .subscribe((contract: ContractSummaryModel) => {

        this.contractConfirmDialog = this.matDialog.open(ContractConfirmDialogComponent, {
          panelClass: 'contract-confirm-dialog',
          data: {
            title: 'Delete Budget',
            contract: contract,
            message: 'If you Delete Contract Number: ' + contract.contractNumber + ', you Delete next items: '
          }
        });

        this.contractConfirmDialog.afterClosed().subscribe((response: string) => {
          if (!response) {
            return;
          }

          switch (response) {
            case 'delete':
              this.contractService.deleteContract(id)
                .then(() => {
                  this.snackBar.open('Contract Deleted Successfully!', 'close', { duration: 1000 });
                })
                .catch(() => {
                  this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
                });
              break;
            default:
              break;
          }
        });
      }, (error) => {
        this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 });
      });
  }

}

export class ContractDataSource extends DataSource<any>
{
  constructor(private contractService: ContractsService) {
    super();
  }

  connect(): Observable<any[]> {
    return this.contractService.allElementsChanged;
  }

  disconnect(): void {

  }
}
