using System;
using System.Threading.Tasks;
using System.Windows.Input;

/// <summary>
/// Ermöglicht die Ausführung von asynchronen Aufgaben.
/// </summary>
public sealed class AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    /// <summary>
    /// Event, das von der UI (z. B. Button) abonniert wird, um Änderungen der Ausführbarkeit zu erkennen.
    /// </summary>
    public event EventHandler? CanExecuteChanged;
    
    /// <summary>
    /// Prüft, ob das Kommando derzeit ausgeführt werden darf.
    /// </summary>
    /// <param name="parameter">ICommand-Parameter (nicht verwendet, da Command parameterlos ist)</param>
    /// <returns>
    /// true - wenn Kommando ausführbar,
    /// false - sonst
    /// </returns>
    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    /// <summary>
    /// Führt das Kommando aus.
    /// </summary>
    /// <param name="parameter">ICommand-Parameter (nicht verwendet, da Command parameterlos ist)</param>
    public async void Execute(object? parameter) => await execute();

    /// <summary>
    /// Löst <see cref="CanExecuteChanged"/> aus, damit die UI <see cref="CanExecute(object?)"/> erneut abfragt
    /// und z. B. Buttons entsprechend aktiviert/deaktiviert.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}