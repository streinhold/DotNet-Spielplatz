# DotNet Spielplatz

Dieses Spielplatzprojekt ist eine Programmierübung in C#.
Der Programmcode ist bewußt in Deutsch geschrieben.

## Voraussetzungen
- .Net SDK 3.1
- Visual Studio Code mit C# Erweiterung

## Vorbereitung
Öffnen des Arbeitsbereiches `DotNet.code-workspace` über "Open Workspace" in VSCode.
Dabei wird ein `dotnet restore` aufgerufen und die benötigten Binärdateien in die Unterverzeichnisse `bin` und `obj` kopiert.

## Progamm ausführen

### In VSCode

Programm in der "Run"-View über eine Launch configuration starten.
1. "Starte Schiffe Versenken" startet direkt "Schiffe versenken".
2. "Starte Hauptprogramm" startet die Menüauswahl.

### Auf der Kommandozeile

1. `dotnet run 1` startet direkt "Schiffe versenken".
2. `dotnet run` startet die Menüauswahl.

## Programme

### Schiffe versenken
#### Inhalt
Das ist ein einfaches klassisches "Schiffe versenken" in der Konsole.
Gib Koordinaten ein und finde alle Schiffe.

Es gibt:
- 1 Träger (5 Felder)
- 1 Schlachtschiff (4 Felder)
- 1 Kreuzer (3 Felder)
- 2 Zerstörer (2 Felder)
- 2 U-Boote (1 Feld)

#### Hilfe
Wird die Umgebungsvariable `DEBUG` auf `1` gesetzt, dann werden auch ungetroffene Schiffsfelder angezeigt.

In der Eingabeaufforderung (`cmd.exe`) geht das mit `set DEBUG=1`.
In der PowerShell muss `$env:DEBUG=1` ausgeführt werden.
