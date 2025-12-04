namespace NcpAdminBlazor.Client.Stores;

public class LayoutStore
{
    public bool IsDarkMode
    {
        get;
        set
        {
            field = value;
            OnDarkModeChanged?.Invoke(field);
        }
    } = false;

    public event Action<bool>? OnDarkModeChanged;

    /// <summary>
    /// 内容区全屏模式（隐藏侧边栏和顶部导航栏）
    /// </summary>
    public bool IsContentFullscreen
    {
        get;
        private set
        {
            field = value;
            OnContentFullscreenChanged?.Invoke(field);
        }
    } = false;

    public event Action<bool>? OnContentFullscreenChanged;

    public void ToggleContentFullscreen()
    {
        IsContentFullscreen = !IsContentFullscreen;
    }
}