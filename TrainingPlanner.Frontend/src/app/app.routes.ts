import { Routes } from '@angular/router';
import { HomePage } from './pages/home/home';
import { LoginPage } from './pages/login/login';
import { RegisterPage } from './pages/register/register';
import { DashboardPage } from './pages/dashboard/dashboard';
import { CreateWorkoutPage } from './pages/dashboard/create-workout/create-workout';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'TrainingPlanner' },
  { path: 'login', component: LoginPage, title: 'Přihlášení' },
  { path: 'register', component: RegisterPage, title: 'Registrace' },
  { path: 'dashboard', component: DashboardPage, title: 'Dashboard' },
  { path: 'create-workout', component: CreateWorkoutPage, title: 'Vytvoření tréninku' },
  { path: '**', redirectTo: '' },
];