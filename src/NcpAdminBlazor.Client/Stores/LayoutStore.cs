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
    } = true;

    public event Action<bool>? OnDarkModeChanged;
}