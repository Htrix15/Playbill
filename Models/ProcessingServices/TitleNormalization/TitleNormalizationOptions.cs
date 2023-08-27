
namespace Models.ProcessingServices.TitleNormalization;

public class TitleNormalizationOptions
{
    public HashSet<char> Separators { get; set; }
    public HashSet<string> WordEndings { get; set;}
    public int MaxSizeForNotWordEndingsRemoveing { get; set; }
}
