import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { ProposalBaseModel } from '@app/core/models/proposal/proposal-base.model';
import { ProposalFormComponent } from './proposal-form/proposal-form.component';
import { ProposalsService } from './proposals.service';

@Component({
  selector: 'app-proposals',
  templateUrl: './proposals.component.html',
  styleUrls: ['./proposals.component.scss']
})
export class ProposalsComponent implements OnInit {

  dialogRef: any;

  get readOnly(): boolean {
    return localStorage.getItem('readOnly') !== null;
  }

  constructor(
    private router: Router,
    private dialog: MatDialog,
    public snackBar: MatSnackBar,
    private proposalService: ProposalsService
  ) { }

  ngOnInit(): void {
  }

  newProposal(): void {
    this.dialogRef = this.dialog.open(ProposalFormComponent, {
      panelClass: 'proposals-form-dialog',
      data: {
        action: 'new'
      }
    });

    this.dialogRef.afterClosed()
      .subscribe((response: FormGroup) => {
        if (!response) {
          return;
        }

        const proposal = new ProposalBaseModel(response.getRawValue());
        this.proposalService.createElement(proposal)
          .then(
            (createdProposal) => {
              const proposalId = Number((createdProposal['body']['id']));
              this.snackBar.open('Proposal created successfully!!!', 'close', { duration: 1000 });
              this.router.navigateByUrl('/app/proposals/proposal-detail/' + proposalId);
            },
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

}
