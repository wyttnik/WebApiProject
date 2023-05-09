using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
                            Id = a.Id, Author_name = a.Author_name,
                            Books = (from b in a.Books
                                     select new BookToTransfer()
                                     {
                                         Id = b.Id, Isbn13 = b.Isbn13, Num_pages = b.Num_pages,
                                         Title = b.Title, Publication_date = b.Publication_date,
                                         PublisherId = b.PublisherId
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
                                    Id = a.Id,
                                    Author_name = a.Author_name,
                                    Books = (from b in a.Books
                                             select new BookToTransfer()
                                             {
                                                 Id = b.Id,
                                                 Isbn13 = b.Isbn13,
                                                 Num_pages = b.Num_pages,
                                                 Title = b.Title,
                                                 Publication_date = b.Publication_date,
                                                 PublisherId = b.PublisherId
                                             }).ToList()
                                }).FirstOrDefaultAsync(i=>i.Id == id);

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
            if (id != author.Id)
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
                Id = authorDto.Id,
                Author_name = authorDto.Author_name
            };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, author);
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
            return (_context.Authors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
