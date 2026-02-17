using Microsoft.Extensions.Localization;

namespace EHRS.Api.Localization;

public sealed class AppLocalizer : IAppLocalizer
{
    private readonly IStringLocalizer _localizer;

    public AppLocalizer(IStringLocalizerFactory factory)
    {
        // ✅ With ResourcesPath="Resources" => baseName is "Messages"
        _localizer = factory.Create("Messages", "EHRS.Api");
    }

    public string this[string key] => _localizer[key].Value;
}
