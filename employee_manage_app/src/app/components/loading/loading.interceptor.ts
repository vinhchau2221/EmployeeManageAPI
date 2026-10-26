// loading.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Loading } from '../../services/loading/loading';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(Loading);
  
  // 1. Kích hoạt loading màn hình
  loading.show();

  // 2. Chuyển tiếp request đi và tự động tắt loading khi request hoàn thành/lỗi
  return next(req).pipe(
    finalize(() => {
      loading.hide();
    })
  );
};
