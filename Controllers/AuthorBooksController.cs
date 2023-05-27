using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestProject.Models;

namespace RestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet("{bookId} & {authorId}")]
        public async Task<ActionResult<AuthorBook>> GetAuthorBook(int bookId, int authorId)
        {
          if (_context.AuthorBooks == null)
          {
              return NotFound();
          }
            var authorBook = await _context.AuthorBooks.FindAsync(bookId, authorId);

            if (authorBook == null)
            {
                return NotFound();
            }

            return authorBook;
        }

        // PUT: api/AuthorBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{bookId} & {authorId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutAuthorBook(int bookId, int authorId, AuthorBook authorBook)
        {

            if (await _context.AuthorBooks.FindAsync(bookId, authorId) == null)
            {
                return NotFound();
            }
            

            try
            {
                await PostAuthorBook(new AuthorBook() { Author_id = authorBook.Author_id, Book_id = authorBook.Book_id });
                await DeleteAuthorBook(bookId, authorId);
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorBookExists(authorBook.Book_id, authorBook.Author_id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict();
                }
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/AuthorBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
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
                if (AuthorBookExists(authorBook.Book_id, authorBook.Author_id))
                {
                    return Conflict();
                }
                else
                {
                    return Conflict();
                }
            }

            return CreatedAtAction("GetAuthorBook", new { authorId = authorBook.Author_id,
                                                          bookId = authorBook.Book_id}, authorBook);
        }

        // DELETE: api/AuthorBooks/5
        [HttpDelete("{bookId} & {authorId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteAuthorBook(int bookId, int authorId)
        {
            if (_context.AuthorBooks == null)
            {
                return NotFound();
            }
            var authorBook = await _context.AuthorBooks.FindAsync(bookId, authorId);
            if (authorBook == null)
            {
                return NotFound();
            }

            _context.AuthorBooks.Remove(authorBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorBookExists(int bookId, int authorId)
        {
            return (_context.AuthorBooks?.Any(e => e.Book_id == bookId && e.Author_id == authorId)).GetValueOrDefault();
        }
    }
}
