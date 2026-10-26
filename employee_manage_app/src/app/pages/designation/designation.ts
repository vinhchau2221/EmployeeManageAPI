import { Component , effect, inject, OnInit, signal } from '@angular/core';
import { DesignationModel } from '../../models/Designation.model';
import { Master } from '../../services/master';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../services/toast/toast';
import { DepartmentModel, DepartmentOpt } from '../../models/Department.model';
import { GridOption } from '../../models/GridOption.model';
import { Department } from '../department/department';
// import { toObservable } from '@angular/core/rxjs-interop';
// import { filter, take } from 'rxjs/operators';

@Component({
  imports: [FormsModule],
  selector: 'app-designation',
  styleUrl: './designation.css',
  templateUrl: './designation.html',
})
export class Designation implements OnInit {
  private toastService = inject(ToastService);

  newDesignationObj: DesignationModel = new DesignationModel(0, '', 0, true);
  masterService = inject(Master);
  designations = signal<DesignationModel[]>([]);
  departments = signal<DepartmentModel[]>([]);
  curDepartment = signal<DepartmentModel | null>(null);
  gridOption: DepartmentOpt = new DepartmentOpt(0, 10, 1, 'asc', 'designationName', '');

  constructor() {
    // effect will automatically rerun whenever this.departments() receives new data
    effect(() => {
      const depts = this.departments();
      if (depts && depts.length > 0 && !this.curDepartment()) {
        this.curDepartment.set(depts[0]);
      }
    });

    effect(() => {
      const selectedDept = this.curDepartment();

      if (selectedDept) {
        // 1. Synchronize the search text on the grid
        this.gridOption.departmentId = selectedDept.departmentId;

        // 2. Load the list of designations for the newly selected department ID
        this.getAllDesignations(this.gridOption);
      }
    });
  }

  ngOnInit() {
    this.getAllDepartments();

    // // Convert Signal to Observable so RxJS operators can be applied
    // toObservable(this.departments).pipe(
    //   filter(depts => depts && depts.length > 0), // Only continue when the array contains elements
    //   take(1) // Only take the first matching value, then unsubscribe automatically
    // ).subscribe(depts => {
    //   this.curDepartment = depts[0];
    //   this.gridOption.searchText = this.curDepartment.departmentName;
    // });

    // get all designations for the current department
    //this.getAllDesignations(this.gridOption);
  }

  onSaveDesignation() {
    if (this.newDesignationObj.designationId === 0) { // insert new designation
      this.masterService.saveDesignation(this.newDesignationObj).subscribe({
        next: (response: any) => {
          console.log('Designation saved successfully:', response);
          this.toastService.success('Designation saved successfully!');
          this.getAllDesignations(this.gridOption); // Refresh the list after saving
          this.onResetForm(); // Reset the form after saving
        },
        error: (error: any) => {
          console.error('Error saving designation:', error);
          this.toastService.error('Error saving designation!');
        }
      });
    }
    else {
      this.masterService.updateDesignation(this.newDesignationObj).subscribe(
        {
          next: (response: any) => {
            console.log('Designation updated successfully:', response);
            this.toastService.success('Designation updated successfully!');
            this.getAllDesignations(this.gridOption); // Refresh the list after updating
          },
          error: (error: any) => {
            console.error('Error updating designation:', error);
            this.toastService.error('Error updating designation!');
          }
        }
      );
    }
  }

  onEditDesignation(designation: DesignationModel) {
    this.newDesignationObj = { ...designation }; // Create a copy of the designation object
  }

  onDeleteDesignation(designationId: number) {
    const confirmed = confirm('Are you sure you want to delete this designation?');
    if (!confirmed) {
      return; // Exit if the user cancels the deletion
    }

    this.masterService.deleteDesignation(designationId).subscribe(
      {
        next: (response: any) => {
          console.log('Designation deleted successfully:', response);
          this.toastService.success('Designation deleted successfully!');
          this.getAllDesignations(this.gridOption); // Refresh the list after deleting
        },
        error: (error: any) => {
          console.error('Error deleting designation:', error);
          this.toastService.error('Error deleting designation!');
        }
      }
    );
  }

  onResetForm() {
    this.newDesignationObj = new DesignationModel(0, '', this.curDepartment()?.departmentId || 0, true); // Reset to default values
  }

  getAllDesignations(option?: DepartmentOpt) {
    this.masterService.getAllDesignations(option).subscribe(
      {
        next: (response: any) => {
          this.designations.set(response.items);
        },
        error: (error: any) => {
          console.error('Error fetching designations:', error);
          this.toastService.error('Error fetching designations!');
        }
      }
    );
  }

  getAllDepartments() {
    this.masterService.getAllDepartments().subscribe(
      {
        next: (response: any) => {
          this.departments.set(response);
        },
        error: (error: any) => {
          this.toastService.error('Error fetching departments!');
        }
      }
    );
  }

  onDepartmentChange() {
    const selectedDept = this.curDepartment();
    if (selectedDept) {
      this.gridOption.departmentId = selectedDept.departmentId;
      this.newDesignationObj.departmentId = selectedDept.departmentId;
      this.getAllDesignations(this.gridOption);
    }
  }
}