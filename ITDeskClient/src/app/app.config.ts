import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations'
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { GoogleLoginProvider, SocialAuthServiceConfig, SocialLoginModule } from '@abacritt/angularx-social-login';
import { MessageService } from 'primeng/api';

export const appConfig: ApplicationConfig = {
  providers: 
  [
    MessageService,
    provideHttpClient(), 
    provideRouter(routes),
    importProvidersFrom(
      [
      BrowserAnimationsModule,
      SocialLoginModule]
      ),
      {
        provide: 'SocialAuthServiceConfig',
        useValue: {
          autoLogin: false,
          providers: [
            {
              id: GoogleLoginProvider.PROVIDER_ID,
              provider: new GoogleLoginProvider(
                '404400755787-iv3mi17oc3c0tcobtr7nuqpv0slipcac.apps.googleusercontent.com'
              )
            }
          ],
          onError: (err:HttpErrorResponse) => {
            console.error(err);
          }
        } as SocialAuthServiceConfig,
      }
    ]
};
