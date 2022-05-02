import { FuseNavigation } from '@fuse/types';

export const navigation: FuseNavigation[] = [
  {
    id: 'applications',
    title: 'MG Capital Maintenance',
    type: 'group',
    icon: 'apps',
    children: [
      {
        id: 'dashboard',
        title: 'Dashboard',
        type: 'item',
        icon: 'dashboard',
        url: '/app/home',
      },
      {
        id: 'inbox-module',
        title: 'Inbox',
        type: 'group',
        children: [
          {
            id: 'inbox',
            title: 'Inbox',
            exactMatch: true,
            type: 'item',
            icon: 'toc',
            url: 'app/inbox'
          },
          {
            id: 'inbox2',
            title: 'Inbox',
            exactMatch: true,
            type: 'item',
            icon: 'toc',
            url: 'app/inbox'
          },
          {
            id: 'tags',
            title: 'Tags',
            type: 'item',
            icon: 'bookmarks',
            url: 'app/tags'
          },
        ]
      },
      {
        id: 'workOrders',
        title: 'Work Orders',
        type: 'group',
        children: [
          {
            id: 'workorder',
            title: 'Work Orders List',
            type: 'item',
            exactMatch: true,
            icon: 'toc',
            url: 'app/work-orders'
          },
          {
            id: 'addworkorder',
            title: 'New Work Order',
            type: 'item',
            exactMatch: true,
            icon: 'note_add',
            queryParams: {
              action: 'new'
            },
            url: 'app/work-orders/detail'
          },
          {
            id: 'dailyreport',
            title: 'Daily Report',
            exactMatch: true,
            type: 'item',
            icon: 'toc',
            url: 'app/work-orders/daily-report-operations-manager'
          },
          {
            id: 'billablereport',
            title: 'Billable Report',
            exactMatch: true,
            type: 'item',
            icon: 'toc',
            url: 'app/work-orders/billable-report'
          }
        ]
      },
      {
        id: 'reports',
        title: 'Reports',
        type: 'group',
        children: [
          {
            id: 'cleaningreport',
            title: 'Cleaning Report',
            exactMatch: true,
            type: 'item',
            icon: 'toc',
            url: 'app/reports/cleaning-report',
            // 'badge': {
            //     'title': this.cleaningReportsUnread,
            //     'bg': this.badgeBg(this.cleaningReportsUnread),
            //     'fg': this.badgeFg(this.cleaningReportsUnread)
            // }
          }
        ]
      },
      {
        id: 'navinspections',
        title: 'Inspections',
        type: 'group',
        children: [
          {
            id: 'inspections',
            title: 'Inspections List',
            type: 'item',
            icon: 'toc',
            url: '/app/inspections'
          }
        ]
      },
      {
        id: 'navcalendar',
        title: 'Calendar',
        type: 'group',
        children: [
          {
            id: 'calendar',
            title: 'Calendar',
            type: 'item',
            icon: 'calendar_today',
            url: '/app/calendar'
          }
          // {
          //   id: 'scheduled',
          //   title: 'Scheduled',
          //   type: 'item',
          //   icon: 'timer',
          //   url: '/app/pre-calendar'
          // }
        ]
      },
      {
        id: 'navbuildings',
        title: 'Buildings',
        type: 'group',
        children: [
          {
            id: 'buildings',
            title: 'Buildings List',
            type: 'item',
            icon: 'location_city',
            url: 'app/buildings'
          },
          // {
          //   id: 'buildings-profile',
          //   title: 'Building profile',
          //   type: 'item',
          //   icon: 'receipt',
          //   url: '/app/buildings-profile/buildings-profile'
          // }
        ]
      },
      {
        id: 'navproposals',
        title: 'Proposals',
        type: 'group',
        featureFlag: 'proposals-module',
        children: [
          {
            id: 'proposals',
            title: 'Proposal List',
            exactMatch: true,
            type: 'item',
            icon: 'receipt',
            url: '/app/proposals/proposals',
            featureFlag: 'proposals-list',
          }
        ]
      },
      {
        id: 'navcontracts',
        title: 'Budgets',
        type: 'group',
        featureFlag: 'budgets-module',
        children: [
          {
            id: 'budgets',
            title: 'Budgets List',
            exactMatch: true,
            type: 'item',
            icon: 'receipt',
            url: '/app/budgets/budgets',
            featureFlag: 'budgets-list',
          },
          {
            id: 'revenues',
            title: 'Revenues',
            exactMatch: true,
            type: 'item',
            icon: 'receipt',
            url: '/app/revenues',
            featureFlag: 'revenues-module',
          },
          {
            id: 'expenses',
            title: 'Expenses',
            exactMatch: true,
            type: 'item',
            icon: 'receipt',
            url: '/app/expenses',
            featureFlag: 'expenses-module',
          }
        ]
      },
      {
        id: 'settings',
        title: 'Settings',
        type: 'group',
        children: [
          {
            id: 'companysettings',
            title: 'Company Settings',
            type: 'item',
            icon: 'settings',
            url: 'app/settings'
          },
          // {
          //   id: 'budgetsettings',
          //   title: 'Budget Settings',
          //   type: 'item',
          //   icon: 'settings',
          //   url: 'app/budget-settings'
          // },
          {
            id: 'roles',
            title: 'Roles',
            type: 'item',
            icon: 'group',
            url: 'app/roles'
          },
          {
            id: 'services',
            title: 'Services Catalog',
            type: 'item',
            icon: 'list',
            url: 'app/services',
            featureFlag: 'services-catalog-module',
          },
          {
            id: 'workorderservicescatalog',
            title: 'Work Order Services Catalog',
            type: 'item',
            icon: 'list',
            url: 'app/work-order-services'
          },
          {
            id: 'contractservicescatalog',
            title: 'Contract Services Catalog',
            type: 'item',
            icon: 'list',
            url: '/app/office-types'
          },
          {
            id: 'expensestypescatalog',
            title: 'Expenses Types Catalog',
            type: 'item',
            icon: 'list',
            url: '/app/expenses-types',
            featureFlag: 'expenses-type-catalog'
          },
          {
            id: 'scheduledworkordercategories',
            title: 'Schedule Settings Category',
            type: 'item',
            icon: 'list',
            url: '/app/schedule-settings-category'
          },
          {
            id: 'users',
            title: 'Users',
            type: 'item',
            icon: 'group',
            url: '/app/users',
          },
          {
            id: 'contacts',
            title: 'All Contacts',
            type: 'item',
            icon: 'contacts',
            url: '/app/contacts'
          },
          {
            id: 'managementco',
            title: 'Management Co.',
            type: 'item',
            icon: 'group',
            url: '/app/customers'
          },
          // {
          //   id: 'todo',
          //   title: 'To Do',
          //   type: 'item',
          //   icon: 'toc',
          //   url: '/maintenance'
          // },
          // {
          //   id: 'departments',
          //   title: 'Departments',
          //   type: 'item',
          //   icon: 'assignment_ind',
          //   url: '/app/departments'
          // },
          {
            id: 'emailactivity',
            title: 'Email Activity',
            type: 'item',
            icon: 'dashboard',
            url: '/app/email-activity-log'
          }
        ]
      }
    ]
  }
];
