import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, NgModule } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';


@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule,FormsModule,RouterModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {
  constructor(private router: Router, private http:HttpClient) {}

  onSubmit(form: NgForm) {
    if (form.valid && form.value.password === form.value.confirmPassword) {
      // Handle successful registration (e.g., call API, show message)
      // var BaseUrl = "https://localhost:7188/api/User/user-registration";
var BaseUrl =
 "https://localhost:7188/api/User/get-all-users";


      var apiData :any;
      apiData = this.http.get(BaseUrl);
      console.log("api response:  ",apiData);


      alert('Registration successful!');
      this.router.navigate(['/login']); // Redirect to login
    } else {
      // Optionally handle errors
      if (form.value.password !== form.value.confirmPassword) {
        alert('Passwords do not match!');
      }
    }
  }
}
