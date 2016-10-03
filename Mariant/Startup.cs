using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mariant.Startup))]
namespace Mariant
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
