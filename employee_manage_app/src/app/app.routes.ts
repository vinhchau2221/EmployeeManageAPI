import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Header } from './pages/header/header';
import { Dashboard } from './pages/dashboard/dashboard';
import { EmployeeForm } from './pages/employee-form/employee-form';
import { EmployeeList } from './pages/employee-list/employee-list';
import { Department } from './pages/department/department';
import { Designation } from './pages/designation/designation';

export const routes: Routes = [
   {
      path: '',
      redirectTo: 'login',
      pathMatch: 'full'
   },
   {
      path: 'login',
      component: Login
   },
   {
      path: '',
      component: Header,
      children: [
         {
            path: 'dashboard',
            component: Dashboard
         },
         {
            path: 'employee-form',
            component: EmployeeForm
         },
         {
            path: 'employee-list',
            component: EmployeeList
         },
         {
            path: 'department',
            component: Department
         },
         {
            path: 'designation',
            component: Designation
         }
      ]
   }
];
