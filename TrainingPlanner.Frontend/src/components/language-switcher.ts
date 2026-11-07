import { Component, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { HlmDropdownMenuImports } from '@spartan-ng/helm/dropdown-menu';

interface Language {
  code: string;
  label: string;
  flag: string;
}

@Component({
  selector: 'app-language-switcher',
  standalone: true,
  imports: [HlmDropdownMenuImports],
  template: `
    <button
      hlmBtn
      variant="outline"
      size="sm"
      [hlmDropdownMenuTrigger]="menu"
      class="cursor-pointer hover:bg-accent px-2 py-1 rounded-xl"
    >
      <span class="font-medium">{{ currentLang().flag }}</span>
    </button>

    <ng-template #menu>
      <hlm-dropdown-menu class="w-32">
        <div hlmDropdownMenuGroup>
          @for (lang of languages; track lang.code) {
            <button
              hlmDropdownMenuItem
              (click)="selectLanguage(lang.code)"
              [class.bg-accent]="currentLang().code === lang.code"
              class="flex items-center justify-between cursor-pointer"
            >
              <span>{{ lang.label }}</span>
              @if (currentLang().code === lang.code) {
                <span class="text-xs text-muted-foreground">✓</span>
              }
            </button>
          }
        </div>
      </hlm-dropdown-menu>
    </ng-template>
  `
})
export class LanguageSwitcherComponent {
  private readonly translate = inject(TranslateService);

  readonly languages: Language[] = [
    { code: 'cs', label: 'Čeština', flag: 'CZ' },
    { code: 'en', label: 'English', flag: 'EN' }
  ];

  readonly currentLang = signal<Language>(this.languages[0]);

  constructor() {
    // Nastavíme výchozí jazyk při načtení
    this.translate.addLangs(['cs', 'en']);
    this.translate.setFallbackLang('cs');
    
    const activeLang = this.translate.currentLang() || 'cs';
    this.useLanguage(activeLang);
  }

  selectLanguage(code: string): void {
    this.useLanguage(code);
  }

  private useLanguage(code: string): void {
    const selected = this.languages.find((l) => l.code === code) || this.languages[0];
    this.translate.use(code);
    this.currentLang.set(selected);
  }
}
