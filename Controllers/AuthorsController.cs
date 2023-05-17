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
    public class AuthorsController : ControllerBase
    {
        private readonly RestProjectContext _context;

        public AuthorsController(RestProjectContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorToReceive>>> GetAuthors()
        {
          if (_context.Authors == null)
          {
              return NotFound();
          }

          var authors = from a in _context.Authors.Include(a => a.Books)
                        select new AuthorToReceive()
                        {
                            Author_id = a.Author_id, Author_name = a.Author_name,
                            Books = (from b in a.Books
                                     select new BookToTransfer()
                                     {
                                         Book_id = b.Book_id, Isbn13 = b.Isbn13, Num_pages = b.Num_pages,
                                         Title = b.Title, Publication_date = b.Publication_date,
                                         Publisher_id = b.Publisher_id, Language_id = b.Language_id
                                     }).ToList()
                        };

            return await authors.ToListAsync();
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorToReceive>> GetAuthor(int id)
        {
          if (_context.Authors == null)
          {
              return NotFound();
          }
            var author = await (from a in _context.Authors.Include(a => a.Books)
                                select new AuthorToReceive()
                                {
                                    Author_id = a.Author_id,
                                    Author_name = a.Author_name,
                                    Books = (from b in a.Books
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
                                }).FirstOrDefaultAsync(i=>i.Author_id == id);

            if (author == null)
            {
                return NotFound();
            }

            return author;
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorToTransfer author)
        {
            if (id != author.Author_id)
            {
                return BadRequest();
            }

            var newAuthor = await _context.Authors.FindAsync(id);
            if (newAuthor == null)
            {
                return NotFound();
            }
            newAuthor.Author_name = author.Author_name;

            _context.Entry(newAuthor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Author>> PostAuthor(AuthorToTransfer authorDto)
        {
          if (_context.Authors == null)
          {
              return Problem("Entity set 'RestProjectContext.Authors'  is null.");
          }
            var author = new Author()
            {
                Author_id = authorDto.Author_id,
                Author_name = authorDto.Author_name
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (AuthorExists(author.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return CreatedAtAction("GetAuthor", new { id = author.Author_id }, author);
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (_context.Authors == null)
            {
                return NotFound();
            }
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return (_context.Authors?.Any(e => e.Author_id == id)).GetValueOrDefault();
        }
    }
}
