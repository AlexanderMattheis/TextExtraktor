# TextExtractor

Dieses Tool durchsucht HTML-Dateien nach `data-include`-Attributen, lädt die referenzierten HTML-Dateien und extrahiert gezielt Inhalte aus `<aside class="notes">`-Tags. Die gesammelten Notizen werden anschließend in eine strukturierte Markdown-Datei exportiert.

Das Programm eignet sich besonders für Präsentations-Workflows (z. B. mit Reveal.js), bei denen Sprecher-Notizen aus mehreren eingebundenen HTML-Dateien zentral gesammelt und weiterverarbeitet werden sollen.

**Features**

- Extraktion von Inhalten aus `<aside class="notes">`
- Ausgabe in gut lesbares Markdown
- Einfach in bestehende Projekte integrierbar

**Anwendungsfälle**

- Export von Sprecher-Notizen aus Präsentationen
- Dokumentation von HTML-basierten Inhalten
