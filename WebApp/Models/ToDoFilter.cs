using System.ComponentModel;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Models
{
    public partial class ToDoFilter : QueryParameters, IDateTimeFilter
    {
        [AutoNotify]
        private DateTime? _from;

        [AutoNotify]
        private DateTime? _to;

        [AutoNotify]
        private Guid? _createdById;

        [AutoNotify]
        private Guid? _modifiedById;

        [AutoNotify]
        private bool? _isCompleted;

        [AutoNotify]
        private string _query;
    }
}
