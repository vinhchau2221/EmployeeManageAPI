import { Service } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Service()
export class Loading {
   private activeRequests = 0; // Request counter
   private loadingSubject = new BehaviorSubject<boolean>(false);
   isLoading$ = this.loadingSubject.asObservable();

   show() {
      this.activeRequests++;
      this.loadingSubject.next(true);
   }

   hide() {
      this.activeRequests--;
      // Hide the loader only after all API requests have completed (when the counter reaches 0).
      if (this.activeRequests <= 0) {
         this.activeRequests = 0;
         this.loadingSubject.next(false);
      }
   }
}
