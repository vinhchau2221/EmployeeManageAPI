import { Component, inject } from '@angular/core';
import { ToastService } from '../../services/toast/toast';

@Component({
  selector: 'app-toaster-container',
  standalone: true,
  imports: [],
  templateUrl: './toast-container.html',
  styleUrls: ['./toast-container.css']
})
export class ToasterContainerComponent {
  // Inject the management service into the component
  toastService = inject(ToastService);

  // Handle the exit animation before removing the element
  closeToast(event: MouseEvent, id: number) {
    const button = event.target as HTMLElement;
    const toastEl = button.closest('.toast-custom');
    
    if (toastEl) {
      toastEl.classList.add('toast-leaving');
      // Wait for the CSS animation to finish (200 ms) before removing the data from the signal
      setTimeout(() => {
        this.toastService.remove(id);
      }, 200);
    } else {
      this.toastService.remove(id);
    }
  }
}
