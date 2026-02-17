namespace EHRS.Api.Localization;

public interface IAppLocalizer
{
    string this[string key] { get; }
}
