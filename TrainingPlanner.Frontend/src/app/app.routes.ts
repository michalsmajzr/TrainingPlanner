import { Routes } from '@angular/router';
import { HomePage } from './pages/home/home';
import { LoginPage } from './pages/login/login';
import { RegisterPage } from './pages/register/register';
import { DashboardPage } from './pages/dashboard/dashboard';
import { CreateWorkoutPage } from './pages/dashboard/create-workout/create-workout';
import { RecordWorkoutPage } from './pages/dashboard/record-workout/record-workout';
import { WorkoutDetailPage } from './pages/dashboard/workout-detail/workout-detail';
import { AddExercisePage } from './pages/dashboard/record-workout/add-exercise/add-exercise';

export const routes: Routes = [
  { path: '', component: HomePage, title: 'TrainingPlanner' },
  { path: 'login', component: LoginPage, title: 'Log In' },
  { path: 'register', component: RegisterPage, title: 'Register' },
  { path: 'dashboard', component: DashboardPage, title: 'Dashboard' },
  { path: 'create-workout', component: CreateWorkoutPage, title: 'Create Workout' },
  { path: 'record-workout', component: RecordWorkoutPage, title: 'Record Workout' },
  { path: 'record-workout/add-exercise', component: AddExercisePage, title: 'Add Exercise' },
  { path: 'dashboard/:id', component: WorkoutDetailPage, title: 'Workout' },
  { path: '**', redirectTo: '' },
];
