namespace Models.ProcessingServices.TitleNormalization.Common;

public interface ITitleNormalization
{
    public string TitleNormalization(string title);
    public List<string> CreateTitleNormalizationTerms(string title);
}
