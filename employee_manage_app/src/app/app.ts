import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoadingComponent } from './components/loading/loading';
import { ToasterContainerComponent } from './components/toast/toast-container';

@Component({
  imports: [RouterOutlet, LoadingComponent, ToasterContainerComponent],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('employee_manage_app');
}
