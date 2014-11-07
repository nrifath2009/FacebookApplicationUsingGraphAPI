using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FacebookApplicationUsingGraphAPI.Startup))]
namespace FacebookApplicationUsingGraphAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
