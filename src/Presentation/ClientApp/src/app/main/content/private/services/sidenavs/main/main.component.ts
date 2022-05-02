import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '@app/core/services/auth.service';
import { Subject } from 'rxjs';
import { ServicesService } from '../../services.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit, OnDestroy {

  filterActive: string;
  filterBy: { [key: string]: string } = {};

  /** Subject that emits when the component has been destroyed. */
  private _onDestroy = new Subject<void>();

  constructor(
    private serviceService: ServicesService,
    private authService: AuthService,
    public snackBar: MatSnackBar,
  ) {
    this.filterActive = 'all';
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this._onDestroy.next();
    this._onDestroy.complete();
  }

  changeFilter(filter: any): void {
    if (filter === 'all') {
      this.filterActive = 'all';
    }
  }

}
