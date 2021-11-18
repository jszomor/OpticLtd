using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using OpticLtd.Api;
using OpticLtd.BusinessLogic.Mediator;
using Microsoft.Extensions.DependencyInjection;
using OpticLtd.BusinessLogic.Product.Queries;

namespace OpticLtd.APITest
{
  public class StartupMock : WebApplicationFactory<Startup>
  {
    public WebApplicationFactory<Startup> StartupInstance()
    {
      return WithWebHostBuilder(builder =>
      {
        builder.ConfigureServices(services =>
        {
          services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
          services.AddAutoMapper(typeof(Startup));
          services.AddMediatR(typeof(GetProduct));
          services.AddControllers();
        });
      });
    }
  }
}
