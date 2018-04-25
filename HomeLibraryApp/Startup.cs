using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeLibraryApp.Startup))]
namespace HomeLibraryApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
