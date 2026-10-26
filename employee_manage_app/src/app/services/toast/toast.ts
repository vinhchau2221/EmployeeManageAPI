import { Service, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastMessage {
   id: number;
   type: ToastType;
   title?: string;
   message: string;
   classname: string;
   icon: string;
   delay?: number;
   isLeaving?: boolean;
}

@Service()
export class ToastService {
   // Manage the toast list state with a signal
   toasts = signal<ToastMessage[]>([]);

   private show(
      message: string,
      type: ToastType,
      options: { title?: string; delay?: number } = {}
   ) {
      const id = Date.now();

      // Define the configuration here
      const config = {
         success: { classname: 'bg-success text-white', icon: 'bi-check-circle-fill' },
         error: { classname: 'bg-danger text-white', icon: 'bi-exclamation-triangle-fill' },
         warning: { classname: 'bg-warning text-dark', icon: 'bi-exclamation-circle-fill' },
         info: { classname: 'bg-info text-white', icon: 'bi-info-circle-fill' }
      }[type];

      // Merge the configuration into the new toast object using the spread operator
      const newToast: ToastMessage = {
         id,
         type,
         message,
         title: options.title,
         delay: options.delay ?? 4000,
         isLeaving: false,
         ...config // Automatically assign the class name and icon from the configuration above
      };

      this.toasts.update(current => [...current, newToast]);

      if (newToast.delay && newToast.delay > 0) {
         setTimeout(() => {
            this.toasts.update(current =>
               current.map(t => t.id === id ? { ...t, isLeaving: true } : t)
            );
            setTimeout(() => this.remove(id), 200);
         }, newToast.delay);
      }
   }

   // Convenience methods called by the component
   success(message: string, title = 'Success', delay?: number) {
      this.show(message, 'success', { title, delay });
   }

   error(message: string, title = 'Error', delay?: number) {
      this.show(message, 'error', { title, delay });
   }

   warning(message: string, title = 'Warning', delay?: number) {
      this.show(message, 'warning', { title, delay });
   }

   info(message: string, title = 'Info', delay?: number) {
      this.show(message, 'info', { title, delay });
   }

   remove(id: number) {
      this.toasts.update(current => current.filter(t => t.id !== id));
   }
}
