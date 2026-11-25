import { DOCUMENT } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { lucideMoon, lucideSun } from '@ng-icons/lucide';
import { HlmButtonImports } from '@spartan-ng/helm/button';

@Component({
  selector: 'app-theme-switcher',
  imports: [HlmButtonImports, NgIcon, TranslatePipe],
  providers: [provideIcons({ lucideMoon, lucideSun })],
  template: `
    <button
      hlmBtn
      type="button"
      variant="outline"
      size="icon"
      [attr.aria-label]="
        (isDark() ? 'THEME.LIGHT' : 'THEME.DARK') | translate
      "
      [attr.aria-pressed]="isDark()"
      (click)="toggle()"
    >
      <ng-icon [name]="isDark() ? 'lucideSun' : 'lucideMoon'" />
    </button>
  `,
})
export class ThemeSwitcherComponent {
  private readonly document = inject(DOCUMENT);

  readonly isDark = signal(
    this.document.documentElement.classList.contains('dark'),
  );

  toggle(): void {
    const dark = !this.isDark();

    this.isDark.set(dark);
    this.document.documentElement.classList.toggle('dark', dark);

    try {
      this.document.defaultView?.localStorage.setItem(
        'theme',
        dark ? 'dark' : 'light',
      );
    } catch {
      // Ignore localStorage errors
    }
  }
}
