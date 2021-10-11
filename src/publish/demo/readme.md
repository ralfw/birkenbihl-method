# Vorgehensweise

Angesichts von wackeligen Servern muss ein Retry vorhergesehen werden.
Die Produktion sollte daher in Schritten geschehen - und auch wiederaufnahmefähig sein, ohne dass
Arbeit, die schon erfolgreich getan wurde, wiederholt werden müsste (Schritte mit *).


## 1. Analyse
1. .md File parsen
2. Index erstellen
   
## 2. Konvertierung
1. Absätze übersetzen*
2. Index übersetzen*
3. Absätze transcribieren* // Die Downloads könnten nach dem Hash des Absatzes benannt werden

## 3. Generieren
Jetzt den interlinearen Text als HTML-Datei generieren.

- Original Absatz mit Link zur Transcription (.mp3) // Überschriftenlevel berücksichtigen
- Übersetzter Absatz
  
- Interlinearer Absatz: Zeilen à 80 Zeichen
  - Originale Zeile Chunk für Chunk
  - Übersetzte Zeile Wort für Wort // leere Worte bei nicht-leeren Chunks berücksichtigen

