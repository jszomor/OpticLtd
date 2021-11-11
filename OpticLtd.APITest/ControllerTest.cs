using Autofac.Extras.Moq;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpticLtd.Api.Controllers;
using OpticLtd.BusinessLogic.Product.Queries;
using OpticLtd.Data;
using OpticLtd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static OpticLtd.BusinessLogic.Product.Queries.GetProduct;

namespace OpticLtd.APITest
{
  public class ControllerTest
  {
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void ProductControllerTest()
    {
      //using (var mock = AutoMock.GetLoose())
      //{

      //var mockQuery = new Mock<Query>();

      var query = new Query();

      var mockHandler = new Mock<IRequestHandler<Query, List<Product>>>()
        .Setup(x => x.Handle(query, CancellationToken.None))
        .Returns(GetSampleProduct());

      var mockMediator = new Mock<IMediator>();
      var mockLogger = new Mock<ILogger<ProductController>>();

      //mock.Mock<Handler>()
      //    .Setup(x => x.Handle(query, CancellationToken.None))
      //    .Returns(GetSampleProduct());

      //var cls = mock.Create<Handler>();
      //var expected = GetSampleProduct();

      //var actual = cls.Handle(query, CancellationToken.None);

      //var productController = new ProductController(_mapper, _mediator, _logger);
      //var result = await productController.GetProduct(mockQuery);

      //Assert.True(actual != null);
      //Assert.Equal(expected.Count, actual.Count);

      //for (int i = 0; i < expected.Count; i++)
      //{
      //  Assert.Equal(expected[i].FirstName, actual[i].FirstName);
      //  Assert.Equal(expected[i].LastName, actual[i].LastName);
      //}
      //}
    }

    private Task<List<Product>> GetSampleProduct()
    {
      List<Product> products = new List<Product>
      {
        new Product
        {
          ProductId = 1,
          ProductCategory = "Lencse",
          ProductName = "Hoya"
        },
        new Product
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