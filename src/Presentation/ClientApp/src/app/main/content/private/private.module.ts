import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FuseSharedModule } from '@fuse/shared.module';
import { FeatureFlagGuardService } from '@app/core/services/feature-flag/feature-flag-guard.service';

const routes = [
  {
    path: 'home',
    loadChildren: () => import('./home/home.module').then(m => m.HomeModule)
  },
  {
    path: 'tags',
    loadChildren: () => import('./tags/tags.module').then(m => m.TagsModule)
  },
  {
    path: 'work-order-services',
    loadChildren: () => import('./work-order-services/work-order-services.module').then(m => m.WorkOrderServicesModule)
  },
  {
    path: 'schedule-settings-category',
    loadChildren: () => import('./schedule-settings-category/schedule-settings-category.module').then(m => m.ScheduleSettingsCategoryModule)
  },
  {
    path: 'roles',
    loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule)
  },
  {
    path: 'users',
    loadChildren: () => import('./users/users.module').then(m => m.UsersModule)
  },
  {
    path: 'buildings',
    loadChildren: () => import('./buildings/buildings.module').then(m => m.BuildingsModule)
  },
  {
    path: 'work-orders',
    loadChildren: () => import('./work-orders/work-orders.module').then(m => m.WorkOrdersModule)
  },
  {
    path: 'reports',
    loadChildren: () => import('./reports/reports.module').then(m => m.ReportsModule)
  },
  {
    path: 'inspections',
    loadChildren: () => import('./inspections/inspections.module').then(m => m.InspectionsModule)
  },
  {
    path: 'calendar',
    loadChildren: () => import('./wo-calendar/wo-calendar.module').then(m => m.WoCalendarModule)
  },
  {
    path: 'expenses-types',
    loadChildren: () => import('./expenses-types/expenses-types.module').then(m => m.ExpensesTypesModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'expenses-type-catalog' }
  },
  {
    path: 'contacts',
    loadChildren: () => import('./contacts/contacts.module').then(m => m.ContactsModule)
  },
  // {
  //   path: 'vendors',
  //   loadChildren: () => import('./vendors/vendors.module').then(m => m.VendorsModule)
  // },
  {
    path: 'customers',
    loadChildren: () => import('./customers/customers.module').then(m => m.CustomersModule)
  },
  {
    path: 'departments',
    loadChildren: () => import('./departments/departments.module').then(m => m.DepartmentsModule)
  },
  // {
  //   path: 'contracts',
  //   loadChildren: () => import('./contracts/contracts.module').then(m => m.ContractsModule)
  // },
  {
    path: 'budgets',
    loadChildren: () => import('./contracts/contracts.module').then(m => m.ContractsModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'contract-service-catalog' }
  },
  {
    path: 'services',
    loadChildren: () => import('./services/services.module').then(m => m.ServicesModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'services-catalog-module' }
  },
  // {
  //   path: 'reports',
  //   loadChildren: () => import('./reports/reports.module').then(m => m.ReportsModule)
  // },
  {
    path: 'inbox',
    loadChildren: () => import('./inbox/inbox.module').then(m => m.InboxModule)
  },
  {
    path: 'office-types',
    loadChildren: () => import('./office-types/office-types.module').then(m => m.OfficeTypesModule)
  },
  {
    path: 'proposals',
    loadChildren: () => import('./proposals/proposals.module').then(m => m.ProposalsModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'proposals-module' }
  },
  {
    path: 'buildings-profile',
    loadChildren: () => import('./buildings-profile/buildings-profile.module').then(m => m.BuildingsProfileModule)
  },
  {
    path: 'email-activity-log',
    loadChildren: () => import('./email-activity-log/email-activity-log.module').then(m => m.EmailActivityLogModule)
  },
  // {
  //   path: 'inspections',
  //   loadChildren: () => import('./inspections/inspections.module').then(m => m.InspectionsModule)
  // },
  {
    path: 'revenues',
    loadChildren: () => import('./revenues/revenues.module').then(m => m.RevenuesModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'revenues-module' }
  },
  {
    path: 'expenses',
    loadChildren: () => import('./expenses/expenses.module').then(m => m.ExpensesModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'expenses-module' }
  },
  {
    path: 'budget-settings',
    loadChildren: () => import('./settings-budget/settings-budget.module').then(m => m.SettingsBudgetModule),
    canActivate: [FeatureFlagGuardService],
    data: { featureFlag: 'budgets-module' }
  },
  {
    path: 'settings',
    loadChildren: () => import('./settings/settings.module').then(m => m.SettingsModule)
  },
];

@NgModule({
  imports: [
    FuseSharedModule,
    RouterModule.forChild(routes),
  ],
  declarations: [

  ]
})
export class FusePrivateModule {
}
