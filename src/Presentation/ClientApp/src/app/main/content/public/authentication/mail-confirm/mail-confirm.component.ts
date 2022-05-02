import { ServerDown } from '@core/error-handling/server-down';
import { NotFoundError } from '@core/error-handling/not-found-error';
import { AppError } from '@core/error-handling/app-error';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FuseConfigService } from '@fuse/services/config.service';
import { fuseAnimations } from '@fuse/animations';
import { AuthService } from '@core/services/auth.service';
import { BehaviorSubject } from 'rxjs';
import { ResetPasswordModel } from '@core/models/account/reset-password.model';

@Component({
  selector: 'fuse-mail-confirm',
  templateUrl: './mail-confirm.component.html',
  styleUrls: ['./mail-confirm.component.scss'],
  animations: fuseAnimations
})
export class FuseMailConfirmComponent implements OnInit {
  resetPasswordForm: FormGroup;
  resetPasswordFormErrors: any;
  isInvalid: boolean;
  success: boolean;
  isLoading$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  badRequest$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  email: string;
  code: string;

  constructor(
    private fuseConfig: FuseConfigService,
    private formBuilder: FormBuilder,
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

    this.resetPasswordFormErrors = {
      email: {},
      password: {},
      confirmPassword: {}
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

    this.resetPasswordForm = this.formBuilder.group({
      email: [{ value: this.email, disabled: true }, [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', [Validators.required, confirmValidator]],
      code: [this.code]
    });

    this.resetPasswordForm.valueChanges.subscribe(() => {
      this.onResetPasswordFormValuesChanged();
    });
  }

  onResetPasswordFormValuesChanged(): void {
    this.isLoading$.next(false);
    this.isInvalid = false;
    for (const field in this.resetPasswordFormErrors) {
      if (!this.resetPasswordFormErrors.hasOwnProperty(field)) {
        continue;
      }

      // Clear previous errors
      this.resetPasswordFormErrors[field] = {};

      // Get the control
      const control = this.resetPasswordForm.get(field);

      if (control && control.dirty && !control.valid) {
        this.resetPasswordFormErrors[field] = control.errors;
      }
    }
  }

  reset(resetPasswordObj: ResetPasswordModel): void {
    // Just in case
    resetPasswordObj.email = this.email;
    resetPasswordObj.code = this.code;
    this.isLoading$.next(true);
    this.authService.resetPassword(resetPasswordObj)
      .subscribe((result) => {
        // Just to avoid the MANY and MANY issues we had on presentations
        this.success = true;
        this.authService.removeToken();
      },
        error => {
          this.isInvalid = true;
          this.isLoading$.next(false);
        },
        () => {
          this.isLoading$.next(false);
        });
  }
}

function confirmValidator(control: AbstractControl): { passwordsNotMatch: boolean } {
  if (!control.parent || !control) {
    return;
  }

  const password = control.parent.get('password');
  const confirm = control.parent.get('confirmPassword');

  if (!password || !confirm) {
    return;
  }

  if (confirm.value === '') {
    return;
  }

  if (password.value !== confirm.value) {
    return {
      passwordsNotMatch: true
    };
  }
}
