using BlazorIdentity.Users.Models;

namespace BlazorApiUser.Repository;


//This is just the RepositoryContracts or Interface defined at the Domain Layer
//as requisite for the Order Aggregate

public interface IUserRepository : IRepository<ApplicationUser>
{
    int Add(ApplicationUser order);

    void Update(ApplicationUser order);

    Task<ApplicationUser> GetAsync(int orderId);
}
