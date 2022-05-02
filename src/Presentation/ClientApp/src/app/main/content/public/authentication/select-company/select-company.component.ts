import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from '@core/services/auth.service';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';

@Component({
  selector: 'fuse-select-company',
  templateUrl: './select-company.component.html',
  styleUrls: ['./select-company.component.scss'],
  animations: fuseAnimations
})
export class FuseSelectCompanyComponent implements OnInit {

  isLoading$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  employees: any[];
  selectedEmployee = 0;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private fuseConfig: FuseConfigService  ) {
    this.fuseConfig.config = {
      layout: {
        navbar: {
          hidden: true
        },
        toolbar: {
          hidden: true
        },
        footer: {
          hidden: true
        },
        sidepanel: {
          hidden: true
        }
      }
    };

    this.employees = this.authService.employees;
    if (!this.employees) {
      this.authService.logout();
    }
    if (this.employees.length === 1) {
      this.changeCompany();
    }
  }

  ngOnInit(): void {

  }

  changeCompany(): void {
    this.isLoading$.next(true);
    this.authService.setEmployee(this.employees[this.selectedEmployee]);
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
    this.router.navigate([returnUrl || 'app/home']);
  }

  logout(): void {
    this.authService.logout();
  }
}
