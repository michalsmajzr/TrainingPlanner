import { HttpInterceptorFn } from '@angular/common/http';
import { Injector, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export const languageInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.includes('/assets/i18n/')) {
    return next(req);
  }

  const translate = inject(Injector).get(TranslateService);
  const currentLang = translate.currentLang() || 'cs';

  const clonedReq = req.clone({
    headers: req.headers.set('Accept-Language', currentLang)
  });

  return next(clonedReq);
};
