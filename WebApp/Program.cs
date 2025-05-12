using AspNetStatic;
using AspNetStaticContrib.AspNetStatic;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression(options => { options.EnableForHttps = true; });
builder.Services.AddRazorPages();
builder.Services.AddCors(options =>
{
    options.AddPolicy("lb", builder =>
    {
        builder.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback || origin.ToLower().Contains("dmd.shaluth.com")).AllowAnyHeader();
    });
});
var staticOnly = Environment.UserName.Contains("buildbot");
//staticOnly = true;
if (staticOnly)
{
    var pages = new List<ResourceInfoBase> { };
    builder.Services.AddSingleton<IStaticResourcesInfoProvider>(
        new StaticResourcesInfoProvider()
            .AddAllProjectRazorPages(builder.Environment)
            .AddAllWebRootContent(builder.Environment)
            .Add(new CssResource("/WebApp.styles.css")));
}
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.UseCors("lb");
app.MapRazorPages().WithStaticAssets();

if (staticOnly)
{
    Directory.CreateDirectory(@"bin\static");
    app.GenerateStaticContent(@"bin\static", true);
    await app.RunAsync();
    Environment.Exit(0);
}

app.Run();
