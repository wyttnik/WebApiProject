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
    public class BooksController : ControllerBase
    {
        private readonly RestProjectContext _context;

        public BooksController(RestProjectContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookToReceive>>> GetBooks()
        {
          if (_context.Books == null)
          {
              return NotFound();
          }

            var book = await (from b in _context.Books
                              .Include(b => b.Publisher)
                              .Include(b => b.Authors)
                              .Include(b => b.BookLanguage)
                              select new BookToReceive()
                              {
                                  Book_id = b.Book_id, Isbn13 = b.Isbn13, Num_pages = b.Num_pages, Title = b.Title,
                                  Publication_date = b.Publication_date,
                                  Publisher = new PublisherToTransfer()
                                  {
                                      Publisher_id = b.Publisher_id,
                                      Publisher_name = b.Publisher.Publisher_name
                                  },
                                  BookLanguage = new BookLanguageToTransfer()
                                  {
                                      Language_id = b.Language_id,
                                      Language_code = b.BookLanguage.Language_code,
                                      Language_name = b.BookLanguage.Language_name
                                  },
                                  Authors = (from a in b.Authors
                                             select new AuthorToTransfer()
                                             {
                                                 Author_id = a.Author_id,
                                                 Author_name = a.Author_name
                                             }).ToList()
                              }).ToListAsync();

            return book;
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookToReceive>> GetBook(int id)
        {
          if (_context.Books == null)
          {
              return NotFound();
          }
            var book = await (from b in _context.Books
                              .Include(b => b.Publisher)
                              .Include(b => b.Authors)
                              .Include(b => b.BookLanguage)
                              select new BookToReceive()
                              {
                                  Book_id = b.Book_id,
                                  Isbn13 = b.Isbn13,
                                  Num_pages = b.Num_pages,
                                  Title = b.Title,
                                  Publication_date = b.Publication_date,
                                  Publisher = new PublisherToTransfer()
                                  {
                                      Publisher_id = b.Publisher_id,
                                      Publisher_name = b.Publisher.Publisher_name
                                  },
                                  BookLanguage = new BookLanguageToTransfer()
                                  {
                                      Language_id = b.Language_id, 
                                      Language_code = b.BookLanguage.Language_code,
                                      Language_name = b.BookLanguage.Language_name
                                  },
                                  Authors = (from a in b.Authors
                                             select new AuthorToTransfer()
                                             {
                                                 Author_id = a.Author_id,
                                                 Author_name = a.Author_name
                                             }).ToList()
                              }).FirstOrDefaultAsync(i=>i.Book_id == id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookToTransfer book)
        {
            if (id != book.Book_id)
            {
                return BadRequest();
            }

            var newBook = await _context.Books.FindAsync(id);
            if (newBook == null)
            {
                return NotFound();
            }
            newBook.Title = book.Title;
            newBook.Isbn13 = book.Isbn13;
            newBook.Num_pages = book.Num_pages;
            newBook.Publication_date = book.Publication_date;
            newBook.Publisher_id = book.Publisher_id;
            newBook.Language_id = book.Language_id;

            _context.Entry(newBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookToTransfer bookDto)
        {
          if (_context.Books == null)
          {
              return Problem("Entity set 'RestProjectContext.Books'  is null.");
          }
            var book = new Book() 
            {
                Book_id = bookDto.Book_id, Isbn13 = bookDto.Isbn13, Num_pages = bookDto.Num_pages, Title = bookDto.Title,
                Publication_date=bookDto.Publication_date,
                Publisher_id = bookDto.Publisher_id,
                Language_id = bookDto.Language_id
            };/*_mapper.Map<Book>(bookDto);*/

          _context.Books.Add(book);
            await _context.SaveChangesAsync();
            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (BookExists(book.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return CreatedAtAction("GetBook", new { id = book.Book_id }, book);
        }

        //[HttpPost("{AuthorId}")]
        //public async Task<ActionResult<Book>> AttachAuthorToBook(int id)
        //{
        //    if (_context.Books == null)
        //    {
        //        return Problem("Entity set 'RestProjectContext.Books'  is null.");
        //    }
        //    _context.Books.Add(book);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetBook", new { id = book.Id }, book);
        //}

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.Book_id == id)).GetValueOrDefault();
        }
    }
}
