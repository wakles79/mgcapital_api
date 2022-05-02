import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { SettingsService } from '@app/main/content/private/settings/settings.service';

@Component({
  selector: 'app-verify-freshdesk',
  templateUrl: './verify-freshdesk.component.html',
  styleUrls: ['./verify-freshdesk.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class VerifyFreshdeskComponent implements OnInit {

  @Input() key: string;
  @Input() agentId: string;

  isValid = false;
  isValidated = false;
  isLoading = false;

  constructor(
    private _companySettingsService: SettingsService
  ) { }

  ngOnInit(): void {
  }

  verify(): void {
    this.isLoading = true;
    this.isValidated = false;
    this.isValid = false;

    this._companySettingsService.verifyFreshdeskCredentials({ key: this.key, agentId: this.agentId })
      .subscribe((response) => {
        this.isValid = true;
        this.isValidated = true;
        this.isLoading = false;
      }, (error) => {
        this.isValidated = true;
        this.isLoading = false;
      });
  }
}
