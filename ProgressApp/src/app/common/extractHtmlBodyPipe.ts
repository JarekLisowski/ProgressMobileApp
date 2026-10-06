import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'extractHtmlBody',
    standalone: true
})
export class ExtractHtmlBodyPipe implements PipeTransform {
    private parser = new DOMParser();

    transform(value?: string | null): string {
        if (!value) return '';
        if (value.includes('<html') || value.includes('<HTML') || value.includes('<!DOCTYPE')) {
            const doc = this.parser.parseFromString(value, 'text/html');
            return doc.body.innerHTML;
        }
        return value;
    }
}
