import { Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ListItemModel } from '@app/core/models/common/list-item.model';
import { TicketBaseModel, TicketDestinationType, TicketSource } from '@app/core/models/ticket/ticket-base.model';
import { fuseAnimations } from '@fuse/animations';
import { Subject, Subscription } from 'rxjs';
import { TicketsService } from '../../tickets.service';
import { BuildingsService } from '@app/main/content/private/buildings/buildings.service';
import { TagsService } from '../../../tags/tags.service';
import { TicketFormDialogComponent } from '@app/core/modules/ticket-form/ticket-form/ticket-form.component';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { ListBuildingModel } from '@app/core/models/building/list-buildings.model';
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-inbox-main-sidenav',
  templateUrl: './inbox-main.component.html',
  styleUrls: ['./inbox-main.component.scss'],
  animations: fuseAnimations,
  encapsulation: ViewEncapsulation.None
})
export class InboxMainComponent implements OnInit, OnDestroy {

  @Input() readOnly: boolean;

  private _unsubscribeAll: Subject<any>;

  folders: any[];
  filters: any[];
  labels: any[];
  dialogRef: any;

  public REF = {
    TicketSource: TicketSource,
    TicketDestinationType: TicketDestinationType,
  };

  sourcesToFilter: { id: number; name: string }[] = [];
  filterSources = new FormControl();
  selectedSourcesToFilter = '';

  destinationsToFilter: { id: number; name: string }[] = [];
  filterDestinations = new FormControl();
  selectedDestinationsToFilter = '';

  onSelectedForlderChanged: Subscription;

  filterBy: { [key: string]: string } = {};

  // building
  buildings: { id: number, name: string, fullAddress: string }[] = [];
  filteredBuildings: { id: number, name: string, fullAddress: string }[] = [];
  buildingFormControl: FormControl;
  buildingSearchFormControl: FormControl;

  tags: ListItemModel[] = [];
  selectedTags: number[] = [];
  selectedTagsFormControl: FormControl;

  constructor(
    public dialog: MatDialog,
    private ticketService: TicketsService,
    public snackBar: MatSnackBar,
    private _buildingService: BuildingsService,
    private _tagService: TagsService,
  ) {
    this._unsubscribeAll = new Subject();

    this.folders = [
      {
        title: 'Open',
        color: 'red',
        icon: 'assignment',
        handle: 'pending'
      },
      {
        title: 'Closed',
        color: 'red',
        icon: 'assignment_turned_in',
        handle: 'resolved'
      },
      {
        title: 'Snoozed',
        color: 'red',
        icon: 'access_time',
        handle: 'snoozed'
      },
      {
        title: 'Deleted',
        color: 'red',
        icon: 'delete',
        handle: 'delete'
      }
    ];

    this.filterBy = this.ticketService.filterBy;

    this.buildingFormControl = new FormControl('0');
    this.buildingSearchFormControl = new FormControl('');
    this.selectedTagsFormControl = new FormControl([]);

    this.loadFilters();
  }

  ngOnInit(): void {
    this.getSourcesToFilter();
    this.getDestinationsToFilter();
    this.getBuildings();
    this.getTags();

    this.onSelectedForlderChanged = this.ticketService.onSelectedForlderChanged.subscribe(() => {
      // unselect applied filters
      this.selectedSourcesToFilter = undefined;
      this.selectedDestinationsToFilter = undefined;
    });

    this.buildingFormControl.valueChanges
      .subscribe(value => {
        this.filterBy['BuildingId'] = value === '0' ? null : value;
        this.ticketService.saveFiltersLocally(this.filterBy);
        this.ticketService.filterTickets(this.filterBy);

        this.ticketService.filterBy = this.filterBy;
      });

    this.buildingSearchFormControl.valueChanges
      .subscribe(value => {
        this.filterBuildings();
      });
  }

  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  addTicket(): void {
    this.dialogRef = this.dialog.open(TicketFormDialogComponent, {
      panelClass: 'ticket-form-dialog',
      data: {
        action: 'new'
      }
    });
    this.dialogRef.afterClosed()
      .subscribe((formData: FormGroup) => {
        if (!formData) {
          return;
        }
        const ticketToCreate = new TicketBaseModel(formData.getRawValue());

        this.ticketService.createElement(ticketToCreate)
          .then(
            () => this.snackBar.open('Ticket created successfully!!!', 'close', { duration: 1000 }),
            () => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }))
          .catch(() => this.snackBar.open('Oops, there was an error', 'close', { duration: 1000 }));
      });
  }

  getSourcesToFilter(): void {
    for (const source in TicketSource) {
      if (typeof TicketSource[source] === 'number') {
        this.sourcesToFilter.push({ id: TicketSource[source] as any, name: source });
      }
    }
    const indexToDelete = this.sourcesToFilter.indexOf(this.sourcesToFilter.find(source => source.name === 'undefined'));
    this.sourcesToFilter.splice(indexToDelete, 1);

    this.sourcesToFilter.sort((a, b) => {
      if (a.name < b.name) { return -1; }
      if (a.name > b.name) { return 1; }
      return 0;
    });
  }

  get selectedSourceById(): any {
    return this.sourcesToFilter.find(source => source.id === this.filterSources.value[0]);
  }

  onSourcesChanged($event): void {
    let selectedSourcesFlag = $event.value;
    if ($event.value[0]) {
      selectedSourcesFlag = selectedSourcesFlag.reduce((x: number, y: number) => x | y, 0);
      this.filterBy['source'] = selectedSourcesFlag;
      this.ticketService.filterTickets(this.filterBy);
    }
  }

  getDestinationsToFilter(): void {
    for (const destination in TicketDestinationType) {
      if (typeof TicketDestinationType[destination] === 'number') {
        this.destinationsToFilter.push({ id: TicketDestinationType[destination] as any, name: destination });
      }
    }
    const indexToDelete = this.destinationsToFilter.indexOf(this.destinationsToFilter.find(destination => destination.name === 'undefined'));
    this.destinationsToFilter.splice(indexToDelete, 1);
  }

  get selectedDestinationById(): any {
    return this.destinationsToFilter.find(destination => destination.id === this.filterDestinations.value[0]);
  }

  onDestinationsChanged($event): void {

    let selectedDestinationFlag = $event.value;

    if ($event.value[0]) {

      selectedDestinationFlag = selectedDestinationFlag.reduce((x: number, y: number) => x | y, 0);
      this.filterBy['destination'] = selectedDestinationFlag;
      this.ticketService.filterTickets(this.filterBy);
    }
  }

  onlyAssignedChanged(event: MatCheckboxChange): void {
    this.filterBy['OnlyAssigned'] = event.checked + '';
    this.ticketService.filterTickets(this.filterBy);
  }

  // buildings
  getBuildings(): void {
    this._buildingService.getAllAsList('readallcbo', '', 0, 200, null, {})
      .subscribe((response: { count: number, payload: ListBuildingModel[] }) => {
        this.buildings = response.payload;
        this.filteredBuildings = response.payload;
      },
        (error) => this.snackBar.open('Oops, there was an error fetching buildings', 'close', { duration: 1000 }));
  }

  private filterBuildings(): void {
    if (!this.buildings) {
      return;
    }
    // get the search keyword
    let search = this.buildingSearchFormControl.value;
    if (!search) {
      this.filteredBuildings = this.buildings.slice();
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the buildings
    this.filteredBuildings =
      this.buildings.filter(building => (building.name.toLowerCase() + building.fullAddress.toLowerCase()).indexOf(search) > -1)
      ;
  }

  // Tags
  getTags(): void {
    this.tags = [];
    this._tagService.getTagsAsList()
      .subscribe((result: ListItemModel[]) => {
        this.tags = result;
      }, (error) => {

      });
  }

  tagsChanged(event: MatSelectChange): void {
    const selected: number[] = event.value;
    const strSelected = selected.join(',');
    this.filterBy['StrTags'] = strSelected;
    this.ticketService.saveFiltersLocally(this.filterBy);
    this.ticketService.filterTickets(this.filterBy);

    this.ticketService.filterBy = this.filterBy;
  }

  // Default
  loadFilters(): void {
    if (Object.keys(this.filterBy).length > 0) {

      if (this.filterBy['BuildingId']) {
        this.buildingFormControl.setValue(this.filterBy['BuildingId']);
      }

      if (this.filterBy['StrTags']) {
        const tags = this.filterBy['StrTags'].split(',');
        if (tags.length > 0) {
          tags.forEach(t => {
            this.selectedTags.push(Number(t));
          });
          // this.selectedTagsFormControl.setValue(this.selectedTags);
        }
      }
    }
  }
}
