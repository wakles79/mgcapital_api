import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { fuseAnimations } from '@fuse/animations';
import { FuseConfigService } from '@fuse/services/config.service';

@Component({
  selector: 'fuse-lock',
  templateUrl: './lock.component.html',
  styleUrls: ['./lock.component.scss'],
  animations: fuseAnimations
})
export class FuseLockComponent implements OnInit {
  lockForm: FormGroup;
  lockFormErrors: any;

  constructor(
    private fuseConfig: FuseConfigService,
    private formBuilder: FormBuilder
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

    this.lockFormErrors = {
      username: {},
      password: {}
    };
  }

  ngOnInit(): void {
    this.lockForm = this.formBuilder.group({
      username: [
        {
          value: 'Katherine',
          disabled: true
        }, Validators.required
      ],
      password: ['', Validators.required]
    });

    this.lockForm.valueChanges.subscribe(() => {
      this.onLockFormValuesChanged();
    });
  }

  onLockFormValuesChanged(): void {
    for (const field in this.lockFormErrors) {
      if (!this.lockFormErrors.hasOwnProperty(field)) {
        continue;
      }

      // Clear previous errors
      this.lockFormErrors[field] = {};

      // Get the control
      const control = this.lockForm.get(field);

      if (control && control.dirty && !control.valid) {
        this.lockFormErrors[field] = control.errors;
      }
    }
  }
}
