using BlazorWebApi.Files.Entities;
using BlazorWebApiFiles.SeedWork;

namespace BlazorWebApi.Repository;

//This is just the RepositoryContracts or Interface defined at the Domain Layer
//as requisite for the Order Aggregate

public interface IFileRepository : IRepository<FileData>
{
    //UploadFile Add(Order order);

}
