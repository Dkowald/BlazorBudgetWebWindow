namespace BlazorBudgetWebWindow {

    using WebWindows.Blazor;

    public class Program {

        public static void Main() {
            ComponentsDesktop.Run<Startup>("Blazor Budget", "wwwroot/index.html");
        }
    }
}
