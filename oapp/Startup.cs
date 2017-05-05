using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(oapp.Startup))]
namespace oapp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
