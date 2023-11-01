using System;
using System.Reflection;
using System.Threading.Tasks;
using Onova;
using Onova.Services;

namespace Fossa.Client.Desktop.Services.Updater;

public class GithubUpdater
{
    private readonly IUpdateManager _updateManager = new UpdateManager(
        new GithubPackageResolver(
                "libremindsph",
            "fossa-client-desktop",
            "fossa-standalone-*.zip"
        ),
        new ZipPackageExtractor()
    );

    private Version? _latestVersion;

    public string GetLatestVersion()
    {
        return $"{_latestVersion!.Major}.{_latestVersion.Minor}.{_latestVersion.Build}";
    }

    public async Task<UpdateStatus> CheckForUpdates()
    {
        var result = await _updateManager.CheckForUpdatesAsync();
        if (result.LastVersion is null) return UpdateStatus.NoAsset;

        var readCurrentVersion = ReadCurrentVersion();

        if (readCurrentVersion is null) return UpdateStatus.Error;

        _latestVersion = result.LastVersion;

        return ShouldUpdate(readCurrentVersion, _latestVersion) ? UpdateStatus.Available : UpdateStatus.NotAvailable;
    }

    public async Task PerformUpdate(IProgress<double> progress)
    {
        if (_latestVersion is null) return;
        await _updateManager.PrepareUpdateAsync(_latestVersion, progress);
        await Task.Run(() => _updateManager.LaunchUpdater(_latestVersion));
        Environment.Exit(0);
    }

    public static Version? ReadCurrentVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version;
    }

    private static bool ShouldUpdate(Version currentVersion, Version latestVersion)
    {
        return latestVersion > currentVersion;
    }
}