using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PDFinator.Models;
using PDFinator.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Humanizer;

namespace PDFinator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PDFsController : ControllerBase
{
    private readonly PDFinatorContext _context;
    private readonly IWebHostEnvironment _hostingEnv;

    public PDFsController(PDFinatorContext context, IWebHostEnvironment hostingEnv)
    {
        _context = context;
        _hostingEnv = hostingEnv;
    }

    // GET api/PDFs
    [HttpGet]
    public async Task<ActionResult<List<PDF>>> GetAll()
    {
        var list = await _context.PDFs.ToListAsync();
        return Ok(list);
    }

    // POST api/PDFs
    [HttpPost, DisableRequestSizeLimit]
    public async Task<ActionResult<PDF>> Create([FromForm] PDFViewModel PDFvm)
    {
        var ext = Path.GetExtension(PDFvm.File.FileName)?.ToLowerInvariant();
        if (ext != ".pdf")
        {
            ModelState.AddModelError("File", "Це не PDF.");
            return BadRequest(ModelState);
        }

        var uploads = Path.Combine(_hostingEnv.WebRootPath, "uploads");
        if (!Directory.Exists(uploads))
            Directory.CreateDirectory(uploads);

        var fileName = Path.GetFileName(PDFvm.File.FileName);
        var filePath = Path.Combine(uploads, fileName);

        using (var fs = new FileStream(filePath, FileMode.Create))
            await PDFvm.File.CopyToAsync(fs);

        var pdf = new PDF
        {
            Title = PDFvm.Title,
            UploadDate = DateTime.Now,
            Abstract = PDFvm.Abstract,
            Author=PDFvm.Author,
            Tags = PDFvm.Tags,
            FilePath = $"/uploads/{fileName}"
        };

        _context.PDFs.Add(pdf);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = pdf.Id }, pdf);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, [FromBody] UpdatePDFViewModel vm)
    {
        if (id != vm.Id)
            return BadRequest();

        var pdf = await _context.PDFs.FindAsync(id);
        if (pdf == null)
            return NotFound();

        pdf.Title = vm.Title;
        pdf.Abstract = vm.Abstract;
        pdf.Author = vm.Author;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pdf = await _context.PDFs.FindAsync(id);
        if (pdf == null)
            return NotFound();

        var relativePath = pdf.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_hostingEnv.WebRootPath, relativePath);

        if (System.IO.File.Exists(physicalPath))
        {
            try
            {
                System.IO.File.Delete(physicalPath);
            }
            catch (IOException ioEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error deleting file on disk: {ioEx.Message}");
            }
        }

        _context.PDFs.Remove(pdf);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}


