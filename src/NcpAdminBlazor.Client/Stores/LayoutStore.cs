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
}