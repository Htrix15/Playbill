using Microsoft.Extensions.Options;
using Models.ProcessingServices.TitleNormalization.Common;
using System.Text.RegularExpressions;

namespace Models.ProcessingServices.TitleNormalization;

public class TitleNormalizationService: ITitleNormalization
{
    private readonly char[] _separators;
    private readonly List<string> _wordEndings;
    private readonly int _maxSizeForNotWordEndingsRemoveing;

    public TitleNormalizationService(IOptions<TitleNormalizationOptions> titleCompareOptions)
    {
        _separators = titleCompareOptions.Value.Separators.ToArray();
        _wordEndings = titleCompareOptions.Value.WordEndings.OrderByDescending(wordEnding => wordEnding.Length).ToList();
        _maxSizeForNotWordEndingsRemoveing = titleCompareOptions.Value.MaxSizeForNotWordEndingsRemoveing;
    }

    const string NormilizeTitlePattern = "(?i)[^А-ЯЁA-Z0-9]";

    public string TitleNormalization(string title) => Regex.Replace(title ?? "", NormilizeTitlePattern, "").ToLower();

    private Func<string, List<string>, int, string> TermNormalization = (string term, List<string> wordEndings, int maxSizeForNotWordEndingsRemoveing) =>
    {
        term = Regex.Replace(term, NormilizeTitlePattern, "");
        if (term.Length <= maxSizeForNotWordEndingsRemoveing)
        {
            return term;
        }
        foreach (var wordEnding in wordEndings)
        {
            if (term.EndsWith(wordEnding))
            {
                return term.Substring(0, term.Length - wordEnding.Length);
            }
        }
        return term;
    };

    public List<string> CreateTitleNormalizationTerms(string title)
    {
        return title.Split(_separators)
            .Where(term => !string.IsNullOrEmpty(term))
            .Select(term => TermNormalization(term, _wordEndings, _maxSizeForNotWordEndingsRemoveing))
            .ToList();
    }
}
