using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace TextExtractorGUI;

/// <summary>
/// Einstiegspunkt der Anwendung.
/// </summary>
/// <remarks>
/// Diese Klasse verbindet die XAML-Definition (App.axaml) mit dem Laufzeitverhalten.
/// </remarks>
public class App : Application
{
    /// <summary>
    /// Methode aufgerufen zur Initialisierung der Anwendung.
    /// </summary>
    /// <remarks>
    /// Lädt das zugehörige XAML (App.axaml) und wendet definierte Ressourcen/Styles an.
    /// </remarks>
    public override void Initialize()
    {
        // lädt das XAML für diese Application-Instanz (Ressourcen, Styles, ggf. Theme-Definitionen).
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Methode aufgerufen nach der Initialisierung der Anwendung.
    /// </summary>
    /// <remarks>
    /// Hier wird typischerweise das ApplicationLifetime-spezifische Verhalten gesetzt,
    /// z.B. das Hauptfenster bei klassischen Desktop-Apps.
    /// </remarks>
    public override void OnFrameworkInitializationCompleted()
    {
        // wenn: klassisches Desktop-App (und nicht Mobile-App)
        // dann: setzt das Hauptfenster
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // setzt das beim Start aufzurufende Hauptfenster der Anwendung
            desktop.MainWindow = new MainWindow();
        }

        // ruft die Basisklassen-Implementierung auf (wichtig für den restlichen Framework-Start)
        base.OnFrameworkInitializationCompleted();
    }
}