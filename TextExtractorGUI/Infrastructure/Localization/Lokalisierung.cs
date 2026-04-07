using System;
using System.ComponentModel;
using System.Globalization;
using TextExtractorGUI.Ressources;

namespace TextExtractorGUI.Infrastructure.Localization;

/// <summary>
/// Lokalisierung der Anwendung.
/// </summary>
/// <remarks>
/// Aufgaben:
/// - verwaltet die aktuell aktive <see cref="CultureInfo"/>.
/// - liefert lokalisierte Texte aus den .resx-Ressourcen über einen Indexer.
/// - informiert die UI (Bindings) via <see cref="INotifyPropertyChanged"/>
///   bei Änderungen an Kultur/Sprache.
/// </remarks>
public sealed class Lokalisierung : ObservableObject
{
    /// <summary>
    /// Singleton-Instanz der Lokalsierung zum Einsatz in XAML/Bindings.
    /// </summary>
    public static Lokalisierung Instanz { get; } = new();

    /// <summary>
    /// Aktuell aktive Kultur für Resource-Lookups und Formatierungen.
    /// </summary>
    /// <remarks>
    /// Initial wird <see cref="CultureInfo.CurrentUICulture"/> verwendet.
    /// </remarks>
    public CultureInfo Kultur
    {
        get; 
        private set;
    } = CultureInfo.CurrentUICulture;

    /// <summary>
    /// Privater Konstruktor zur Sicherstellung einer einzelnen Instanz.
    /// </summary>
    private Lokalisierung() { }

    /// <summary>
    /// Setzt die aktuelle Kultur anhand eines Kultur-Tags (z. B. "de-DE", "en-US").
    /// </summary>
    /// <param name="cultureName">Tag der Kultur</param>
    /// <remarks>
    /// Neben der eigenen <see cref="Kultur"/> werden auch
    /// <see cref="CultureInfo.CurrentUICulture"/> und <see cref="CultureInfo.CurrentCulture"/> gesetzt:
    /// - CurrentUICulture: relevant für Resource-Lookups in vielen Frameworks
    /// - CurrentCulture: relevant für allgemeine Formatierungen (Zahlen/Datum etc.)
    ///
    /// Anschließend werden PropertyChanged-Events ausgelöst, damit die UI aktualisiert.
    /// </remarks>
    public void SetKultur(string cultureName)
    {
        // liefert eine Instanz der Kultur
        CultureInfo kultur = CultureInfo.GetCultureInfo(cultureName);

        // setzt globale Thread-/App-weite Kulturen (je nach Runtime/Framework-Verhalten)
        CultureInfo.CurrentUICulture = kultur;
        CultureInfo.CurrentCulture = kultur;
        Kultur = kultur; // setzt Kultur in dieser Lokalisierungs-Klasse

        // informiert UI über Änderung der Kultur
        // viele Binding-Engines signalisieren Indexer-Änderungen über Property-Namen:
        // - "Item"
        // - "Item[]"
        // - leerer String
        // Dadurch wird z. B. Text="{Binding [Window_Title]}" neu ausgewertet.
        OnPropertyChanged(nameof(Kultur));
        OnPropertyChanged("Item");
        OnPropertyChanged("Item[]");
        OnPropertyChanged(string.Empty);
    }
    
    /// <summary>
    /// Indexer zum Abrufen eines lokalisierten Strings über seinen Ressourcenschlüssel.
    /// </summary>
    /// <param name="key">Ressourcenschlüssel (Name des .resx-Eintrags).</param>
    /// <remarks>
    /// Beispiel:
    /// <code>string titel = Lokalisierung.Instanz["Window_Title"];</code>
    ///
    /// Rückgabeverhalten:
    /// - Schlüssel gefunden => lokalisierter Text
    /// - Schlüssel gefunden nicht gefunden => Fallback-String
    /// </remarks>
    public string this[string key]
    {
        get
        {
            // holt Wert aus automatisch generierten ResourceManager der Strings.resx.
            // CurrentCulture steuert, aus welcher Sprachdatei gelesen wird
            string? wert = Strings.ResourceManager.GetString(key, Kultur);

            // wenn: Resource fehlt oder leer ist
            // dann: Platzhalter zurückgeben (damit fehlende Übersetzungen sofort auffallen)
            return string.IsNullOrEmpty(wert) ? $"!{key}!" : wert;
        }
    }
    
    /// <summary>
    /// Liefert einen lokalisierten Text und formatiert ihn kulturell korrekt.
    /// </summary>
    /// <param name="key">Schlüssel der Ressource</param>
    /// <param name="args">Formatargumente für <see cref="string.Format(IFormatProvider,string,object?[])"/>.</param>
    /// <remarks>
    /// Formatierung erfolgt mit <see cref="CurrentCulture"/> (z. B. Dezimaltrennzeichen/Datumsformate)
    /// </remarks>
    public string Format(string key, params object?[] args)
        => string.Format(Kultur, this[key], args);
}