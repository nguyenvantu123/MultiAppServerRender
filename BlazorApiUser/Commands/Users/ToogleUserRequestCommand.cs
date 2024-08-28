using MediatR;
using System.Runtime.Serialization;

namespace BlazorApiUser.Commands.Users
{
    [DataContract]
    public record ToogleUserRequestCommand : IRequest<Tuple<int, string>>
    {
        [DataMember]
        public bool ActivateUser { get; set; }

        [DataMember]
        public string UserId { get; set; }

      
    }
}
