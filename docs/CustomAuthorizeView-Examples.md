# CustomAuthorizeView 使用示例

本文档提供 CustomAuthorizeView 组件的实际使用示例。

## 基础示例

### 示例 1: 简单的登录检查

```razor
@page "/secure-page"
@using NcpAdminBlazor.Client.Component

<h3>安全页面</h3>

<CustomAuthorizeView>
    <Authorized>
        <p>欢迎回来，@context.User.Identity?.Name！</p>
        <p>这是只有登录用户才能看到的内容。</p>
    </Authorized>
    <NotAuthorized>
        <p>请先登录才能查看此页面。</p>
        <a href="/login">点击这里登录</a>
    </NotAuthorized>
    <Authorizing>
        <p>正在验证您的身份...</p>
    </Authorizing>
</CustomAuthorizeView>
```

### 示例 2: 基于角色的权限控制

```razor
@page "/admin-panel"
@using NcpAdminBlazor.Client.Component

<h3>管理面板</h3>

<CustomAuthorizeView Roles="Admin,SuperAdmin">
    <Authorized>
        <div>
            <h4>管理员工具</h4>
            <ul>
                <li><a href="/admin/users">用户管理</a></li>
                <li><a href="/admin/roles">角色管理</a></li>
                <li><a href="/admin/settings">系统设置</a></li>
            </ul>
        </div>
    </Authorized>
    <NotAuthorized>
        <MudAlert Severity="Severity.Warning">
            您没有权限访问管理面板。需要 Admin 或 SuperAdmin 角色。
        </MudAlert>
    </NotAuthorized>
</CustomAuthorizeView>
```

### 示例 3: 嵌套权限控制

```razor
@page "/dashboard"
@using NcpAdminBlazor.Client.Component

<h3>控制面板</h3>

<CustomAuthorizeView>
    <Authorized>
        <MudGrid>
            <MudItem xs="12" md="6">
                <MudCard>
                    <MudCardHeader>
                        <CardHeaderContent>
                            <MudText Typo="Typo.h6">基础信息</MudText>
                        </CardHeaderContent>
                    </MudCardHeader>
                    <MudCardContent>
                        <p>所有登录用户都能看到这部分内容</p>
                    </MudCardContent>
                </MudCard>
            </MudItem>

            <!-- 管理员专属区域 -->
            <MudItem xs="12" md="6">
                <CustomAuthorizeView Roles="Admin">
                    <Authorized>
                        <MudCard>
                            <MudCardHeader>
                                <CardHeaderContent>
                                    <MudText Typo="Typo.h6">管理员统计</MudText>
                                </CardHeaderContent>
                            </MudCardHeader>
                            <MudCardContent>
                                <p>只有管理员能看到这部分内容</p>
                                <MudChip Color="Color.Primary">Admin Only</MudChip>
                            </MudCardContent>
                        </MudCard>
                    </Authorized>
                    <NotAuthorized>
                        <MudCard>
                            <MudCardContent>
                                <MudText Color="Color.Secondary">
                                    此区域需要管理员权限
                                </MudText>
                            </MudCardContent>
                        </MudCard>
                    </NotAuthorized>
                </CustomAuthorizeView>
            </MudItem>
        </MudGrid>
    </Authorized>
    <NotAuthorized>
        <RedirectToLogin />
    </NotAuthorized>
</CustomAuthorizeView>
```

### 示例 4: 组件中条件渲染按钮

```razor
@using NcpAdminBlazor.Client.Component

<MudCard>
    <MudCardHeader>
        <CardHeaderContent>
            <MudText Typo="Typo.h6">文章标题</MudText>
        </CardHeaderContent>
    </MudCardHeader>
    <MudCardContent>
        <p>文章内容...</p>
    </MudCardContent>
    <MudCardActions>
        <!-- 只有作者和管理员能看到编辑按钮 -->
        <CustomAuthorizeView Roles="Author,Admin">
            <Authorized>
                <MudButton Variant="Variant.Text" Color="Color.Primary">
                    编辑
                </MudButton>
            </Authorized>
        </CustomAuthorizeView>

        <!-- 只有管理员能看到删除按钮 -->
        <CustomAuthorizeView Roles="Admin">
            <Authorized>
                <MudButton Variant="Variant.Text" Color="Color.Error">
                    删除
                </MudButton>
            </Authorized>
        </CustomAuthorizeView>

        <!-- 所有人都能看到的按钮 -->
        <MudButton Variant="Variant.Text">
            分享
        </MudButton>
    </MudCardActions>
</MudCard>
```

## 高级示例

### 示例 5: 使用 AuthService 进行程序化权限检查

```razor
@page "/user-profile"
@using NcpAdminBlazor.Client.Component
@using NcpAdminBlazor.Client.Services
@inject AuthService AuthService

<h3>用户资料</h3>

<CustomAuthorizeView>
    <Authorized>
        <MudCard>
            <MudCardContent>
                <MudText>用户名：@_userName</MudText>
                <MudText>角色：@_userRole</MudText>
                
                @if (_canEdit)
                {
                    <MudButton Color="Color.Primary" OnClick="EditProfile">
                        编辑资料
                    </MudButton>
                }
            </MudCardContent>
        </MudCard>
    </Authorized>
</CustomAuthorizeView>

@code {
    private string _userName = "";
    private string _userRole = "";
    private bool _canEdit = false;

    protected override async Task OnInitializedAsync()
    {
        var user = await AuthService.GetUserAsync();
        _userName = user?.Identity?.Name ?? "未知";
        
        // 程序化检查角色
        _canEdit = await AuthService.IsInRoleAsync("Admin") 
                   || await AuthService.IsInRoleAsync("User");
        
        if (await AuthService.IsInRoleAsync("Admin"))
        {
            _userRole = "管理员";
        }
        else if (await AuthService.IsInRoleAsync("User"))
        {
            _userRole = "普通用户";
        }
    }

    private async Task EditProfile()
    {
        // 编辑逻辑
    }
}
```

### 示例 6: 导航菜单中的权限控制

```razor
@using NcpAdminBlazor.Client.Component

<MudNavMenu>
    <!-- 所有人都能看到的菜单 -->
    <MudNavLink Href="/" Icon="@Icons.Material.Filled.Home">
        首页
    </MudNavLink>

    <!-- 需要登录才能看到 -->
    <CustomAuthorizeView>
        <Authorized>
            <MudNavLink Href="/dashboard" Icon="@Icons.Material.Filled.Dashboard">
                控制台
            </MudNavLink>
            <MudNavLink Href="/profile" Icon="@Icons.Material.Filled.Person">
                个人中心
            </MudNavLink>
        </Authorized>
    </CustomAuthorizeView>

    <!-- 只有管理员能看到 -->
    <CustomAuthorizeView Roles="Admin">
        <Authorized>
            <MudNavGroup Title="管理" Icon="@Icons.Material.Filled.Settings">
                <MudNavLink Href="/admin/users" Icon="@Icons.Material.Filled.People">
                    用户管理
                </MudNavLink>
                <MudNavLink Href="/admin/roles" Icon="@Icons.Material.Filled.Security">
                    角色管理
                </MudNavLink>
            </MudNavGroup>
        </Authorized>
    </CustomAuthorizeView>
</MudNavMenu>
```

### 示例 7: 表格中的行级权限

```razor
@using NcpAdminBlazor.Client.Component
@using NcpAdminBlazor.Client.Services
@inject AuthService AuthService

<MudTable Items="@_items">
    <HeaderContent>
        <MudTh>名称</MudTh>
        <MudTh>状态</MudTh>
        <MudTh>操作</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.Name</MudTd>
        <MudTd>@context.Status</MudTd>
        <MudTd>
            <MudButtonGroup>
                <!-- 作者或管理员可以编辑 -->
                <CustomAuthorizeView Roles="Author,Admin">
                    <Authorized>
                        <MudIconButton Icon="@Icons.Material.Filled.Edit" 
                                      Size="Size.Small" 
                                      Color="Color.Primary" />
                    </Authorized>
                </CustomAuthorizeView>

                <!-- 只有管理员可以删除 -->
                <CustomAuthorizeView Roles="Admin">
                    <Authorized>
                        <MudIconButton Icon="@Icons.Material.Filled.Delete" 
                                      Size="Size.Small" 
                                      Color="Color.Error" />
                    </Authorized>
                </CustomAuthorizeView>
            </MudButtonGroup>
        </MudTd>
    </RowTemplate>
</MudTable>

@code {
    private List<ItemModel> _items = new();

    public class ItemModel
    {
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
```

## 布局组件示例

### 示例 8: 主布局中使用 @key 优化

```razor
@inherits LayoutComponentBase
@using NcpAdminBlazor.Client.Component

<CustomAuthorizeView>
    <Authorized>
        @{
            // 提取用户标识作为 key
            var userId = context.User?.FindFirst("sub")?.Value 
                         ?? context.User?.FindFirst("UserId")?.Value 
                         ?? "authenticated";
        }
        
        <!-- 使用 @key 确保只在用户变化时重新渲染 -->
        <MudLayout @key="@userId">
            <MudAppBar>
                <MudText Typo="Typo.h6">
                    欢迎，@context.User.Identity?.Name
                </MudText>
                <MudSpacer />
                <MudIconButton Icon="@Icons.Material.Filled.Logout" 
                              OnClick="Logout" 
                              Color="Color.Inherit" />
            </MudAppBar>
            
            <MudMainContent>
                @Body
            </MudMainContent>
        </MudLayout>
    </Authorized>
    
    <NotAuthorized>
        <RedirectToLogin />
    </NotAuthorized>
    
    <Authorizing>
        <MudProgressCircular Indeterminate="true" />
    </Authorizing>
</CustomAuthorizeView>

@code {
    private void Logout()
    {
        // 登出逻辑
    }
}
```

## 性能对比

### 使用 AuthorizeView（原始方式）
```razor
<!-- 每次认证状态变化都会导致整个布局重新渲染 -->
<AuthorizeView>
    <Authorized>
        <MudLayout>
            <!-- 整个布局树都会重新创建 -->
            <MudAppBar>...</MudAppBar>
            <MudDrawer>...</MudDrawer>
            <MudMainContent>...</MudMainContent>
        </MudLayout>
    </Authorized>
</AuthorizeView>
```

### 使用 CustomAuthorizeView + @key（优化后）
```razor
<!-- 只有在用户真正变化时才重新渲染 -->
<CustomAuthorizeView>
    <Authorized>
        @{
            var userId = context.User?.FindFirst("sub")?.Value ?? "authenticated";
        }
        <MudLayout @key="@userId">
            <!-- 同一用户的状态变化不会导致重新创建 -->
            <MudAppBar>...</MudAppBar>
            <MudDrawer>...</MudDrawer>
            <MudMainContent>...</MudMainContent>
        </MudLayout>
    </Authorized>
</CustomAuthorizeView>
```

## 最佳实践

1. **在布局组件中使用 @key**：确保用户身份变化时才重新创建组件实例
2. **嵌套权限检查**：可以自由嵌套多层 CustomAuthorizeView
3. **组合使用 AuthService**：对于复杂的权限逻辑，在代码中使用 AuthService
4. **避免过度使用**：对于简单的显示/隐藏，直接使用条件渲染可能更简单
5. **保持一致性**：在整个应用中统一使用 CustomAuthorizeView

## 迁移清单

从 AuthorizeView 迁移到 CustomAuthorizeView：

- [ ] 1. 在 Program.cs 中注册 AuthService
- [ ] 2. 更新主布局组件 (AppLayout.razor)
- [ ] 3. 添加 @key 指令到主布局的 MudLayout
- [ ] 4. 在需要权限控制的组件中使用 CustomAuthorizeView
- [ ] 5. 测试登录/登出流程
- [ ] 6. 验证页面不会出现不必要的重新渲染
- [ ] 7. 检查性能改进效果

## 故障排除

### 问题：组件没有响应认证状态变化
**解决**：确保 AuthService 已在 Program.cs 中注册

### 问题：Roles 参数不工作
**解决**：检查用户的 Claims 中是否包含正确的角色信息

### 问题：页面仍然频繁重新渲染
**解决**：确保在主布局中使用了 @key 指令，并且 key 的值是稳定的用户标识

### 问题：编译错误 - 找不到 CustomAuthorizeView
**解决**：确保添加了正确的 using 语句：`@using NcpAdminBlazor.Client.Component`
