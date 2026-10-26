// loading.component.ts
import { Component } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Loading } from '../../services/loading/loading';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [AsyncPipe], // Use AsyncPipe instead of CommonModule to reduce bundle size.
  templateUrl: './loading.html',
  styleUrl: './loading.css',
})
export class LoadingComponent {
  constructor(public loading: Loading) { }
}
