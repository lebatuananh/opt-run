import { Routes } from '@angular/router';
import { AuthenticationGuard } from '@app/shared/services/authentication.guard';

export const APP_LAYOUT_ROUTES: Routes = [
    //Apps
    {
        path: '',
        loadChildren: () => import('../views/apps/apps.module').then(m => m.AppsModule)
    },
];
