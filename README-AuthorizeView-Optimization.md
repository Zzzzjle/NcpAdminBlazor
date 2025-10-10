# 优化 AuthorizeView 重新渲染问题

## 问题
当认证状态发生变化时（如 Token 刷新），`AuthorizeView` 组件会导致整个页面布局重新渲染。

## 解决方案
在 `AppLayout.razor` 中为 `MudLayout` 组件添加 `@key` 指令，使用用户标识作为 key 值。

### 工作原理
- 当用户保持不变时（如 Token 刷新），key 值不变，组件实例保持不变，避免不必要的重新渲染
- 当用户真正改变时（如登录/登出），key 值改变，组件会被重新创建

### 代码更改
```razor
<Authorized>
    @{
        var userKey = context.User?.Identity?.IsAuthenticated == true 
            ? (context.User.FindFirst("sub")?.Value 
               ?? context.User.FindFirst("UserId")?.Value 
               ?? context.User.Identity.Name 
               ?? "authenticated")
            : "anonymous";
    }
    <MudLayout @key="@userKey">
        <!-- 布局内容 -->
    </MudLayout>
</Authorized>
```

### 效果
- ✅ Token 刷新时不会重新渲染整个布局
- ✅ 用户切换时正常重新渲染
- ✅ 无需额外的服务或组件
- ✅ 最小化代码变更
