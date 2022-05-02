import { Component, OnInit, ViewEncapsulation, Inject, NgZone, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MapsAPILoader } from '@agm/core';
import { AddressModel } from '@core/models/common/address.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { fuseAnimations } from '@fuse/animations';

declare var google: any;

@Component({
  selector: 'fuse-address-form',
  templateUrl: './address-form.component.html',
  styleUrls: ['./address-form.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddressFormComponent implements OnInit {
  address: AddressModel;
  addressForm: FormGroup;
  dialogTitle: string;
  action: string;
  entityId: number;

  // If type == building, hide fields "type" and "name"
  type: any;

  @ViewChild('search', { static: true })
  public searchElementRef: ElementRef;


  constructor(
    public addressDialogRef: MatDialogRef<AddressFormComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private formBuilder: FormBuilder,
    private mapsAPILoader: MapsAPILoader,
    private ngZone: NgZone
  ) {
    this.action = data.action;
    this.type = data.type;
    this.entityId = data.entityId;

    if (this.action === 'edit') {
      this.dialogTitle = 'Edit Address';
      this.address = data.address;
      this.addressForm = this.createAddressUpdateForm();
    }
    else {
      this.dialogTitle = 'New Address';
      this.addressForm = this.createAddressCreateForm();

    }
  }

  ngOnInit(): void {
    // load Places Autocomplete
    this.mapsAPILoader.load().then(() => {
      const autocomplete = new google.maps.places.Autocomplete(this.searchElementRef.nativeElement);
      autocomplete.addListener('place_changed', () => {
        this.ngZone.run(() => {
          const place = autocomplete.getPlace();
          const components = place.address_components;
          // Getting all separate fields from place object
          const loader = {
            locality: '',
            administrative_area_level_1: '',
            postal_code: '',
            street_number: '',
            route: '',
            subpremise: '',
            country: ''
          };


          // tslint:disable-next-line: prefer-for-of
          for (let i = 0; i < components.length; i++) {
            const ac = components[i];
            const types = ac.types;
            if (types) {
              const t = types[0];
              loader[t] = ac.short_name;
            }
          }

          // Just mandatory formats for address_line
          // route
          if (loader.route !== '') {
            loader.route = ' ' + loader.route;
          }
          // apt number
          if (loader.subpremise !== '') {
            loader.subpremise = '#' + loader.subpremise;
          }

          // Setting results
          this.addressForm.patchValue(
            {
              addressLine1: loader.street_number + loader.route,
              addressLine2: loader.subpremise,
              city: loader.locality,
              state: loader.administrative_area_level_1,
              zipCode: loader.postal_code,
              countryCode: loader.country
            }
          );
        });
      });
    });
  }

  createAddressCreateForm(): FormGroup {
    return this.formBuilder.group({
      entityId: [this.entityId],
      type: [this.type, [Validators.required, Validators.maxLength(80)]],
      name: ['', [Validators.maxLength(80)]],
      default: [false],
      addressLine1: ['', [Validators.required, Validators.maxLength(80)]],
      addressLine2: ['', [Validators.maxLength(80)]],
      city: ['', [Validators.maxLength(80)]],
      state: ['', [Validators.maxLength(80)]],
      zipCode: ['', [Validators.maxLength(32)]],
      countryCode: ['', [Validators.maxLength(3)]],
      latitude: [null],
      longitude: [null]
    });
  }

  createAddressUpdateForm(): FormGroup {
    return this.formBuilder.group({
      addressId: [this.address.addressId],
      entityId: [this.address.entityId],
      type: [this.address.type, [Validators.required, Validators.maxLength(80)]],
      name: [this.address.name, [Validators.maxLength(80)]],
      default: [this.address.default],
      addressLine1: [this.address.addressLine1, [Validators.required, Validators.maxLength(80)]],
      addressLine2: [this.address.addressLine2, [Validators.maxLength(80)]],
      city: [this.address.city, [Validators.maxLength(80)]],
      state: [this.address.state, [Validators.maxLength(80)]],
      zipCode: [this.address.zipCode, [Validators.maxLength(32)]],
      countryCode: [this.address.countryCode, [Validators.maxLength(3)]],
      latitude: [this.address.latitude],
      longitude: [this.address.longitude],
    });

  }
}
