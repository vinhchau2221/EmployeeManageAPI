import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DepartmentModel, DepartmentOpt } from '../models/Department.model';
import { DesignationModel } from '../models/Designation.model';
import { GridOption } from '../models/GridOption.model';

@Service()
export class Master {
   http = inject(HttpClient);
   baseUrl: string = 'https://localhost:7259/api/';

   getAllDepartments() {
      return this.http.get(`${this.baseUrl}DepartmentMaster`);
   }

   saveDepartment(department: DepartmentModel) {
      return this.http.post(`${this.baseUrl}DepartmentMaster`, department);
   }

   updateDepartment(department: DepartmentModel) {
      return this.http.put(`${this.baseUrl}DepartmentMaster/${department.departmentId}`, department);
   }

   deleteDepartment(departmentId: number) {
      return this.http.delete(`${this.baseUrl}DepartmentMaster/${departmentId}`);
   }

   // Add similar methods for Designation if needed
   saveDesignation(designation: DesignationModel) {
      return this.http.post(`${this.baseUrl}DesignationMaster`, designation);
   }

   updateDesignation(designation: DesignationModel) {
      return this.http.put(`${this.baseUrl}DesignationMaster/${designation.designationId}`, designation);
   }

   deleteDesignation(designationId: number) {
      return this.http.delete(`${this.baseUrl}DesignationMaster/${designationId}`);
   }

   getAllDesignations(option?: DepartmentOpt) {
      const params = new URLSearchParams();
      if (option) {
         params.set('page', option.pageNumber.toString());
         params.set('sortBy', option.sortColumn);
         params.set('sortDir', option.sortDirection);
         params.set('search', option.searchText);
         params.set('departmentId', option.departmentId.toString());
      }
      return this.http.get(`${this.baseUrl}DesignationMaster?${params.toString()}`);
   }
}
