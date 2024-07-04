using System.ComponentModel;

namespace WebApp.Interfaces
{
    public interface IDateTimeFilter : INotifyPropertyChanged
    {
        DateTime? From { get; set; }
        DateTime? To { get; set; }
    }
}
