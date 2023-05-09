using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestProject.Models;

namespace RestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorBooksController : ControllerBase
    {
        private readonly RestProjectContext _context;

        public AuthorBooksController(RestProjectContext context)
        {
            _context = context;
        }

        // GET: api/AuthorBooks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorBook>>> GetAuthorBooks()
        {
          if (_context.AuthorBooks == null)
          {
              return NotFound();
          }
            return await _context.AuthorBooks.ToListAsync();
        }

        // GET: api/AuthorBooks/5
        [HttpGet("{authorId} & {bookId}")]
        public async Task<ActionResult<AuthorBook>> GetAuthorBook(int authorId, int bookId)
        {
          if (_context.AuthorBooks == null)
          {
              return NotFound();
          }
            var authorBook = await _context.AuthorBooks.FindAsync(authorId, bookId);

            if (authorBook == null)
            {
                return NotFound();
            }

            return authorBook;
        }

        // PUT: api/AuthorBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{authorId} & {bookId}")]
        public async Task<IActionResult> PutAuthorBook(int authorId, int bookId, AuthorBook authorBook)
        {

            if (await _context.AuthorBooks.FindAsync(authorId, bookId) == null)
            {
                return NotFound();
            }
            await PostAuthorBook(new AuthorBook() { AuthorId = authorBook .AuthorId, BookId = authorBook.BookId});
            await DeleteAuthorBook(authorId, bookId);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorBookExists(authorBook.AuthorId, authorBook.BookId))
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

        // POST: api/AuthorBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AuthorBook>> PostAuthorBook(AuthorBook authorBook)
        {
          if (_context.AuthorBooks == null)
          {
              return Problem("Entity set 'RestProjectContext.AuthorBooks'  is null.");
          }
            _context.AuthorBooks.Add(authorBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AuthorBookExists(authorBook.AuthorId, authorBook.BookId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthorBook", new { authorId = authorBook.AuthorId,
                                                          bookId = authorBook.BookId}, authorBook);
        }

        // DELETE: api/AuthorBooks/5
        [HttpDelete("{authorId} & {bookId}")]
        public async Task<IActionResult> DeleteAuthorBook(int authorId, int bookId)
        {
            if (_context.AuthorBooks == null)
            {
                return NotFound();
            }
            var authorBook = await _context.AuthorBooks.FindAsync(authorId, bookId);
            if (authorBook == null)
            {
                return NotFound();
            }

            _context.AuthorBooks.Remove(authorBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorBookExists(int authorId, int bookId)
        {
            return (_context.AuthorBooks?.Any(e => e.AuthorId == authorId && e.BookId == bookId)).GetValueOrDefault();
        }
    }
}
