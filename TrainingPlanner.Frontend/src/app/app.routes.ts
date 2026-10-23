import { Routes } from '@angular/router';
import { HomePage } from './pages/home/home';
import { LoginPage } from './pages/login/login';
import { RegisterPage } from './pages/register/register';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'TrainingPlanner' },
  { path: 'login', component: LoginPage, title: 'Přihlášení' },
  { path: 'register', component: RegisterPage, title: 'Registrace' },
  { path: '**', redirectTo: '' },
];