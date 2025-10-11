namespace NcpAdminBlazor.Client.Services;

public interface IAuthTokenService
{
    event EventHandler? TokensCleared;

    /// <summary>
    /// 获取当前令牌快照，包含访问令牌、刷新令牌及相关元数据。
    /// </summary>
    ValueTask<TokenStorageSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前访问令牌，如果不存在或已失效则返回 null。
    /// </summary>
    ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取刷新令牌，用于续期访问令牌。
    /// </summary>
    ValueTask<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 持久化令牌及其元数据。
    /// </summary>
    ValueTask SetTokensAsync(TokenStorageSnapshot snapshot, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清除所有令牌信息。
    /// </summary>
    ValueTask ClearTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 尝试使用刷新令牌续期访问令牌。
    /// </summary>
    ValueTask<bool> TryRefreshTokenAsync(CancellationToken cancellationToken = default);
}