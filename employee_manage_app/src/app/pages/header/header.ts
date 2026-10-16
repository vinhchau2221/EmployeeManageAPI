import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

interface NavItem {
  label: string;
  icon: string;
  route?: string;
  children?: NavItem[];
}

@Component({
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  selector: 'app-header',
  styleUrl: './header.css',
  templateUrl: './header.html',
})
export class Header {
  isCollapsed = false;
  isMobileOpen = false;
  reportsExpanded = false;

  navItems: NavItem[] = [
    {
      label: 'Dashboard',
      icon: 'fa-solid fa-gauge-high',
      route: '/dashboard'
    },
    {
      label: 'New employee',
      icon: 'fa-solid fa-user-plus',
      route: '/employee-form'
    },
    {
      label: 'Employees',
      icon: 'fa-solid fa-users',
      route: '/employee-list'
    },
    {
      label: 'Departments',
      icon: 'fa-solid fa-building',
      route: '/department'
    },
    {
      label: 'Designations',
      icon: 'fa-solid fa-cart-shopping',
      route: '/designation'
    }
  ];

  reportItems: NavItem[] = [
    {
      label: 'Sales Reports',
      icon: 'fa-solid fa-chart-column',
      route: '/reports/sales'
    },
    {
      label: 'User Reports',
      icon: 'fa-solid fa-user-group',
      route: '/reports/users'
    }
  ];

  toggleSidebar(): void {
    this.isCollapsed = !this.isCollapsed;
  }

  toggleMobileSidebar(): void {
    this.isMobileOpen = !this.isMobileOpen;
  }

  closeMobileSidebar(): void {
    this.isMobileOpen = false;
  }

  toggleReports(): void {
    this.reportsExpanded = !this.reportsExpanded;
  }
}
