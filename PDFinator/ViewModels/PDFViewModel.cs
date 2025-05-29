using PDFinator.Models;
using System.ComponentModel.DataAnnotations;

namespace PDFinator.ViewModels;

public class PDFViewModel
{
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;

    [Display(Name = "Анотація")]
    public string? Abstract { get; set; }
    public string? Author { get; set; }

    public List<PDFTags> Tags { get; set; } = new List<PDFTags>();

    public IFormFile File { get; set; } = null!;
}
