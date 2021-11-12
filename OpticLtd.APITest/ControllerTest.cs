using Autofac.Extras.Moq;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpticLtd.Api.Controllers;
using OpticLtd.Api.Mapping;
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
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ControllerTest()
    {
      var mapperConfig = new MapperConfiguration(c =>
      {
        c.AddProfile<MappingProfile>();
      });

      _mapper = mapperConfig.CreateMapper();
    }

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public async Task ProductControllerTest()
    {
      //using (var mock = AutoMock.GetLoose())
      //{
       
      var mockQuery = new Mock<Query>();


      using (var mockHandler = AutoMock.GetLoose())
      {
        mockHandler.Mock<IRequestHandler<Query, List<Product>>>()
        .Setup(x => x.Handle(mockQuery.Object, CancellationToken.None))
        .Returns(GetSampleProduct());

        var cls = mockHandler.Create<GetProduct.Query>();

        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<ProductController>>();

        var productController = new ProductController(_mapper, mockMediator.Object, mockLogger.Object);
        var result = await productController.GetProduct(cls);

      }
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