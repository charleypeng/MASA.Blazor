namespace Masa.Blazor;

/// <summary>
/// screen service implementation for webview based app,
/// if you need another service please implement your own IScreenService
/// </summary>
public class ScreenService(IJSRuntime js) : IScreenService
{
    private readonly IJSRuntime Js = js;
    public async Task<ScreenStatus?> GetScreenStatusAsync()
    {
        var module = await Js.InvokeAsync<IJSObjectReference>("import", "/_content/Masa.Blazor/js/screenhelper.js");
        return await module.InvokeAsync<ScreenStatus?>("getDemensions");
    }
}