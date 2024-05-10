namespace Models.Billboards.Common.Exceptions;

public class FailParsePageExceptions : Exception
{
    public FailParsePageExceptions(PageBlock pageBlock)
        : base($"Block: {pageBlock}") { }
}

public enum PageBlock
{
    Page,
    FullAfishaBlock,
    AfishaItems,
    Title,
    Image,
    Place,
    Link,
    Date
}
