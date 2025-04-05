//functional interceptor
//the error interceptor catches HTTP errors and performs different actions
//based on the status code of the error
import { HttpInterceptorFn } from '@angular/common/http'; //functional interceptor for HTTP requests
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router'; //for navigation
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs'; //catch errors from observable streams

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  //injecting dependencies
  const router = inject(Router);
  const toastr = inject(ToastrService);

  //intercepting the request and passing it to the next handler
  return next(req).pipe(
    catchError((error) => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              const modalStateErrors = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modalStateErrors.push(error.error.errors[key]);
                }
              }
              throw modalStateErrors.flat();
            } else {
              toastr.error(error.error, error.status);
            }
            break;
          case 401:
            toastr.error('Unauthorized', error.status);
            break;
          case 404:
            router.navigateByUrl('/not-found');
            break;
          case 500:
            const navigationExtras: NavigationExtras = {
              state: { error: error.error },
            };
            router.navigateByUrl('/server-error', navigationExtras);
            break;
          default:
            toastr.error('Something unexpected went wrong');
            break;
        }
      }
      throw error;
    })
  );
};
