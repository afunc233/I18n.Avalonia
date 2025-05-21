using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace I18n.Avalonia.Primitives;

internal static class I18nUnitExtension
{
    private static readonly FuncMultiValueConverter<string, string?> s_funcMultiValueConverter =
        new(bindingValues =>
        {
            var args = bindingValues.ToList();
            var format = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(format))
            {
                return format;
            }

            try
            {
                return string.Format(format, args.Skip(1).ToArray<object?>());
            }
            catch
            {
                return format;
            }
        });

    internal static IBinding ToBinding(this I18nUnit unit, IEnumerable<object>? args)
    {
        var retBinding = unit.Value.ToBinding();

        var argList = (args ?? []).ToList();
        if (argList.Count <= 0) return retBinding;

        var bindings = new List<IBinding> { retBinding, };

        foreach (var arg in argList)
        {
            switch (arg)
            {
                case I18nUnit unitArg:
                    bindings.Add(unitArg.Value.ToBinding());
                    break;
                case I18nExtension i18:
                    var i18nProvideValue = i18.ProvideValue(null);
                    if (i18nProvideValue is IBinding i18nBinding)
                    {
                        bindings.Add(i18nBinding);
                    }

                    break;
                case IBinding binding:
                    bindings.Add(binding);
                    break;
                case Func<string> func:
                    bindings.Add(new Binding { Source = func.Invoke() });
                    break;
                default:
                    bindings.Add(new Binding { Source = arg.ToString() });
                    break;
            }
        }

        retBinding = new MultiBinding { Bindings = bindings, Converter = s_funcMultiValueConverter, };

        return retBinding;
    }
}
