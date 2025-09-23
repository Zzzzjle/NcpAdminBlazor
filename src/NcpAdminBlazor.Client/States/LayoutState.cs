namespace NcpAdminBlazor.Client.States;

public class LayoutState
{
    public bool IsDarkMode
    {
        get;
        set
        {
            field = value;
            OnDarkModeChanged?.Invoke(field);
        }
    } = true;

    public event Action<bool>? OnDarkModeChanged;
}