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

            var book = await (from b in _context.Books.Include(b => b.Publisher).Include(b => b.Authors)
                              select new BookToReceive()
                              {
                                  Id = b.Id, Isbn13 = b.Isbn13, Num_pages = b.Num_pages, Title = b.Title,
                                  Publication_date = b.Publication_date, Publisher_name = b.Publisher.Publisher_name,
                                  Authors = (from a in b.Authors
                                             select new AuthorToTransfer()
                                             {
                                                 Id = a.Id,
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
            var book = await (from b in _context.Books.Include(b => b.Publisher).Include(b => b.Authors)
                              select new BookToReceive()
                              {
                                  Id = b.Id,
                                  Isbn13 = b.Isbn13,
                                  Num_pages = b.Num_pages,
                                  Title = b.Title,
                                  Publication_date = b.Publication_date,
                                  Publisher_name = b.Publisher.Publisher_name,
                                  Authors = (from a in b.Authors
                                             select new AuthorToTransfer()
                                             {
                                                 Id = a.Id,
                                                 Author_name = a.Author_name
                                             }).ToList()
                              }).FirstOrDefaultAsync(i=>i.Id == id);

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
            if (id != book.Id)
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
            newBook.PublisherId = book.PublisherId;

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
                Id = bookDto.Id, Isbn13 = bookDto.Isbn13, Num_pages = bookDto.Num_pages, Title = bookDto.Title,
                Publication_date=bookDto.Publication_date, PublisherId = bookDto.PublisherId
            };/*_mapper.Map<Book>(bookDto);*/

          _context.Books.Add(book);
          await _context.SaveChangesAsync();

          return CreatedAtAction("GetBook", new { id = book.Id }, book);
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
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
