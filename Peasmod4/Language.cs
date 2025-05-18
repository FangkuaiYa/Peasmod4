using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using AmongUs.Data;

namespace Peasmod4;

public class Language
{
    private static Language language;
    private static readonly Dictionary<string, string> defaultLanguageSet = new();

    public Dictionary<string, string> languageSet;


    public Language()
    {
        languageSet = new Dictionary<string, string>(defaultLanguageSet);
    }


    public static string GetString(string key)
    {
        if (language?.languageSet.ContainsKey(key) ?? false) return language.languageSet[key];
        return /*"*" + */key;
    }

    public static bool CheckValidKey(string key)
    {
        return language?.languageSet.ContainsKey(key) ?? true;
    }

    public static void AddDefaultKey(string key, string format)
    {
        defaultLanguageSet.Add(key, format);
        if (!CheckValidKey(key)) language.languageSet.Add(key, format);
    }


    public static string GetLanguage(uint language)
    {
        switch (language)
        {
            case 0:
                return "English";
            case 1:
                return "Latam";
            case 2:
                return "Brazilian";
            case 3:
                return "Portuguese";
            case 4:
                return "Korean";
            case 5:
                return "Russian";
            case 6:
                return "Dutch";
            case 7:
                return "Filipino";
            case 8:
                return "French";
            case 9:
                return "German";
            case 10:
                return "Italian";
            case 11:
                return "Japanese";
            case 12:
                return "Spanish";
            case 13:
                return "SChinese";
            case 14:
                return "TChinese";
            case 15:
                return "Irish";
        }

        return "English";
    }

    public static void LoadDefaultKey()
    {
        AddDefaultKey("option.empty", "");
    }

    public static void Load()
    {
        var lang = GetLanguage((uint)DataManager.Settings.Language.CurrentLanguage);


        language = new Language();

        language.deserialize(GetDefaultLanguageStream());

        var builtinLang = GetBuiltinLanguageStream(lang);
        if (builtinLang != null) language.deserialize(builtinLang);
    }

    public static Stream GetDefaultLanguageStream()
    {
        return Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("Peasmod4.Resources.Languages.English.dat");
    }

    public static Stream GetBuiltinLanguageStream(string lang)
    {
        try
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Peasmod4.Resources.Languages." + lang + ".dat");
        }
        catch
        {
            return null;
        }
    }

    public bool deserialize(Stream stream)
    {
        using (var sr = new StreamReader(
                   stream, Encoding.GetEncoding("utf-8")))
        {
            return deserialize(sr);
        }
    }

    public bool deserialize(StreamReader reader)
    {
        var result = true;
        try
        {
            string data = "", line;


            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length < 3) continue;
                if (data.Equals(""))
                    data = line;
                else
                    data += "," + line;
            }


            if (!data.Equals(""))
            {
                var option = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };

                var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>("{ " + data + " }", option);
                foreach (var entry in deserialized) languageSet[entry.Key] = entry.Value;

                result = true;
            }
        }
        catch
        {
            result = false;
        }

        return result;
    }
}

internal static class LanguageExtension
{
    internal static string Translate(this string key)
    {
        return Language.GetString(key);
    }
}