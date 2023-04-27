using System.Text.RegularExpressions;

namespace DeveloperClipboardCore;

public class RegexReplaceConverter
{
    public Task<ConvertionResult> Convert(string code, ConvertionInfo[] convertions)
    {
        var result = code;

        foreach (var convertion in convertions)
        {
            try
            {
                if (!string.IsNullOrEmpty(convertion.For))
                {
                    result = Regex.Replace(result, convertion.For, x => ApplyConvertion(convertion, x.Value));
                }
                else
                {
                    result = ApplyConvertion(convertion, result);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка работы над [{convertion.Find}]", e);
            }
        }

        return Task.FromResult(new ConvertionResult(result,
            result != code ? ConvertionState.Ok : ConvertionState.NothingToConvert));
    }

    private static string ApplyConvertion(ConvertionInfo convertion, string result)
    {
        if (convertion.MatchEvaluator != null)
        {
            result = Regex.Replace(result, convertion.Find, convertion.MatchEvaluator,
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
        else if (convertion.Replace != null)
        {
            result = Regex.Replace(result, convertion.Find, convertion.Replace,
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        return result;
    }
}