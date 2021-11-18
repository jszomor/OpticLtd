using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OpticLtd.Api;
using OpticLtd.Api.Controllers;
using OpticLtd.Api.Mapping;
using OpticLtd.BusinessLogic.Product.Queries;
using OpticLtd.Data;
using OpticLtd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static OpticLtd.BusinessLogic.Product.Queries.GetProduct;

namespace OpticLtd.APITest
{
  public class TestController
  {
    private readonly StartupMock _startupMock;
    public TestController(StartupMock startupMock)
    {
      _startupMock = startupMock;
    }


    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public TestController()
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

      var mockStartup = _startupMock.StartupInstance();


      var query = new Query();

      var mock = new Mock<IRequestHandler<Query, List<Product>>>();
      mock.Setup(p => p.Handle(query, CancellationToken.None)).Returns(GetSampleProduct());




      using (var mockHandler = AutoMock.GetLoose())
      {
        mockHandler.Mock<IRequestHandler<Query, List<Product>>>()
        .Setup(x => x.Handle(query, CancellationToken.None))
        .Returns(GetSampleProduct());

        var cls = mockHandler.Create<GetProduct.Query>();

        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<ProductController>>();

        var productController = new ProductController(_mapper, mockMediator.Object, mockLogger.Object);
        //var result = await productController.GetProduct(cls);

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

    [Test]
    public async Task StartupStatusCodeTest()
    {
      var waf = new WebApplicationFactory<Startup>();
      var client = waf.CreateDefaultClient();
      var response = await client.GetAsync("api/product/GetProduct");
      response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
  }
}