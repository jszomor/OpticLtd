using Moq;
using NUnit.Framework;
using OpticLtd.BusinessLogic.Product.Queries;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace OpticLtd.BusinessLogic.Test
{
  public class ProductTest
  {

    [Test]
    public void GetProductTest()
    {
      var productQuery = new GetProduct.Query();

      var repository = new MockRepository(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };

      // Create a mock using the repository settings
      var mockGetProduct = repository.Create<IRequestHandler<GetProduct.Query, List<Data.Entities.Product>>>();
            
      mockGetProduct.Setup(mock => mock.Handle(productQuery, CancellationToken.None))
      .Returns(GetSampleProduct());

      List<Data.Entities.Product> actual = mockGetProduct.Object.Handle(productQuery, CancellationToken.None).Result;

      List<Data.Entities.Product> expected = GetSampleProduct().Result;

      for (int i = 0; i < expected.Count; i++)
      {
        Assert.AreEqual(expected[i].ProductId, actual[i].ProductId);
        Assert.AreEqual(expected[i].ProductCategory, actual[i].ProductCategory);
        Assert.AreEqual(expected[i].ProductName, actual[i].ProductName);
      }
    }

    private Task<List<Data.Entities.Product>> GetSampleProduct()
    {
      List<Data.Entities.Product> products = new List<Data.Entities.Product>
      {
        new Data.Entities.Product
        {
          ProductId = 1,
          ProductCategory = "Lencse",
          ProductName = "Hoya"
        },
        new Data.Entities.Product
        {
          ProductId = 2,
          ProductCategory = "Szemüveg",
          ProductName = "Rayban"
        }

      };
      return Task.FromResult(products);
    }
  }
}