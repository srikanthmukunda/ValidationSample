using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ValidationSample.Startup))]

namespace ValidationSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.MapSignalR();
        }
    }
}
