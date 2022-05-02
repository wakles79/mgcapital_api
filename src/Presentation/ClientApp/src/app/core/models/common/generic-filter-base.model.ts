
export class GenericFilterBaseModel {

  ///  Filter unique identifier (e.g categoryId)
  identifier: string;

  ///  Name to be shown (e.g Category)
  displayName: string;

  type: GenericFilterType;

  ///  URL to fetch elements in case of a "select" widget
  apiURL: string;

  ///  Order to sort filter widgets in interface
  order: number;

  // Values to select by default, the values must be the Ids
  defaultValues: any[];

  // Values to make the selection, values will not be fetching from API
  // In this case, the values must be a ListItemModel[] or an array of objects with id and name
  values: any[];

  displayOptionAll: boolean;

  displayOptionNone: boolean;

  // Indicates if the selected value is required
  isRequired: boolean;

  constructor(filter) {
    this.identifier = filter.identifier || '';
    this.displayName = filter.displayName || '';
    this.type = filter.type || GenericFilterType.Select;
    this.apiURL = filter.apiURL || '';
    this.defaultValues = filter.defaultValues || [];
    this.values = filter.values || [];
    this.displayOptionAll = filter.hasOwnProperty('displayOptionAll') ? filter.displayOptionAll : false;
    this.displayOptionNone = filter.hasOwnProperty('displayOptionNone') ? filter.displayOptionNone : false;
    this.isRequired = filter.hasOwnProperty('isRequired') ? filter.isRequired : true;
  }
}

export enum GenericFilterType {

  /// Widget is a "Select Box"
  Select = 0,

  /// Widget is a "DatePicker" with range
  DateRange = 1,

  /// Widget is a 'true/false' check
  Toggle = 2,

  /// Widget is a simple input
  Value = 3,

  /// Internal filter (won't show up in client)
  Application = 4,

  /// Widget is a "DatePicker"
  Date = 5,

  /// Widget is a multiselect
  Multiselect = 6,
}
