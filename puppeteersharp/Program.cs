CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
var server = new Server();
#if DEBUG
server.UseHotReload();
#endif
server.AddAppsFromAssembly();
server.AddConnectionsFromAssembly();
var chromeSettings = new ChromeSettings()

    .UseTabs(preventDuplicates: true)
    ;

server.UseChrome(chromeSettings);
var fetcher = new BrowserFetcher();
await fetcher.DownloadAsync();

await server.RunAsync();
