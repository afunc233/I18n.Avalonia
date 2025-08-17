using System.IO;
using System.Linq;
using System.Reflection;

namespace I18n.Avalonia.TranslatorProviders.Json;

public class DefaultEmbeddedJsonI18nProvider(string resourceKey, Assembly? assembly = null) : AbsJsonI18nProvider
{
    private readonly Assembly _assembly = assembly ?? Assembly.GetCallingAssembly();

    public override void FillValues()
    {
        var manifestResourceNames = _assembly.GetManifestResourceNames();

        foreach (var resourceName in manifestResourceNames.Where(it => it.Split('.').Contains(resourceKey)))
        {
            using var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream is null) continue;

            using var streamReader = new StreamReader(stream);
            var jsonString = streamReader.ReadToEnd();
            AddJson(jsonString);
        }
    }
}
