import { Component, OnInit, OnDestroy } from '@angular/core';
import { DepartmentsService } from '../../departments.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: string } = {};

  constructor(private departmentsService: DepartmentsService) {
    this.filterBy = this.departmentsService.filterBy || {};
    this.filterActive = 'all';
  }

  ngOnInit(): void { }

  // Refactor code if filter is a dictionary (key: value)
  changeFilter(filter): void {
    this.filterActive = filter;
    this.filterBy = filter;
    this.departmentsService.onFilterChanged.next(this.filterBy);
  }

  ngOnDestroy(): void {
  }

}
