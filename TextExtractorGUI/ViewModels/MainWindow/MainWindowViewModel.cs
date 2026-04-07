using TextExtractorGUI.Services;

namespace TextExtractorGUI.ViewModels.MainWindow;

/// <summary>
/// ViewModel für das Hauptfenster.
/// </summary>
/// <remarks>
/// Verantwortlich für das Bereitstellen von Commands
/// (z. B. Datei-Auswahl und Starten der Verarbeitung),
/// die anschließend von der View (MainWindow) gebunden und ausgelöst werden.
/// </remarks>
public sealed partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// Erstellt eine neue Instanz des <see cref="MainWindowViewModel"/> und initialisiert die Commands.
    /// </summary>
    /// <param name="dialogService">
    /// Service zum Öffnen von Datei-Dialogen (z. B. zur Auswahl von Index-HTML oder Markdown-Dateien).
    /// Wird an die jeweiligen Command-Factories weitergereicht.
    /// </param>
    public MainWindowViewModel(DateiDialogService dialogService)
    {
        // Command zum Auswählen einer index.html (oder vergleichbaren HTML-Eingabedatei).
        // Benötigt den Dialog-Service, um dem Benutzer einen Dateiauswahldialog anzuzeigen.
        BrowseIndexHtmlCommand = CreateBrowseIndexHtmlCommand(dialogService);

        // Command zum Auswählen einer Markdown-Zieldatei/-quelle (je nach Anwendungslogik).
        // Benötigt den Dialog-Service, um dem Benutzer einen Dateiauswahldialog anzuzeigen.
        BrowseMarkdownCommand = CreateBrowseMarkdownCommand(dialogService);

        // Command zum Starten der eigentlichen Berechnung/Verarbeitung.
        // Benötigt hier keinen Dialog-Service, da die Arbeit auf bereits ausgewählten Pfaden/Daten basiert.
        StartBerechnungCommand = CreateStartBerechnungCommand();
    }
}