using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using OpticLtd.Api;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpticLtd.APITest
{
  public class TestController
  {
    private List<Domain.Model.Product> GetSampleProduct()
    {
      List<Domain.Model.Product> products = new List<Domain.Model.Product>
      {
        new Domain.Model.Product
        {
          ProductId = 2,
          ProductCategory = "Szemüveg Keret",
          ProductName = "Keret edf",
          Description = "string",
          Stock = 3,
          Picture = "string",
          Brand = "string",
          Gender = true,
          AgeGroup = true,
          FeatureId = 0,
          ProductFeature = null
        },
        new Domain.Model.Product
        {
          ProductId = 3,
          ProductCategory = "Szemüveg Lencse",
          ProductName = "Lencse 123",
          Description = "string",
          Stock = 13,
          Picture = "string",
          Brand = "string",
          Gender = true,
          AgeGroup = true,
          FeatureId = 0,
          ProductFeature = null
        },
        new Domain.Model.Product
        {
          ProductId = 6,
          ProductCategory = "Szemüveg",
          ProductName = "Keret",
          Description = "Muanyag",
          Stock = 8,
          Picture = "SzemuvegPic.jpg",
          Brand = "Hoya",
          Gender = false,
          AgeGroup = false,
          FeatureId = 0,
          ProductFeature = null
        },
        new Domain.Model.Product
        {
          ProductId = 7,
          ProductCategory = "TestSzemüveg",
          ProductName = "TestKeret",
          Description = "TestMuanyag",
          Stock = 8,
          Picture = "SzemuvegPic.jpg",
          Brand = "Hoya",
          Gender = false,
          AgeGroup = false,
          FeatureId = 0,
          ProductFeature = null
        }
      };
      return products;
    }

    string GetProductEndPoint { get { return "api/product/GetProduct"; } }
    string CreateProductEndPoint { get { return "api/product/CreateProduct"; } }
    string DeleteProductEndPoint { get { return "api/product/DeleteProduct"; } }

    private HttpClient CallApi()
    {
      var waf = new WebApplicationFactory<Startup>();
      return waf.CreateDefaultClient();
    }

    [Test]
    public async Task MultipleFunctionProductEndPointTest()
    {
      var product = GetSampleProduct()[3];
      var productJson = JsonConvert.SerializeObject(product);
      var stringContent = new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
      var responseCreate = CallApi().PostAsync(CreateProductEndPoint, stringContent).Result;
      responseCreate.EnsureSuccessStatusCode();

      var responseBody = await responseCreate.Content.ReadAsStringAsync();
      var insertedProduct = JsonConvert.DeserializeObject<Domain.Model.Product>(responseBody);
      var actualProducts = CallApi().GetAsync(GetProductEndPoint + $"?productId={insertedProduct.ProductId}").Result.Content.ReadAsStringAsync().Result;
      
      product.ProductId = insertedProduct.ProductId;
      var expectedProduct = JsonConvert.SerializeObject(new List<Domain.Model.Product> { product });

      Assert.AreEqual(expectedProduct, actualProducts);

      var responseDelete = CallApi().DeleteAsync(DeleteProductEndPoint + $"?productId={insertedProduct.ProductId}").Result;
      responseDelete.EnsureSuccessStatusCode();

      //CallApi().GetAsync(GetProductEndPoint + $"?productId={insertedProduct.ProductId}").Result.StatusCode.Should().Be(HttpStatusCode.NotFound);

      var notFoundProduct = CallApi().GetAsync(GetProductEndPoint + $"?productId={insertedProduct.ProductId}").Result.RequestMessage.Content;

      Assert.AreEqual(null, notFoundProduct);
    }

    [Test]
    public void GetProductApiStatusCode_Should_Be_Ok()
    {
      CallApi().GetAsync(GetProductEndPoint).Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public void GetProductAssertNo6_ShouldBe_Equal()
    {
      var read = CallApi().GetAsync(GetProductEndPoint + "?productId=6").Result.Content.ReadAsStringAsync().Result;
      var actualProduct = JsonConvert.DeserializeObject<List<Domain.Model.Product>>(read)[0];      
      var expectedProduct = GetSampleProduct()[2];

      Assert.AreEqual(expectedProduct.ProductId, actualProduct.ProductId);
      Assert.AreEqual(expectedProduct.ProductCategory, actualProduct.ProductCategory);
      Assert.AreEqual(expectedProduct.ProductName, actualProduct.ProductName);
      Assert.AreEqual(expectedProduct.Description, actualProduct.Description);
      Assert.AreEqual(expectedProduct.Stock, actualProduct.Stock);
      Assert.AreEqual(expectedProduct.Picture, actualProduct.Picture);
      Assert.AreEqual(expectedProduct.Brand, actualProduct.Brand);
      Assert.AreEqual(expectedProduct.Gender, actualProduct.Gender);
      Assert.AreEqual(expectedProduct.AgeGroup, actualProduct.AgeGroup);
    }

    [Test]
    public void GetProductNo6_AssertAsString_Should_Be_Equal()
    {
      var actualProduct = CallApi().GetAsync(GetProductEndPoint + "?productId=6").Result.Content.ReadAsStringAsync().Result;
      var expectedProduct = JsonConvert.SerializeObject(new List<Domain.Model.Product> { GetSampleProduct()[2] });
      Assert.AreEqual(expectedProduct, actualProduct);
    }
  }
}