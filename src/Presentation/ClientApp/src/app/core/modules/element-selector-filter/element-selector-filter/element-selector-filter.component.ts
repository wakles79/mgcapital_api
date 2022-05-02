import { AfterViewInit, Component, OnInit, OnDestroy, ViewEncapsulation, Input, Output, EventEmitter, Inject } from '@angular/core';
import { AbstractControl, FormBuilder } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GenericFilterBaseModel, GenericFilterType } from '@app/core/models/common/generic-filter-base.model';
import { ListDataSourceRequestModel } from '@app/core/models/common/list-data-source-request.model';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { fuseAnimations } from '@fuse/animations';
import { ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ElementSelectorFilterService } from './element-selector-filter.service';

@Component({
  selector: 'app-element-selector-filter',
  templateUrl: './element-selector-filter.component.html',
  styleUrls: ['./element-selector-filter.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class ElementSelectorFilterComponent implements OnInit, AfterViewInit, OnDestroy {

  @Input() filterData: GenericFilterBaseModel;
  REF = GenericFilterType;

  filterForm: any;
  get searchItems(): AbstractControl { return this.filterForm.get('searchItems'); }
  get searchItemCtrl(): AbstractControl { return this.filterForm.get('searchItemCtrl'); }

  items: ListItemModel[] = [];
  filteredItems$: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);

  // Indicates if display or not options 'All' and 'None',
  // HACK: Improve this implementation, it's not DRY..
  displayOptionAll = false;
  displayOptionNone = false;

  // In case all items are checked, option 'All' was selected
  allItemsChecked = false;

  @Output() selectedValuesChanged: EventEmitter<any> = new EventEmitter();

  /** Subject that emits when the component has been destroyed. Indicates unsubscribe all subscriptions */
  private _onDestroy = new Subject<void>();

  constructor(
    @Inject(ElementSelectorFilterService)
    private elementSelectorFilterService: ElementSelectorFilterService,
    private snackBar: MatSnackBar,
    private _formBuilder: FormBuilder,
  ) {
  }

  ngOnInit(): void {
    console.log('this.filterData', this.filterData);

    this.filterForm = this.createFilterForm();

    this.displayOptionAll = this.filterData.displayOptionAll;
    this.displayOptionNone = this.filterData.displayOptionNone;

    // In case there are giving values (filterData.values), build the selection with them,
    // Else the values will be fetching from API
    if (this.filterData.values && this.filterData.values !== undefined && this.filterData.values.length > 0) {

      this.buildSelectionValuesWithGivingValues();

    } else {
      this.buildSelectionValuesWithGivingURL();
    }

    // In case there are not default values, select or deselect items depending is the option 'All' is added
    if (!this.thereAreDefaultValues()) {
      this.selectValuesDependingOptionAll();
    }

    this.searchItemCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterItems();
      });
  }

  ngAfterViewInit(): void {
    // Cleans selected values
    this.elementSelectorFilterService.onCleanSelectedValues
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {

        this.selectValuesDependingOptionAll();
      });
  }

  // Fetch values from giving values (filterData.values)
  buildSelectionValuesWithGivingValues(): void {
    this.items = this.filterData.values;
    this.filteredItems$.next(this.items);
  }

  // Fecth values from giving URL (filterData.apiURL)
  buildSelectionValuesWithGivingURL(): void {

    // In case there are default values, pass the first value to request and selected it
    // At this moment, request to fetch items only accept one value to garantice it will be included in the response, in the future this could be change

    let firstDefaultValue;
    if (this.thereAreDefaultValues()) {
      firstDefaultValue = this.filterData.defaultValues[0];
    }

    this.getItems('', firstDefaultValue);
  }

  // HACK: Validate when default values is an array with more than one element and build a formArray
  // and changes validation depending of filterData.isRequired
  createFilterForm(): any {

    let defaultValue: any;
    if (this.thereAreDefaultValues) {
      if (this.filterData.type === this.REF.Select) {
        defaultValue = this.filterData.defaultValues[0];
      } else {
        defaultValue = this.filterData.defaultValues;
      }
    }

    return this._formBuilder.group({
      searchItems: [defaultValue],
      searchItemCtrl: ['']
    });
  }

  private filterElements(ctrl: any, selectedElement: number, filterFunc): any {
    // get the search keyword
    const search = (ctrl.value || '').toLowerCase();
    if (search === '' && selectedElement) {
      return;
    }
    // make another request
    filterFunc(search, this.searchItems.value);
  }

  private filterItems(): any {
    const id = this.searchItems.value ? this.searchItems.value.id : null;
    this.filterElements(this.searchItemCtrl, id, this.getItems);
  }

  getItems = (filter = '', selectedValueId = null) => {

    const listDataSourceRequest = new ListDataSourceRequestModel({});
    listDataSourceRequest.filterApiUrl = this.filterData.apiURL;
    listDataSourceRequest.filter = filter;
    listDataSourceRequest.elementId = selectedValueId;

    this.elementSelectorFilterService.getDataToFilter(listDataSourceRequest)
      .pipe(takeUntil(this._onDestroy))
      .subscribe((response: { count: number, payload: ListItemModel[] }) => {
        this.items = response.payload.slice();
        this.filteredItems$.next(response.payload.slice());
      },
        (error) => this.snackBar.open('Oops, there was an error fetching items', 'close', { duration: 1000 }),
        () => {

          if (!this.thereAreDefaultValues()) {
            this.selectValuesDependingOptionAll();
          }

        });
  }

  // Indicates if there are giving default values (filterData.defaultValues)
  thereAreDefaultValues(): boolean {
    if (this.filterData.hasOwnProperty('defaultValues') && Array.isArray(this.filterData.defaultValues) && this.filterData.defaultValues.length > 0) {
      return true;
    }
    return false;
  }

  itemOnChanged(event: any): void {

    this.allItemsChecked = false;
    let selectedValues = null;

    if (this.filterData.type === this.REF.Select) {
      if (event.value === 'All' || event.value === 'None') {
        selectedValues = null;
      }
      else {
        selectedValues = this.searchItems.value;
      }
    }
    else if (this.filterData.type === this.REF.Multiselect) {
      selectedValues = this.getSelectedValues();
    }

    this.selectedValuesChanged.emit({
      filter: this.filterData,
      selectedValues: selectedValues
    });
  }

  allItemsOnChanged(checked: any): void {
    this.toggleItems(checked);

    this.selectedValuesChanged.emit({
      filter: this.filterData,
      selectedValues: this.getSelectedValues()
    });
  }

  // Returned selected values depending if option 'all' is checked
  getSelectedValues(): any {
    if (!this.searchItems.value || this.allItemsChecked === true) {
      return [];
    }
    return this.searchItems.value;
  }

  toggleItems(checked: any): void {
    if (checked) {
      this.searchItems.patchValue([...this.items.map(item => item.id), 0]);
    }
    else {
      this.searchItems.patchValue([]);
    }
  }

  // Set selected values depending if there are an option 'All' added and the filter type ('select' or 'multiselect')
  selectValuesDependingOptionAll(): void {
    if (this.displayOptionAll === true) {
      if (this.filterData.type === this.REF.Select) {
        this.searchItems.setValue('All');
      }
      else {
        this.allItemsChecked = true;
        this.toggleItems(true);
      }
    }
    else {
      this.allItemsChecked = false;
      this.toggleItems(false);
    }
  }

  get nameFirstSelectedValue(): string {
    if (this.searchItems.value) {
      return this.items.find(item => item.id === this.searchItems.value[0]).name;
    }
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

}
