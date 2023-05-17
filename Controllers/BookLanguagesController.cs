using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestProject.Models;

namespace RestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookLanguagesController : ControllerBase
    {
        private readonly RestProjectContext _context;

        public BookLanguagesController(RestProjectContext context)
        {
            _context = context;
        }

        // GET: api/BookLanguages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLanguageToReceive>>> GetBookLanguage()
        {
          if (_context.BookLanguages == null)
          {
              return NotFound();
          }

            var languages = from l in _context.BookLanguages
                            .Include(l => l.Books)
                             select new BookLanguageToReceive()
                             {
                                 Language_id = l.Language_id,
                                 Language_code = l.Language_code,
                                 Language_name = l.Language_name,
                                 Books = (from b in l.Books
                                          select new BookToTransfer()
                                          {
                                              Book_id = b.Book_id,
                                              Isbn13 = b.Isbn13,
                                              Num_pages = b.Num_pages,
                                              Title = b.Title,
                                              Publication_date = b.Publication_date,
                                              Publisher_id = b.Publisher_id,
                                              Language_id = b.Language_id
                                          }).ToList()
                             };

            return await languages.ToListAsync();
        }

        // GET: api/BookLanguages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookLanguageToReceive>> GetBookLanguage(int id)
        {
          if (_context.BookLanguages == null)
          {
              return NotFound();
          }
            var bookLanguage = await (from l in _context.BookLanguages
                            .Include(l => l.Books)
                                      select new BookLanguageToReceive()
                                      {
                                          Language_id = l.Language_id,
                                          Language_code = l.Language_code,
                                          Language_name = l.Language_name,
                                          Books = (from b in l.Books
                                                   select new BookToTransfer()
                                                   {
                                                       Book_id = b.Book_id,
                                                       Isbn13 = b.Isbn13,
                                                       Num_pages = b.Num_pages,
                                                       Title = b.Title,
                                                       Publication_date = b.Publication_date,
                                                       Publisher_id = b.Publisher_id,
                                                       Language_id = b.Language_id
                                                   }).ToList()
                                      }).FirstOrDefaultAsync(i => i.Language_id == id);

            if (bookLanguage == null)
            {
                return NotFound();
            }

            return bookLanguage;
        }

        // PUT: api/BookLanguages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookLanguage(int id, BookLanguageToTransfer bookLanguage)
        {
            if (id != bookLanguage.Language_id)
            {
                return BadRequest();
            }
            var newBookLanguage = await _context.BookLanguages.FindAsync(id);
            if (newBookLanguage == null) return NotFound();
            newBookLanguage.Language_code = bookLanguage.Language_code;
            newBookLanguage.Language_name = bookLanguage.Language_name;

            _context.Entry(newBookLanguage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookLanguageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BookLanguages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookLanguage>> PostBookLanguage(BookLanguageToTransfer bookLanguageDto)
        {
          if (_context.BookLanguages == null)
          {
              return Problem("Entity set 'RestProjectContext.BookLanguage'  is null.");
          }

            var bookLanguage = new BookLanguage()
            {
                Language_id = bookLanguageDto.Language_id,
                Language_name = bookLanguageDto.Language_name,
                Language_code = bookLanguageDto.Language_code
            };
            _context.BookLanguages.Add(bookLanguage);
            await _context.SaveChangesAsync();
            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (BookLanguageExists(bookLanguage.LanguageId))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return CreatedAtAction("GetBookLanguage", new { id = bookLanguage.Language_id }, bookLanguage);
        }

        // DELETE: api/BookLanguages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookLanguage(int id)
        {
            if (_context.BookLanguages == null)
            {
                return NotFound();
            }
            var bookLanguage = await _context.BookLanguages.FindAsync(id);
            if (bookLanguage == null)
            {
                return NotFound();
            }

            _context.BookLanguages.Remove(bookLanguage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookLanguageExists(int id)
        {
            return (_context.BookLanguages?.Any(e => e.Language_id == id)).GetValueOrDefault();
        }
    }
}
