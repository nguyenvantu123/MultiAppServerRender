using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlazorWebApiFiles.Entity._base
{
    public class EntityBase : IEntityBase
    {
        [JsonIgnore]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public Guid? DeletedById { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public Guid? InsertedById { get; set; }

        [JsonIgnore]
        public DateTime InsertedAt { get; set; }

        [JsonIgnore]
        public Guid? UpdatedById { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public bool IsActive { get; set; } = true;

        public int? RequestedHashCode { get; set; }

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        [JsonIgnore]
        public string DeletedBy { get; set; }

        [JsonIgnore]
        public string InsertedBy { get; set; }

        [JsonIgnore]
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return this.Id == default;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is EntityBase))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            EntityBase item = (EntityBase)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!RequestedHashCode.HasValue)
                    RequestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return RequestedHashCode.Value;
            }
            else
                return base.GetHashCode();

        }
        public static bool operator ==(EntityBase left, EntityBase right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(EntityBase left, EntityBase right)
        {
            return !(left == right);
        }

    }
}
