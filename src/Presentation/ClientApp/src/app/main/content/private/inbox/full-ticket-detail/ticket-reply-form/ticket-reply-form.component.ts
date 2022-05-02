import { Component, OnInit, ViewEncapsulation, OnDestroy, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FdTicketReplyModel } from '@app/core/models/freshdesk/fd-ticket-reply.model';
import { fuseAnimations } from '@fuse/animations';
import { FullTicketDetailService } from '../full-ticket-detail.service';

@Component({
  selector: 'app-ticket-reply-form',
  templateUrl: './ticket-reply-form.component.html',
  styleUrls: ['./ticket-reply-form.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class TicketReplyFormComponent implements OnInit, OnDestroy {

  public ticketNumberMGCAP: string;
  public to_email: string;

  public replyTicketForm: FormGroup;

  public CcEmail: string[] = [];
  public attachments: File[] = [];

  public ccText = '';
  private regexp = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);

  constructor(
    public dialogRef: MatDialogRef<TicketReplyFormComponent>,
    private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) data: any,
    private _snackBar: MatSnackBar,
    private _ticketService: FullTicketDetailService
  ) {

    this.ticketNumberMGCAP = data.ticketNumberMGCAP;
    this.to_email = data.to_email;

    this.replyTicketForm = this.createSendReplyTicketForm();
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
  }

  createSendReplyTicketForm(): FormGroup {
    return this.formBuilder.group({
      body: ['', [Validators.required]]
    });
  }

  // CC
  addCCEmail(): void {
    if (!this.regexp.test(this.ccText)) {
      this._snackBar.open('Invalid email address', 'close', { duration: 3000 });
      return;
    }

    this.CcEmail.push(this.ccText);
    this.ccText = '';
  }
  revomeCcEmail(email: string): void {
    const index = this.CcEmail.findIndex(e => e === email);
    if (index >= 0) {
      this.CcEmail.splice(index, 1);
    }
  }

  // Files
  fileChange(files: File[]): void {
    try {
      if (files.length > 0) {
        for (let i = 0; i < files.length; i++) {
          this.attachments.push(files[i]);
        }
      }
    } catch (ex) { console.log(ex); }
  }
  removeFile(index: number): void {
    this.attachments.splice(index, 1);
  }

  // Actions
  submit(): void {

    const objTicketReply = new FdTicketReplyModel(this.replyTicketForm.getRawValue());
    objTicketReply.ticket_id = 20;
    objTicketReply.user_id = 63000177535;

    // this._ticketService.sendTicketReply(objTicketReply, this.attachments)
    //   .subscribe(() => {
    //     this._snackBar.open('success', 'close', { duration: 3000 });
    //   }, (error) => {
    //     this._snackBar.open('failed', 'close', { duration: 3000 });
    //     console.log(JSON.stringify(error));
    //   });
  }

}
