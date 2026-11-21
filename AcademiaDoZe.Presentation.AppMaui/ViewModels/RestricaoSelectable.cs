
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Presentation.AppMaui.Models;

public class RestricaoSelectable : INotifyPropertyChanged
{
    private bool _isSelected;

    public MatriculaRestricoes Value { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
                return;

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
