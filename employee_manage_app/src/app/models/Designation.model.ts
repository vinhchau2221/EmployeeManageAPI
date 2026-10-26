export class DesignationModel {
   designationId: number;
   designationName: string;
   departmentId: number;
   isActive: boolean;

   constructor(designationId: number, designationName: string, departmentId: number, isActive: boolean = true) {
      this.designationId = designationId;
      this.designationName = designationName;
      this.departmentId = departmentId;
      this.isActive = isActive;
   }
}