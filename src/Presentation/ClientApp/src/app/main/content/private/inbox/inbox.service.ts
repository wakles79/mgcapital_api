import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InboxService {

  convertionComponentToDisplay: any;
  onComponentChanged: Subject<any> = new Subject();

  constructor() {
    this.onComponentChanged.subscribe((dynamicComponentToLoad: any) => {
      // Do something else to set the child component to display for convertion
    });
  }
}
