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
  public class ProductApiTest
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
    string UpdateProductEndPoint { get { return "api/product/UpdateProduct"; } }

    public int ProductId { get; set; }

    private static HttpClient CallApi()
    {
      var waf = new WebApplicationFactory<Startup>();
      return waf.CreateDefaultClient();
    }

    private static async Task<int> GetProductId(HttpResponseMessage responseCreate)
    {
      var responseBody = await responseCreate.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<Domain.Model.Product>(responseBody).ProductId;
    }

    private StringContent ConvertProduct(Domain.Model.Product product)
    {
      var productJson = JsonConvert.SerializeObject(product);
      return new StringContent(productJson, System.Text.Encoding.UTF8, "application/json");
    }

    [Test]
    public void IntegrationProductApiTest()
    {
      //create a test product
      var product = GetSampleProduct()[3];
      var responseCreate = CallApi().PostAsync(CreateProductEndPoint, ConvertProduct(product)).Result;
      var actualCreateStatusCode = responseCreate.EnsureSuccessStatusCode().StatusCode;
      ProductId = GetProductId(responseCreate).Result;
      Assert.AreEqual(HttpStatusCode.OK, actualCreateStatusCode);

      //check the inserted test product
      var actualProduct = CallApi().GetAsync(GetProductEndPoint + $"?productId={ProductId}").Result.Content.ReadAsStringAsync().Result;
      product.ProductId = ProductId;
      var expectedProduct = JsonConvert.SerializeObject(new List<Domain.Model.Product> { product });
      Assert.AreEqual(expectedProduct, actualProduct);

      //update the inserted test product
      product.ProductName = "Update test product name";
      product.ProductCategory = "Update test product category";

      var responseUpdate = CallApi().PutAsync(UpdateProductEndPoint, ConvertProduct(product)).Result;
      var actualUpdateStatusCode = responseUpdate.EnsureSuccessStatusCode().StatusCode;
      Assert.AreEqual(HttpStatusCode.OK, actualUpdateStatusCode);

      //check the updated test product
      var actualUpdatedProduct = CallApi().GetAsync(GetProductEndPoint + $"?productId={ProductId}").Result.Content.ReadAsStringAsync().Result;
      product.ProductId = ProductId;
      var expectedUpdatedProduct = JsonConvert.SerializeObject(new List<Domain.Model.Product> { product });
      Assert.AreEqual(expectedUpdatedProduct, actualUpdatedProduct);

      //delete the test product
      var responseDelete = CallApi().DeleteAsync(DeleteProductEndPoint + $"?productId={ProductId}").Result;
      var actualDeleteStatusCode = responseDelete.EnsureSuccessStatusCode().StatusCode;
      Assert.AreEqual(HttpStatusCode.OK, actualDeleteStatusCode);

      //check the deleted test product
      var actualResult = CallApi().GetAsync(GetProductEndPoint + $"?productId={ProductId}").Result.RequestMessage.Content;
      Assert.AreEqual(null, actualResult);
    }

    [Test]
    public void GetProductNo6_AssertAsString_Should_Be_Equal()
    {
      var actualProduct = CallApi().GetAsync(GetProductEndPoint + "?productId=6").Result.Content.ReadAsStringAsync().Result;
      var expectedProduct = JsonConvert.SerializeObject(new List<Domain.Model.Product> { GetSampleProduct()[2] });
      Assert.AreEqual(expectedProduct, actualProduct);
    }

    [Test]
    public void GetProductApiStatusCode_Should_Be_Ok()
    {
      CallApi().GetAsync(GetProductEndPoint).Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
  }
}