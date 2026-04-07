using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextExtractorGUI;

/// <summary>
/// Zentrale Basis für INotifyPropertyChanged.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    /// <summary>
    /// Event, um UI über Property-Änderungen zu informieren.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Hilfsmethode für PropertyChanged.
    /// </summary>
    /// <remarks>
    /// <see cref="CallerMemberName"/> sorgt dafür, dass der Property-Name automatisch eingesetzt wird.
    /// </remarks>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}