using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RecViewerApp.Startup))]
namespace RecViewerApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
