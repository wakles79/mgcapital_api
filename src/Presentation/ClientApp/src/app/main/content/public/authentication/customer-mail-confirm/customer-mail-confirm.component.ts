import { ServerDown } from '@core/error-handling/server-down';
import { NotFoundError } from '@core/error-handling/not-found-error';
import { AppError } from '@core/error-handling/app-error';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '@core/services/auth.service';
import { BehaviorSubject } from 'rxjs';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';

@Component({
  selector: 'fuse-customer-mail-confirm',
  templateUrl: './customer-mail-confirm.component.html',
  styleUrls: ['./customer-mail-confirm.component.scss'],
  animations: fuseAnimations
})
export class FuseCustomerMailConfirmComponent implements OnInit {
  isInvalid: boolean;
  success: boolean;
  isLoading$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  badRequest$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  email: string;
  code: string;

  constructor(
    private fuseConfig: FuseConfigService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private router: Router
  ) {
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

    // Gets all corresponding query string params
    const params = this.route.snapshot.queryParamMap;
    const userId = params.get('userId');
    const confirm = params.get('confirm');
    this.code = params.get('code');

    // If at least one of the params is null we sentence
    // the navigation to 404
    if (!(userId && confirm && this.code)) {
      this.router.navigate(['errors/error-404']);
    }

    this.authService.confirmUser(userId, confirm, this.code)
      .subscribe((response: any) => {
        if (response && response.email) {
          this.email = response.email;
        } else {
          this.badRequest$.next(true);
        }
      },
        (error: AppError) => {
          if (error instanceof NotFoundError) {
            this.router.navigate(['errors/error-404']);
          } else if (error instanceof ServerDown) {
            this.router.navigate(['errors/error-500']);
          }
          console.log(error);
          this.badRequest$.next(true);
        });
  }

  ngOnInit(): void {

  }
}
