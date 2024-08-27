// @ts-ignore
import { ApplicationConfig } from '@angular/core';
import { provideRoutes } from '@angular/router';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [provideRoutes(routes)]
};
