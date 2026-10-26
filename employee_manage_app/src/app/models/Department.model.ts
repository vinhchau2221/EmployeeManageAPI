import { GridOption } from "./GridOption.model";

export class DepartmentModel {
   departmentId: number;
   departmentName: string;
   isActive: boolean;

   constructor(departmentId: number, departmentName: string, isActive: boolean) {
      this.departmentId = departmentId;
      this.departmentName = departmentName;
      this.isActive = isActive;
   }
}

export class DepartmentOpt extends GridOption {
   departmentId: number;

   constructor(departmentId: number, pageSize: number, pageNumber: number, sortDirection: 'asc' | 'desc' = 'asc', sortColumn: string = '', searchText: string = '') {
      super(pageSize, pageNumber, sortDirection, sortColumn, searchText);
      this.departmentId = departmentId;
   }
}