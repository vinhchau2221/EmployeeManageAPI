import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  imports: [FormsModule],
  selector: 'app-login',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class Login {
  loginObj: any = {
    email: '',
    contactNo: ''
  };

  http = inject(HttpClient);
  router = inject(Router);

  onLogin() {
    debugger;
    this.http.post('https://localhost:7259/api/Auth/login', this.loginObj).subscribe(
      (response: any) => {
        console.log('Login successful:', response);
        // Handle successful login, e.g., navigate to dashboard
        localStorage.setItem('token', response.token); // Store the token in local storage
        localStorage.setItem('user', JSON.stringify(response)); // Store user info in local storage
        this.router.navigateByUrl('dashboard');
      },
      (error: any) => {
        console.error('Login failed:', error);
        // Handle login error, e.g., show error message
        alert('Login failed. Please check your credentials and try again.');
      }
    );
  }
}
