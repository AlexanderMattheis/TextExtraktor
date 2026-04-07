using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using TextExtractorGUI.Infrastructure.Localization;

namespace TextExtractorGUI.Extensions;

/// <summary>
/// MarkupExtension für eine Lokalisierungsbindung unter gegebenem Schlüssel.
/// </summary>
/// <param name="key">Schlüssel für den der String abgefragt werden soll</param>
/// <remarks>
/// Verwendet, um mit Hilfe eines Schlüssels einen
/// String aus der Lokalisierung abzurufen
/// z. B. {ext:Loc Key=...} bzw. {ext:Loc ...}.
/// </remarks>
public sealed class LocExtension(string key) : MarkupExtension
{
    /// <summary>
    /// Liefert den Wert, den diese MarkupExtension an die Ziel-Property in XAML zurückgibt.
    /// </summary>
    /// <param name="serviceProvider">Infrastruktur-Parameter von Avalonia/XAML</param>
    /// <returns>
    /// Ein OneWay-<see cref="Binding"/>, dessen Source auf die Lokalisierungs-Singletoninstanz zeigt.
    /// </returns>
    /// <remarks>
    /// Hier wird kein statischer String zurückgegeben, sondern ein <see cref="Binding"/>,
    /// das auf den Indexer der Lokalisierungsquelle zeigt (z. B. Lokalisierung["MeinSchluessel"]).
    /// Dadurch aktualisiert sich der Text automatisch, wenn die Quelle entsprechende Änderungen signalisiert.
    /// </remarks>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // erzeugt ein Binding auf den Indexer mit dem aktuellen Key:
        // $"[{Key}]" entspricht einer Indexer-Bindung, z. B. Lokalisierung.Instanz[Key].
        return new Binding($"[{key}]")
        {
            // Binding-Quelle ist zentrale Lokalisierungsinstanz
            Source = Lokalisierung.Instanz,

            // OneWay: UI liest nur aus der Lokalisierung; UI-Änderungen werden nicht zurückgeschrieben.
            Mode = BindingMode.OneWay
        };
    }
    
}
