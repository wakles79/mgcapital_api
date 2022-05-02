import { Component, Input, OnInit, ViewEncapsulation, NgModule } from '@angular/core';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-ticket-fd-conversation',
  templateUrl: './ticket-fd-conversation.component.html',
  styleUrls: ['./ticket-fd-conversation.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.ShadowDom
})
export class TicketFdConversationComponent implements OnInit {

  showBlockquote: boolean = false;
  showMoreButton: boolean = false;
  moreButtonText: string = "show more...";
  @Input() html;
  @Input() blockquote;

  get styledHtml(): string {
    const styles = `
    <style>
      blockquote {
        margin: 0px 0px 0px 0.8ex !important;
        border-left: 1px solid #6e5d5d !important;
        padding-left: 1ex !important;
      }
    </style>
    `;

    return `${styles}${this.html}`;
  }
  
  constructor() { }

  ngOnInit(): void {
    if(this.blockquote!=null) this.showMoreButton = true;
  }

  hideBlockquote(): void {
    this.showBlockquote = !this.showBlockquote;
    if(this.showBlockquote) this.moreButtonText = "show less...";
    else this.moreButtonText = "show more...";
  }

}
