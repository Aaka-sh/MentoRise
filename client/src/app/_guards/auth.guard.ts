import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const authGuard: CanActivateFn = (route, state) => {
  //injecting dependencies using inject function
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  // Check if the user is logged in
  // If logged in, return true
  // If not logged in, show error message and return false
  if (accountService.currentUser()) {
    return true;
  } else {
    toastr.error('You need to be logged in to access this page');
    return false;
  }
};
