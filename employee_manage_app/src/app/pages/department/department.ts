import { Component, inject, OnInit, signal } from '@angular/core';
import { DepartmentModel } from '../../models/Department.model';
import { Master } from '../../services/master';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../services/toast/toast';

@Component({
  imports: [FormsModule],
  selector: 'app-department',
  styleUrl: './department.css',
  templateUrl: './department.html',
})
export class Department implements OnInit {
  private toastService = inject(ToastService);

  newDeptObj: DepartmentModel = new DepartmentModel(0, '', true);
  masterService = inject(Master);
  departments = signal<DepartmentModel[]>([]);

  ngOnInit() {
    this.getAllDepartments();
  }

  onSaveDepartment() {
    if (this.newDeptObj.departmentId === 0) { // insert new department
      this.masterService.saveDepartment(this.newDeptObj).subscribe({
        next: (response: any) => {
          console.log('Department saved successfully:', response);
          this.toastService.success('Department saved successfully!');
          this.getAllDepartments(); // Refresh the list after saving
        },
        error: (error: any) => {
          console.error('Error saving department:', error);
          this.toastService.error('Error saving department!');
        }
      });
    }
    else {
      this.masterService.updateDepartment(this.newDeptObj).subscribe(
        {
          next: (response: any) => {
            console.log('Department updated successfully:', response);
            this.toastService.success('Department updated successfully!');
            this.getAllDepartments(); // Refresh the list after updating
          },
          error: (error: any) => {
            console.error('Error updating department:', error);
            this.toastService.error('Error updating department!');
          }
        }
      );
    }
  }

  onEditDepartment(department: DepartmentModel) {
    this.newDeptObj = { ...department }; // Create a copy of the department object
  }

  onDeleteDepartment(departmentId: number) {
    const confirmed = confirm('Are you sure you want to delete this department?');
    if (!confirmed) {
      return;
    }

    this.masterService.deleteDepartment(departmentId).subscribe(
      {
        next: (response: any) => {
          console.log('Department deleted successfully:', response);
          this.toastService.success('Department deleted successfully!');
          this.getAllDepartments(); // Refresh the list after deleting
        },
        error: (error: any) => {
          console.error('Error deleting department:', error);
          this.toastService.error('Error deleting department!');
        }
      }
    );
  }

  onResetForm() {
    this.newDeptObj = new DepartmentModel(0, '', true); // Reset to default values
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
}
