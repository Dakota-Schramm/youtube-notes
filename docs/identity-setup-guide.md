# Adding ASP.NET Core Identity to youtube-notes

Step-by-step guide for adding password-based authentication using ASP.NET Core Identity.

**Docs**: [Introduction to Identity on ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0)

---

## Step 1: Install NuGet Packages

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

**Docs**: [Identity package reference](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#adddefaultidentity-and-addidentity)

---

## Step 2: Update the DbContext

Change `youtube_notesContext` to inherit from `IdentityDbContext` instead of `DbContext`. This adds the Identity tables (Users, Roles, Claims, etc.).

**File**: `Data/youtube_notesContext.cs`

```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using youtube_notes.Models;

namespace youtube_notes.Data
{
    public class youtube_notesContext : IdentityDbContext
    {
        public youtube_notesContext(DbContextOptions<youtube_notesContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Note { get; set; } = default!;
    }
}
```

**Docs**: [IdentityDbContext](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore.identitydbcontext?view=aspnetcore-9.0)

---

## Step 3: Register Identity Services in Program.cs

Add Identity services and configure cookie authentication. This goes **before** `builder.Build()`.

**File**: `Program.cs`

```csharp
// Add after builder.Services.AddControllersWithViews();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<youtube_notesContext>();
```

Then add the authentication/authorization middleware **after** `app.UseRouting()` (or after `app.UseHttpsRedirection()`), and **before** `app.MapControllerRoute()`:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Add this using statement at the top of `Program.cs`:

```csharp
using Microsoft.AspNetCore.Identity;
```

**Docs**: [Configure Identity services](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#configure-identity-services)

---

## Step 4: Create and Apply the Migration

```bash
dotnet ef migrations add AddIdentity
dotnet ef database update
```

This creates the Identity tables: `AspNetUsers`, `AspNetRoles`, `AspNetUserClaims`, `AspNetUserRoles`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`.

**Docs**: [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

---

## Step 5: Scaffold Identity UI Pages (Register, Login, Logout)

Identity ships with default Razor Pages for login/register/logout. You can scaffold them to customize:

```bash
dotnet aspnet-codegenerator identity -dc youtube_notes.Data.youtube_notesContext --files "Account.Register;Account.Login;Account.Logout;Account.Manage.Index"
```

This generates pages under `Areas/Identity/Pages/Account/`.

Alternatively, skip scaffolding and rely on the default UI from `Microsoft.AspNetCore.Identity.UI` — it works out of the box with `AddDefaultIdentity`.

**Docs**: [Scaffold Identity in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-9.0)

---

## Step 6: Map Razor Pages for Identity UI

Identity UI uses Razor Pages. Add this to `Program.cs` alongside your existing MVC route mapping:

```csharp
app.MapRazorPages();
```

This is required for the login/register/logout pages to work.

---

## Step 7: Add Login/Logout Partial to Layout

Add a login partial to `_Layout.cshtml` so users can see login/register/logout links in the navbar.

**File**: `Views/Shared/_LoginPartial.cshtml` (new file)

```html
@using Microsoft.AspNetCore.Identity
@inject SignInManager<IdentityUser> SignInManager
@inject UserManager<IdentityUser> UserManager

<ul class="navbar-nav">
@if (SignInManager.IsSignedIn(User))
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Manage/Index"
           title="Manage">Hello @User.Identity?.Name!</a>
    </li>
    <li class="nav-item">
        <form class="form-inline" asp-area="Identity" asp-page="/Account/Logout"
              asp-route-returnUrl="@Url.Action("Index", "Home", new { area = "" })">
            <button type="submit" class="nav-link btn btn-link text-dark">Logout</button>
        </form>
    </li>
}
else
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Register">Register</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="Identity" asp-page="/Account/Login">Login</a>
    </li>
}
</ul>
```

Then reference it in `_Layout.cshtml` inside the navbar:

```html
<partial name="_LoginPartial" />
```

**Docs**: [Login partial view](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#log-in)

---

## Step 8: Protect Controllers with [Authorize]

Add the `[Authorize]` attribute to controllers or actions that require login:

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class NotesController : Controller
{
    // All actions now require authentication
}

// Or on individual actions:
public class SearchController : Controller
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(...)
    {
        // Only logged-in users can create notes
    }
}
```

**Docs**: [Simple authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/simple?view=aspnetcore-9.0)

---

## Step 9: Associate Notes with Users (Optional but Recommended)

Add a `UserId` foreign key to the `Note` model so each note belongs to a user:

**File**: `Models/Note.cs`

```csharp
public class Note
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public string YoutubeId { get; set; }
    public int TimeAt { get; set; }
    public string UserId { get; set; }  // FK to AspNetUsers
}
```

Then in controllers, filter notes by the logged-in user:

```csharp
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
var notes = _context.Note.Where(n => n.UserId == userId);
```

Run a migration after adding the column:

```bash
dotnet ef migrations add AddUserIdToNote
dotnet ef database update
```

---

## Summary of Changes

| File | Change |
|---|---|
| `youtube-notes.csproj` | Add Identity NuGet packages |
| `Data/youtube_notesContext.cs` | Inherit from `IdentityDbContext` |
| `Program.cs` | Register Identity services, add auth middleware, map Razor Pages |
| `Views/Shared/_LoginPartial.cshtml` | New file — login/logout/register links |
| `Views/Shared/_Layout.cshtml` | Add `<partial name="_LoginPartial" />` |
| `Controllers/NotesController.cs` | Add `[Authorize]` attribute |
| `Controllers/SearchController.cs` | Add `[Authorize]` to write actions |
| `Models/Note.cs` | Add `UserId` property (optional) |
| `Migrations/` | New migration for Identity tables |

## Key Documentation Links

- [ASP.NET Core Identity overview](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0)
- [Scaffold Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-9.0)
- [Configure Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-9.0)
- [Authorization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-9.0)
- [Account confirmation and password recovery](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-9.0)
