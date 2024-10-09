
namespace Masa.Blazor;

public interface IScreenService
{
    Task<ScreenStatus?> GetScreenStatusAsync();
}
