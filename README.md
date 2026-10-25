# WebLoginLab

一个用于学习 ASP.NET Core 网页认证的练习项目，通过 Razor Pages 和 Cookie 认证，实现登录、退出登录及基于角色的页面访问控制。

重点研究：浏览器提交表单 → 后端验证 → 签发认证 Cookie → 后续请求恢复身份 → 检查页面访问权限。

## 技术与环境

- C# / ASP.NET Core 10
- Razor Pages：服务器生成 HTML，并处理页面请求
- Cookie 认证与 Claims 角色声明
- Bootstrap：模板自带的页面样式
- 内存 Dictionary：保存固定演示账户，无需数据库

需要安装 .NET SDK。`global.json` 指定 **10.0.402**，并允许同一功能带中的更新补丁版本（`latestPatch`）。建议使用支持 .NET 10 的 Visual Studio，或配置了 C# 开发支持的其他编辑器。

## 本地运行

在项目目录打开终端，首次运行时信任本机 HTTPS 开发证书：

```shell
dotnet dev-certs https --trust
```

启动服务：

```shell
dotnet run --launch-profile https
```

浏览器访问：**https://localhost:7220/**。保持终端运行，按 `Ctrl+C` 停止服务。

开发时可以使用文件监视与热重载：

```shell
dotnet watch run --launch-profile https
```

端口配置位于 `Properties/launchSettings.json`。请使用 HTTPS 地址，认证 Cookie 配置为仅通过 HTTPS 发送；`http://localhost:7220` 的协议与端口不匹配。

## 演示账户

以下为代码中公开的本地测试凭据，用户名区分大小写：

| 用户名 | 密码 | 角色 |
| --- | --- | --- |
| `Admin` | `admin114514` | `Admin` |
| `USER1` | `user1919810` | `User` |

账户定义在 `Pages/Login.cshtml.cs` 中。明文密码与固定账户仅用于学习，真实账户系统应使用密码哈希验证等机制。

## 页面与访问规则

| 路径 | 用途 | 访问要求 |
| --- | --- | --- |
| `/` | 首页 | 无需登录 |
| `/login` | 登录表单 | 无需登录 |
| `/private` | 个人页面与退出操作 | 已登录 |
| `/admin` | 管理员页面 | 具有 `Admin` 角色 |

未登录访问受保护页面时，服务器重定向到 `/login`，并携带 `ReturnUrl`。登录成功后只允许跳回本站地址，默认进入 `/private`。已登录但角色不满足要求时，按当前 `AccessDeniedPath` 配置返回首页。

退出使用 `/private` 页面的 POST `Logout` handler，清除认证 Cookie 后返回首页。

## 代码阅读入口

```text
Program.cs                  服务注册、Cookie 配置、请求处理管道
Pages/Login.cshtml          登录表单、验证错误摘要
Pages/Login.cshtml.cs       模型绑定、账户验证、身份构造、Cookie 签发
Pages/Private.cshtml.cs     [Authorize] 与退出登录处理
Pages/Admin.cshtml.cs       [Authorize(Roles = "Admin")] 角色检查
Pages/_ViewImports.cshtml   Razor 命名空间及 Tag Helper 配置
wwwroot/                   页面样式、脚本及模板前端库
```

登录时，后端将用户标识、用户名和角色组成 `ClaimsPrincipal`，通过 `SignInAsync()` 签发认证 Cookie。后续请求由认证中间件恢复 `HttpContext.User`，再由授权中间件检查页面要求。

认证票据默认有效期为 30 分钟，并启用滑动续期。角色保存在签发时的票据中；修改演示账户角色后，应退出并重新登录以获得新的角色声明。

## 手动验证

1. 在未登录状态访问 `/private`，应跳转到登录页。
2. 分别输入不存在的账户、存在账户的错误密码，应显示错误并保持未登录。
3. 使用 `USER1` 登录，能访问 `/private`，访问 `/admin` 时返回首页。
4. 退出后再次访问 `/private`，应重新要求登录。
5. 使用 `Admin` 登录，应能访问 `/admin`。

可在浏览器开发者工具的 Network 面板启用 Preserve log，观察表单 POST、302 重定向、响应的 `Set-Cookie` 和后续请求的 `Cookie`。测试未登录场景时，先退出或使用新的无痕会话。

## 后续学习方向

- 增加受保护的 JSON API，比较 HTML 表单与 `fetch` 请求。
- 将账户验证逻辑从页面模型中提取出来。
- 接入 ASP.NET Core Identity，实现真实用户和密码管理。
- 学习授权策略、会话撤销，以及角色变更后旧票据的处理。
