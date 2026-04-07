using System;
using System.Windows.Input;
using TextExtractorGUI.Localization;
using TextExtractorGUI.Records;
using TextExtractorGUI.Services;
using TextExtractorGUI.Utilities;

namespace TextExtractorGUI.ViewModels.MainWindow;

/// <summary>
/// Kommandos definiert im ViewModel.
/// </summary>
public sealed partial class MainWindowViewModel
{
    #region Felder
    /// <summary>
    /// Command zum Auswählen der index.html.
    /// </summary>
    public ICommand BrowseIndexHtmlCommand { get; internal set; }

    /// <summary>
    /// Command zum Auswählen des Markdown-Ausgabeorts.
    /// </summary>
    public ICommand BrowseMarkdownCommand { get; internal set; }

    /// <summary>
    /// Command zum Starten der Extraktion.
    /// </summary>
    public ICommand StartBerechnungCommand { get; internal set; }
    #endregion

    #region BrowseIndexHtmlCommand
    /// <summary>
    /// Erstellt das Kommando, mit dem der Benutzer die Eingabe-Datei (index.html) auswählen kann.
    /// </summary>
    /// <param name="dialogService">Service zum Öffnen des „Datei öffnen“-Dialogs.</param>
    /// <returns>
    /// Command zum Öffnen des Dateiauswahl-Dialogs für die "index.html".
    /// </returns>
    private AsyncCommand CreateBrowseIndexHtmlCommand(DateiDialogService dialogService)
    {
        return new AsyncCommand(execute: async () =>
        {
            // öffnet Datei-Dialog und gibt ggf. den ausgewählten Pfad zurück
            string? pfad = await dialogService.PickIndexHtmlAsync();

            // wenn: gültiger Pfad
            // dann: übernehme in ViewModel
            if (!string.IsNullOrWhiteSpace(pfad)) IndexHtmlPfad = pfad;
        }, canExecute: () => !IsBeschaeftigt);
    }
    #endregion

    #region BrowseMarkdownCommand
    /// <summary>
    /// Erstellt das Kommando, mit dem der Benutzer die Ausgabe-Datei (*.md) auswählen kann.
    /// </summary>
    /// <param name="dialogService">Service zum Öffnen des „Speichern unter“-Dialogs.</param>
    /// <returns>
    /// Command zum Öffnen des Dateiauswahl-Dialogs für den Ausgabepfad.
    /// </returns>
    private AsyncCommand CreateBrowseMarkdownCommand(DateiDialogService dialogService)
    {
        return new AsyncCommand(execute: async () =>
        {
            // öffnet "Speichern unter"-Dialog; "notizen.md" als vorgeschlagener Dateiname.
            string? pfad = await dialogService.PickOutputMarkdownAsync(Lokalisierung.Instanz["Notes_Name"]);

            // wenn: gültiger Pfad
            // dann: übernehme in ViewModel
            if (!string.IsNullOrWhiteSpace(pfad)) MarkdownPfad = pfad;
        }, canExecute: () => !IsBeschaeftigt);
    }
    #endregion

    #region StartBerechnungCommand
    /// <summary>
    /// Erstelle Start-Kommando.
    /// </summary>
    /// <remarks>
    /// Ablauf:
    /// 1) validiere Eingaben (index.html, *.md)
    /// 2) setze GUI beschäftigt
    /// 3) führe Extraktor aus
    /// 4) erstelle Status-Text
    /// 5) setze GUI unbeschäftigt
    /// </remarks>
    private AsyncCommand CreateStartBerechnungCommand()
    {
        return new AsyncCommand(execute: async () =>
        {
            // 1. validiere Eingaben 
            // wenn: Eingaben fehlerhaft
            // dann: beende mit Fehlermeldung
            if (!TryValidateEingaben(out string fehler))
            {
                Status = fehler;
                return;
            }

            // 2. setze GUI beschäftigt
            SetGuiBeschaeftigt(true, "Extrahiere…");

            try
            {
                // 3. führe Extraktor aus
                Ausgabe ausgabe =
                    await ProgrammAusfuehrer.Run(() => TextExtractor.Program.Main([IndexHtmlPfad!, MarkdownPfad!]));

                // 4. erstelle Status-Text
                Status = BuildStatusText(ausgabe, MarkdownPfad!);
            }
            catch (Exception exception)
            {
                Status = "Unerwarteter Fehler: " + exception.Message;
            }
            finally
            {
                // 5. setze GUI unbeschäftigt
                SetGuiBeschaeftigt(false);
            }
        }, canExecute: () => !IsBeschaeftigt);
    }

    /// <summary>
    /// Prüft, ob die notwendigen Eingaben vorhanden sind.
    /// </summary>
    /// <param name="fehler">im Fehlerfall Fehlermeldung</param>
    /// <returns>
    /// <c>true</c> - wenn alle notwendigen Eingaben gesetzt sind,
    /// <c>false</c> - sonst
    /// </returns>
    private bool TryValidateEingaben(out string fehler)
    {
        // wenn: Eingabepfad nicht gesetzt
        // dann: breche mit Fehlermeldung ab
        if (string.IsNullOrWhiteSpace(IndexHtmlPfad))
        {
            fehler = Lokalisierung.Instanz["Error_SelectIndexHtml"];
            return false;
        }

        // wenn: Ausgabe-Pfad nicht gesetzt
        // dann: breche mit Fehlermeldung ab
        if (string.IsNullOrWhiteSpace(MarkdownPfad))
        {
            fehler = Lokalisierung.Instanz["Error_SelectMarkdownTarget"];
            return false;
        }

        fehler = "";
        return true;
    }

    /// <summary>
    /// Setzt den "beschäftigt"-Zustand des ViewModels und optional einen Status-Text.
    /// </summary>
    /// <param name="beschaeftigt">beschreibt, ob die UI beschäftigt ist</param>
    /// <param name="statusText">
    /// Optional: zu setzender Status-Text.
    /// Wenn <c>null</c>, wird der aktuelle Status nicht verändert.
    /// </param>
    private void SetGuiBeschaeftigt(bool beschaeftigt, string? statusText = null)
    {
        IsBeschaeftigt = beschaeftigt;
        if (statusText is not null) Status = statusText;
    }

    /// <summary>
    /// Baut aus Rückgabecode und Logs einen Status-Text für die GUI.
    /// </summary>
    /// <param name="ausgabe">Objekt mit Informationen zur Ausgabe</param>
    /// <param name="markdownPfad">Zielpfad der erzeugten Markdown-Datei.</param>
    /// <returns>
    /// benutzerfreundlicher, mehrzeiliger Status-Text
    /// </returns>
    private static string BuildStatusText(Ausgabe ausgabe, string markdownPfad)
    {
        // wenn: Ausgabe erfolgreich
        // dann: gebe positive Meldung aus
        if (ausgabe.rueckgabeWert == 0)
        {
            string ergebnisErfolg = Lokalisierung.Instanz.Format("Result_Success", markdownPfad);
            string logInformationen = string.IsNullOrWhiteSpace(ausgabe.text)
                ? string.Empty
                : $"\n\n{Lokalisierung.Instanz["Label_Log"]}\n{ausgabe.text}";

            return ergebnisErfolg + logInformationen;
        }

        // sonst: gebe Fehlermeldung aus
        string endeMitFehler = Lokalisierung
            .Instanz.Format("Result_EndedWithCode", ausgabe.rueckgabeWert);
        
        string fehler = string.IsNullOrWhiteSpace(ausgabe.fehler)
            ? string.Empty
            : $"\n\n{Lokalisierung.Instanz["Label_Error"]}\n{ausgabe.fehler}";
        
        string text = string.IsNullOrWhiteSpace(ausgabe.text)
            ? string.Empty
            : $"\n\n{Lokalisierung.Instanz["Label_Log"]}\n{ausgabe.text}";
        
        return endeMitFehler + fehler + text;
    }
    #endregion
}