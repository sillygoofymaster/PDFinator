namespace PDFinator.Models;

public class PDFTags
{
    public int Id { get; set; }
    public int PDFsId { get; set; }
    public int TagsId { get; set; }
    public PDF? PDF { get; set; }
    public Tag? Tag { get; set; }
}
