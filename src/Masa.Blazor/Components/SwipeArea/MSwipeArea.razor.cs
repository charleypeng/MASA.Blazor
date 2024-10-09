
namespace Masa.Blazor;

public partial class MSwipeArea : MasaComponentBase
{
    //private fields
    private double? _xDown;
    private double? _yDown;
    private ScreenStatus? _screenStatus;
    [Inject]
    private IScreenService ScreenService { get; set; } = null!;

    //parameters
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    /// <summary>
    /// action on swipe gesture is invoked
    /// </summary>
    [Parameter]
    public Action<SwipeDirection> OnSwipe { get; set; } = null!;
    /// <summary>
    /// sensitive area on user touched
    /// </summary>
    [Parameter]
    public double? TouchSensitiveArea { get; set; } = 0.1;
    /// <summary>
    /// set area for user where is the end to invoke the swipe action 
    /// </summary>
    [Parameter]
    public double? SwipeInvokeArea { get; set; } = 0.5;
    //implementation
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (!(TouchSensitiveArea > 0 && TouchSensitiveArea < 1) | !(SwipeInvokeArea > 0 && SwipeInvokeArea < 1))
        {
            throw new Exception("swipe area parameter should between 0 and 1");
        }
    }
    private async void OnTouchStart(TouchEventArgs args)
    {
        //update screen width and height first on touch to prevent screen rotation
        await UpdateSreenStatus();
        //update finger touch start x and y
        _xDown = args.Touches[0].ClientX;
        _yDown = args.Touches[0].ClientY;
    }

    private async Task UpdateSreenStatus()
    {
        _screenStatus = await ScreenService.GetScreenStatusAsync();
    }

    private void OnTouchEnd(TouchEventArgs args)
    {
        if (_xDown == null || _yDown == null || _screenStatus == null || _screenStatus.Width == null)
        {
            return;
        }

        var xDiff = _xDown.Value - args.ChangedTouches[0].ClientX;
        var yDiff = _yDown.Value - args.ChangedTouches[0].ClientY;

        if (Math.Abs(xDiff) < _screenStatus.Width * TouchSensitiveArea && Math.Abs(yDiff) < _screenStatus.Height * TouchSensitiveArea)
        {
            _xDown = null;
            _yDown = null;
            return;
        }

        if (Math.Abs(xDiff) > Math.Abs(yDiff))
        {
            if (xDiff < -(_screenStatus.Width * SwipeInvokeArea) && _xDown <= _screenStatus.Width * TouchSensitiveArea)
            {
                InvokeAsync(() => OnSwipe(SwipeDirection.LeftToRight));
            }
            else if (xDiff > (_screenStatus.Width * SwipeInvokeArea) && (_screenStatus.Width - _xDown) <= _screenStatus.Width * TouchSensitiveArea)
            {
                InvokeAsync(() => OnSwipe(SwipeDirection.RightToLeft));
            }
        }
      
        _xDown = null;
        _yDown = null;
    }

    private void OnTouchCancel(TouchEventArgs args)
    {
        _xDown = null;
        _yDown = null;
        _screenStatus = null;
    }
}
