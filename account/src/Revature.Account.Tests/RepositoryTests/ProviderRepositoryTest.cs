using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Revature.Account.DataAccess;
using Revature.Account.DataAccess.Repositories;
using Revature.Account.Tests;
using Xunit;

namespace Revature.Account.Test.Repository_Tests
{
  /// <summary>
  /// Tests for the Provider's data access layer and it's supporting database negotiation methods.
  /// </summary>
  public class ProviderRepositoryTest
  {
    /// <summary>
    /// Test for adding a new Provider entry to the database.
    /// </summary>
    [Fact]
    public void AddNewProviderAccountTest()
    {
      // Arrange
      var helper = new TestHelper();
      var options = new DbContextOptionsBuilder<AccountDbContext>()
        .UseInMemoryDatabase("AddNewProviderAccountTest")
        .Options;
      var actContext = new AccountDbContext(options);
      var newProvider = helper.Providers[0];
      var actRepo = new GenericRepository(actContext, new Mapper());

      // Act
      actRepo.AddProviderAccountAsync(newProvider);
      actContext.SaveChanges();

      // Assert
      var assertContext = new AccountDbContext(options);
      var assertProvider = assertContext.ProviderAccount.FirstOrDefault(p => p.ProviderId == newProvider.ProviderId);
      Assert.NotNull(assertProvider);
    }

    /// <summary>
    /// Test for updateing a given Provider's information within the database.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task UpdateProviderAccountTestAsync()
    {
      // Arrange
      var helper = new TestHelper();
      var mapper = new Mapper();
      var options = new DbContextOptionsBuilder<AccountDbContext>()
        .UseInMemoryDatabase("UpdateProviderAccountTestAsync")
        .Options;
      var arrangeContext = new AccountDbContext(options);
      var arrangeProvider = helper.Providers[0];
      arrangeContext.ProviderAccount.Add(mapper.MapProvider(arrangeProvider));
      arrangeContext.SaveChanges();

      arrangeProvider.Name = "Robby";

      // Act
      var repo = new GenericRepository(arrangeContext, new Mapper());
      await repo.UpdateProviderAccountAsync(arrangeProvider);
      arrangeContext.SaveChanges();

      // Assert
      var assertContext = new AccountDbContext(options);
      var assertProvider = assertContext.ProviderAccount.First(p => p.ProviderId == arrangeProvider.ProviderId);
      Assert.Equal(arrangeProvider.Name, assertProvider.Name);
    }

    /// <summary>
    /// Retrieve a provider by way of a Guid Id from the database.
    /// </summary>
    [Fact]
    public async void GetProviderByIdTest()
    {
      // Arrange
      var helper = new TestHelper();
      var mapper = new Mapper();
      var options = new DbContextOptionsBuilder<AccountDbContext>()
        .UseInMemoryDatabase("GetProviderByIdTest")
        .Options;
      var arrangeContext = new AccountDbContext(options);

      arrangeContext.CoordinatorAccount.Add(mapper.MapCoordinator(helper.Coordinators[0]));
      arrangeContext.ProviderAccount.Add(mapper.MapProvider(helper.Providers[0]));
      arrangeContext.SaveChanges();
      var actContext = new AccountDbContext(options);
      var repo = new GenericRepository(actContext, new Mapper());

      // Act
      var result = await repo.GetProviderAccountByIdAsync(helper.Providers[0].ProviderId);

      // Assert
      Assert.NotNull(result);
    }

    /// <summary>
    /// Test the deletion of a given provider from the database.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DeleteProviderTestAsync()
    {
      //Assemble
      var helper = new TestHelper();
      var mapper = new Mapper();
      var options = new DbContextOptionsBuilder<AccountDbContext>()
        .UseInMemoryDatabase("DeleteProviderTestAsync")
        .Options;
      var assembleContext = new AccountDbContext(options);
      var deleteProvider = mapper.MapProvider(helper.Providers[2]);
      assembleContext.Add(deleteProvider);
      assembleContext.SaveChanges();
      var actContext = new AccountDbContext(options);
      var repo = new GenericRepository(actContext, new Mapper());
      // Act
      await repo.DeleteProviderAccountAsync(deleteProvider.ProviderId);
      // Assert
      var provider = actContext.ProviderAccount.ToList();
      Assert.DoesNotContain(deleteProvider, provider);
    }
  }
}
