import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-ticket-fd-detail',
  templateUrl: './ticket-fd-detail.component.html',
  styleUrls: ['./ticket-fd-detail.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.ShadowDom
})
export class TicketFdDetailComponent implements OnInit {

  @Input() html;

  constructor() { }

  ngOnInit(): void {
  }

}
