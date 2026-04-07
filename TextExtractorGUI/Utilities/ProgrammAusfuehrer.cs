using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TextExtractorGUI.Records;

namespace TextExtractorGUI.Utilities;

/// <summary>
/// Führt ein Programm aus.
/// </summary>
public static class ProgrammAusfuehrer
{
    /// <summary>
    /// Führt Extraktor aus und fängt dabei Console-Ausgaben ab (stdout/stderr).
    /// </summary>
    /// <param name="programm">auszuführendes Programm</param>
    /// <param name="args">Argumente für das auszuführende Programm</param>
    /// <returns>
    /// Tupel aus
    /// - <c>rueckgabeWert</c>: Rückgabecode (0 = Erfolg)
    /// - <c>ausgabeText</c>:   gesammelter stdout-Text (getrimmt)
    /// - <c>fehlerText</c>:    gesammelter stderr-Text (getrimmt)
    /// </returns>
    public static async Task<Ausgabe> Run(Func<int> programm)
    {
        // Puffer für "stdout"/"stderr"
        StringBuilder stringBuilderAusgabe = new();
        StringBuilder stringBuilderFehler = new();

        // Writer in die geschrieben wird
        await using StringWriter writerAusgabe = new(stringBuilderAusgabe);
        await using StringWriter writerFehler = new(stringBuilderFehler);

        // originale Streams zur späteren Wiederherstellung
        TextWriter tempAusgabe = Console.Out;
        TextWriter tempFehler = Console.Error;

        try
        {
            // leite auf Ausgabe und Fehler um
            Console.SetOut(writerAusgabe);
            Console.SetError(writerFehler);

            // starte Extraktion in einem separaten Task, um UI-Thread nicht zu blockieren
            int rueckgabeWert = await Task.Run(programm);

            // gesammelte Ausgaben und Fehler auslesen
            string ausgabeText = stringBuilderAusgabe.ToString();
            string fehlerText = stringBuilderFehler.ToString();
            return new Ausgabe(rueckgabeWert, ausgabeText, fehlerText);
        }
        finally
        {
            // Console wieder auf ursprünglichen Zustand zurücksetzen
            Console.SetOut(tempAusgabe);
            Console.SetError(tempFehler);
        }
    }
    
}