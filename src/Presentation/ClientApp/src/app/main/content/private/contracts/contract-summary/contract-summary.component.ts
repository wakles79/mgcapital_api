import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ContractSummaryModel } from '@app/core/models/contract/contract-summary.model';
import { fuseAnimations } from '@fuse/animations';

@Component({
  selector: 'app-contract-summary',
  templateUrl: './contract-summary.component.html',
  styleUrls: ['./contract-summary.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ContractSummaryComponent implements OnInit {

  @Input() displayDetails: boolean;
  @Input() contractSummary: ContractSummaryModel;

  constructor() { }

  ngOnInit(): void {
  }

}
