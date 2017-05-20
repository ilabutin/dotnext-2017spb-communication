using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Owin.Hosting;
using Owin;

namespace DotNext
{
  public class WebApiServer : IServer
  {
    public const string baseAddress = "http://*:22000/";
    private IDisposable webApp;

    public void Dispose()
    {
      webApp?.Dispose();
    }

    public void Start()
    {
      // Start OWIN host 
      webApp = WebApp.Start<Startup>(url: baseAddress);

    }
  }

  public class ReplyController : ApiController
  {
    protected override void Initialize(HttpControllerContext controllerContext)
    {
      base.Initialize(controllerContext);
    }

    public int Get()
    {
      return 0;
    }

    public HttpResponseMessage Post()
    {
      var body = Request.Content.ReadAsByteArrayAsync().Result;
      var reply = ServerLogic.Convert(body.ConvertTo<InputData>());
      return new HttpResponseMessage() {Content = new ByteArrayContent(ByteArray.CreateFrom(reply))};
    }
  }

  public class Startup
  {
    // This code configures Web API. The Startup class is specified as a type
    // parameter in the WebApp.Start method.
    public void Configuration(IAppBuilder appBuilder)
    {
      // Configure Web API for self-host. 
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
      );

      appBuilder.UseWebApi(config);
    }
  }
}
