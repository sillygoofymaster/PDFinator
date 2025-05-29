using System.ComponentModel.DataAnnotations;

namespace PDFinator.Models;

public class PDF
{
    public int Id { get; set; }

    [Required(ErrorMessage="Поле не може бути порожнім")]
    [Display(Name="Назва")]
    public string Title { get; set; } = null!;

    public string FilePath { get; set; } = null!;
    public DateTime UploadDate { get; set; }

    [Display(Name = "Анотація")]
    public string? Abstract { get; set; }

    public string? Author { get; set; }

    public List<PDFTags> Tags { get; set; } = new List<PDFTags>();
}

