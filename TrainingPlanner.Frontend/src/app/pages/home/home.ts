import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';

@Component({
  selector: 'app-home',
  imports: [RouterLink, ...HlmButtonImports, ...HlmCardImports],
  templateUrl: './home.html',
})
export class HomePage {}