import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { interval, merge, Subject } from 'rxjs';
import { startWith, switchMap, takeUntil } from 'rxjs/operators';

import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';
import { AuthService } from '@app/core/services/auth.service';
import { AccessType, ApplicationModule } from '@app/core/models/company-settings/company-settings-base.model';
import { FuseNavigation } from '@fuse/types';


/* Available module id's */
/* dashboard inbox workOrders woList newWorkOrder dailyWoReport billableReport reports inspections calendar buildings proposals contracts */
/* services officetypes  expensestypes users contacts customers emailActivityLog */

/* ID	Name	                          Level
    1	Master	                        10
    2	Office Staff	                  20
    3	Operation Manager	              30
    5	Subcontractor-Operation Manager	35
    4	Supervisor	                    40
    7	Subcontractor-Supervisor	      45
    6	Customer	                      110 */
@Component({
  selector: 'fuse-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FuseNavigationComponent implements OnInit, OnDestroy {
  @Input()
  layout = 'vertical';

  @Input()
  navigation: FuseNavigation[];

  // Private
  private _unsubscribeAll: Subject<any>;

  /**
   *
   * @param {ChangeDetectorRef} _changeDetectorRef
   * @param {FuseNavigationService} _fuseNavigationService
   */
  constructor(
    private _changeDetectorRef: ChangeDetectorRef,
    private _fuseNavigationService: FuseNavigationService,
    private http: HttpClient,
    private authService: AuthService,
  ) {
    // Set the private defaults
    this._unsubscribeAll = new Subject();
  }

  // -----------------------------------------------------------------------------------------------------
  // @ Lifecycle hooks
  // -----------------------------------------------------------------------------------------------------

  /**
   * On init
   */
  ngOnInit(): void {
    // Load the navigation either from the input or from the service
    this.navigation = this.navigation || this._fuseNavigationService.getCurrentNavigation();

    // Subscribe to the current navigation changes
    this._fuseNavigationService.onNavigationChanged
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe(() => {

        // Load the navigation
        this.navigation = this._fuseNavigationService.getCurrentNavigation();

        // Mark for check
        this._changeDetectorRef.markForCheck();
      });

    // Subscribe to navigation item
    merge(
      this._fuseNavigationService.onNavigationItemAdded,
      this._fuseNavigationService.onNavigationItemUpdated,
      this._fuseNavigationService.onNavigationItemRemoved
    ).pipe(takeUntil(this._unsubscribeAll))
      .subscribe(() => {

        // Mark for check
        this._changeDetectorRef.markForCheck();
      });

    // Polling for inbox batch number
    //const inboxUnreadCount$ = this.http.get<number>(`${environment.apiUrl}api/tickets/pending`);
    const inboxUnreadCount$ = this.http.get<number>(document.getElementsByTagName('base')[0].href + `api/tickets/pending`);

    interval(60000)
      .pipe(
        startWith(0),
        switchMap(() => inboxUnreadCount$),
        takeUntil(this._unsubscribeAll)
      )
      .subscribe((count: number) => { this.updateInboxBadge(count); });

    this.updateNavigationAccess();
  }

  ngOnDestroy(): void {
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
  }

  updateNavigationAccess(): void {
    // Checks for access
    this.authService.getModuleAccess()
      .pipe(takeUntil(this._unsubscribeAll))
      .subscribe((result: { module: ApplicationModule, moduleName: string, type: AccessType }[]) => {

        // hack to hide add wo from navigation
        const woAccess = result.find(a => a.module === ApplicationModule.WorkOrder);
        if (woAccess) {
          result.push({ module: ApplicationModule.AddWorkOrder, moduleName: 'Add Work Order', type: woAccess.type });
        }

        this.navigation[0].children.forEach((element, eIndex) => {
          if (element.type === 'group') {

            element.children.forEach((child, cIndex) => {
              const access = result.find(a => ApplicationModule[a.module].toLowerCase() === child.id);
              if (access) {
                if (access.type === AccessType.None) {
                  const item = this.navigation[0].children[eIndex].children[cIndex];
                  item.hidden = true;
                  this._fuseNavigationService.updateNavigationItem(item.id, { hidden: true });
                }
              }
            });

          } else {
            const access = result.find(a => ApplicationModule[a.module].toLowerCase() === element.id);
            if (access) {
              if (access.type === AccessType.None) {
                const item = this.navigation[0].children[eIndex];
                item.hidden = true;
                this._fuseNavigationService.updateNavigationItem(item.id, { hidden: true });
              }
            }
          }

        });
      });
  }

  hidden= false;
  updateInboxBadge(counter: number): void {
    // Updates the inbox nav item
    this._fuseNavigationService.updateNavigationItem('inbox', {
      badge: {
        bg: this.badgeBg(counter),
        fg: this.badgeFg(counter),
        title: counter
      }
    });

    this._fuseNavigationService.updateNavigationItem('inbox2', {
      badge: {
        bg: this.badgeBg(counter),
        fg: this.badgeFg(counter),
        title: counter
      }
    });

    this.hidden = !this.hidden;
    // Update the inbox menu item
    this._fuseNavigationService.updateNavigationItem('inbox', {
        hidden: this.hidden
    });

    // Update the inbox2 menu item
    this._fuseNavigationService.updateNavigationItem('inbox2', {
      hidden: !this.hidden
    });
  }

  badgeBg(qtty: number): string {
    if (qtty > 0) {
      return '#F44336';
    }

    return '#2D323E';
  }

  badgeFg(qtty: number): string {
    if (qtty > 0) {
      return '#FFFFFF';
    }

    return '#2D323E';
  }
}
