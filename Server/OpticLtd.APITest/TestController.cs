using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using OpticLtd.Api;
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
        }
      };
      return products;
    }

    string GetProductEndPoint { get { return "api/product/GetProduct"; } }

    private async Task<HttpResponseMessage> CallApi(string route)
    {
      var waf = new WebApplicationFactory<Startup>();
      var client = waf.CreateDefaultClient();
      return await client.GetAsync(route);
    }

    [Test]
    public void GetProductApiStatusCode_Should_Be_Ok()
    {
      CallApi(GetProductEndPoint).Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public void GetProductsAssert_ShouldBe_Equal()
    {
      var read = CallApi(GetProductEndPoint).Result.Content.ReadAsStringAsync().Result;
      var actualProducts = JsonConvert.DeserializeObject<List<Domain.Model.Product>>(read);
      var expectedProducts = GetSampleProduct();

      for (int i = 0; i < actualProducts.Count; i++)
      {
        Assert.AreEqual(expectedProducts[i].ProductId, actualProducts[i].ProductId);
        Assert.AreEqual(expectedProducts[i].ProductCategory, actualProducts[i].ProductCategory);
        Assert.AreEqual(expectedProducts[i].ProductName, actualProducts[i].ProductName);
        Assert.AreEqual(expectedProducts[i].Description, actualProducts[i].Description);
        Assert.AreEqual(expectedProducts[i].Stock, actualProducts[i].Stock);
        Assert.AreEqual(expectedProducts[i].Picture, actualProducts[i].Picture);
        Assert.AreEqual(expectedProducts[i].Brand, actualProducts[i].Brand);
        Assert.AreEqual(expectedProducts[i].Gender, actualProducts[i].Gender);
        Assert.AreEqual(expectedProducts[i].AgeGroup, actualProducts[i].AgeGroup);
      }
    }

    [Test]
    public void GetProductAssertNo6_ShouldBe_Equal()
    {
      var read = CallApi(GetProductEndPoint + "?Id=6").Result.Content.ReadAsStringAsync().Result;
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

    public void GetProductNo6_AssertAsString_Should_Be_Equal()
    {
      var actualProduct = CallApi(GetProductEndPoint + "?Id=6").Result.Content.ReadAsStringAsync().Result;
      var expectedProduct = JsonConvert.SerializeObject(GetSampleProduct()[2]);
      Assert.AreEqual(expectedProduct, actualProduct);
    }
  }
}