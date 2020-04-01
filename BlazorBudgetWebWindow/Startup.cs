using WebWindows.Blazor;

namespace BlazorBudgetWebWindow {
    using Microsoft.Extensions.DependencyInjection;

    public class Startup {

        public void Configure(DesktopApplicationBuilder app) {
            app.AddComponent<App>("app");
        }

        public void ConfigureServices(IServiceCollection services) {
        }
    }
}
