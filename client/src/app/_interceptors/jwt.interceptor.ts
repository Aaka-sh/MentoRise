//functional interceptor
//this interceptor attaches the JWT token to the request header
//this interceptor is used to authenticate the user
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  //injecting the AccountService dependency
  const accountService = inject(AccountService);
  //checking if the user is logged in
  //if the user is logged in, we need to attach the JWT token to the request header
  if (accountService.currentUser()) {
    //req is a HttpRequest object
    //we need to clone the request object and add the Authorization header
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accountService.currentUser()?.token}`,
      },
    });
  }
  //after modifying the request object, we need to pass it to the next interceptor in the chain
  return next(req);
};
