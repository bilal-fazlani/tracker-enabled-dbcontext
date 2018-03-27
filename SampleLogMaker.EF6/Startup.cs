using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SampleLogMaker.Startup))]
namespace SampleLogMaker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
