import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FdTicketReplyModel } from '@app/core/models/freshdesk/fd-ticket-reply.model';
import { FullTicketDetailService } from '../full-ticket-detail.service';

@Component({
  selector: 'app-ticket-reply',
  templateUrl: './ticket-reply.component.html',
  styleUrls: ['./ticket-reply.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class TicketReplyComponent implements OnInit {

  @Input() replyToData: { to: string, message: string, signature?: string, cc: string[] };
  @Input() company: string;
  @Output() replyEvent = new EventEmitter<any>();

  public addCc = false;
  public abbBcc = false;

  public ccEmailText: string = '';
  public emailCc: string[] = [];

  public bccEmailText: string = '';
  public emailBcc: string[] = [];

  public attachments: File[] = [];

  public emailBody: string = '';

  public toEmail: string = '';

  private regexp = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);

  constructor(
    private _snackBar: MatSnackBar,
    private _ticketService: FullTicketDetailService
  ) {
  }

  ngOnInit(): void {
    try {
      if (this.replyToData.message !== '') {
        this.emailBody = this.replyToData.message;
      }

      this.emailCc = this.replyToData.cc;
      if (this.emailCc.length > 0) {
        this.addCc = true;
      }

      // this.toEmail = this.replyToData.to ? this.replyToData.to : '';
      // this.replyToData.to = '';
      // appends the signature to the message if any
      if (this.replyToData.signature) {
        this.emailBody += `<br>${this.replyToData.signature}<br>`;
      }
    } catch (e) {
      console.log(e);
    }
  }

  // Buttons
  changeCcOption(): void {
    this.addCc = this.addCc ? false : true;

    if (!this.addCc) {
      this.emailCc = [];
    }
  }

  changeBccOption(): void {
    this.abbBcc = this.abbBcc ? false : true;
    if (!this.abbBcc) {
      this.emailBcc = [];
    }
  }

  replyMessage(): void {


    if (this.replyToData.to === '') {
      if (!this.regexp.test(this.toEmail)) {
        this._snackBar.open('Invalid (To Email) address', 'close', { duration: 3000 });
        return;
      }
    }

    // if (!this.regexp.test(this.toEmail)) {
    //   this._snackBar.open('Invalid (To Email) address', 'close', { duration: 3000 });
    //   return;
    // }

    const response: { reply: FdTicketReplyModel, files: File[], to: string } = {
      reply: new FdTicketReplyModel({
        user_id: 63000177535,
        body: this.emailBody,
        cc_emails: this.emailCc,
        bcc_emails: this.emailBcc,
      }),
      files: this.attachments,
      to: this.toEmail
    };

    this.replyEvent.emit(response);
  }

  closeReply(): void {
    this.replyEvent.emit(null);
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

  // Cc
  addCcEmail(): void {
    if (!this.regexp.test(this.ccEmailText)) {
      this._snackBar.open('Invalid email address', 'close', { duration: 3000 });
      return;
    }

    this.emailCc.push(this.ccEmailText);
    this.ccEmailText = '';
  }

  revomeCcEmail(email: string): void {
    const index = this.emailCc.findIndex(e => e === email);
    if (index >= 0) {
      this.emailCc.splice(index, 1);
    }
  }

  // Bcc
  addBccEmail(): void {
    if (!this.regexp.test(this.bccEmailText)) {
      this._snackBar.open('Invalid email address', 'close', { duration: 3000 });
      return;
    }

    this.emailBcc.push(this.bccEmailText);
    this.bccEmailText = '';
  }

  revomeBccEmail(email: string): void {
    const index = this.emailBcc.findIndex(e => e === email);
    if (index >= 0) {
      this.emailBcc.splice(index, 1);
    }
  }
}
