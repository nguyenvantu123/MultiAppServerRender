using BlazorIdentity.Files.Entities;
using BlazorIdentityFiles.SeedWork;

namespace BlazorIdentity.Repository;

//This is just the RepositoryContracts or Interface defined at the Domain Layer
//as requisite for the Order Aggregate

public interface IFileRepository : IRepository<FileData>
{
    //UploadFile Add(Order order);

}
