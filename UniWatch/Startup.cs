using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UniWatch.Startup))]
namespace UniWatch
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
