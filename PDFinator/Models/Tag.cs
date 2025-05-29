using System.ComponentModel.DataAnnotations;

namespace PDFinator.Models;

public class Tag
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public List<PDFTags> PDFs { get; set; } = new List<PDFTags>();
}
