using System.IO;

namespace I18n.Avalonia.TranslatorProviders.Json;

public class DefaultLocalJsonI18nProvider(string filePath) : AbsJsonI18nProvider
{
    public override void FillValues()
    {
        if (!Directory.Exists(filePath)) throw new DirectoryNotFoundException(filePath);

        foreach (var file in Directory.EnumerateFiles(filePath))
        {
            var jsonString = File.ReadAllText(file);

            AddJson(jsonString);
        }
    }
}
