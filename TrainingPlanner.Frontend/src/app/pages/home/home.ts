import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { HlmButtonImports } from '@spartan-ng/helm/button';
import { HlmCardImports } from '@spartan-ng/helm/card';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageSwitcherComponent } from '@components/language-switcher';

@Component({
  selector: 'app-home',
  imports: [RouterLink, ...HlmButtonImports, ...HlmCardImports, TranslatePipe, LanguageSwitcherComponent],
  templateUrl: './home.html',
})
export class HomePage {}