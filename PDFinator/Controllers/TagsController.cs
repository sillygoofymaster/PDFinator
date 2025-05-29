using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDFinator.Models;

namespace PDFinator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsApiController : Controller
{
    private readonly PDFinatorContext _context;

    public TagsApiController(PDFinatorContext context)
    {
        _context = context;
    }

    // GET: api/Tags
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
    {
        return await _context.Tags.ToListAsync();
    }

    // GET: api/Tags/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Tag>> GetTag(int id)
    {
        var tag = await _context.Tags.FindAsync(id);

        if (tag == null)
            return NotFound();

        return tag;
    }

    // POST: api/Tags
    [HttpPost]
    public async Task<ActionResult<Tag>> PostTag(Tag tag)
    {


            _context.Tags.Add(tag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tag);
    }

    // PUT: api/Tags/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTag(int id, Tag tag)
    {
        if (id != tag.Id)
            return BadRequest();

        _context.Entry(tag).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tags.Any(e => e.Id == id))
                return NotFound();

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Tags/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null)
            return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
