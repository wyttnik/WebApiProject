using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Permissions;
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
    public class BooksController : ControllerBase
    {
        private readonly RestProjectContext _context;
        private readonly IWebHostEnvironment _environment;

        public BooksController(RestProjectContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Books
        [HttpGet]
        [AllowAnonymous]
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
                                  ImageUrl = b.ImageUrl,
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
                                  ImageUrl = b.ImageUrl,
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutBook(int id, [FromForm] BookToTransfer book)
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

            string? path = null;
            newBook.Title = book.Title;
            newBook.Isbn13 = book.Isbn13;
            newBook.Num_pages = book.Num_pages;
            newBook.Publication_date = book.Publication_date;
            newBook.Publisher_id = book.Publisher_id;
            newBook.Language_id = book.Language_id;
            if (book.FileUri != null)
            {
                path = await UploadImage(book.FileUri);
                newBook.ImageUrl = path;
            }

            _context.Entry(newBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();


            }
            catch (Exception) // DbUpdateConcurrencyException
            {
                if (path != null) System.IO.File.Delete(path);
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict();
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Book>> PostBook([FromForm]BookToTransfer bookDto)
        {
          if (_context.Books == null)
          {
              return Problem("Entity set 'RestProjectContext.Books'  is null.");
          }
            //string fileName = bookDto.FileUri.FileName;
            //string filePath = GetFilePath(fileName);
            string path = await UploadImage(bookDto.FileUri);
            var book = new Book()
            {
                Book_id = bookDto.Book_id, Isbn13 = bookDto.Isbn13, Num_pages = bookDto.Num_pages, Title = bookDto.Title,
                Publication_date = bookDto.Publication_date,
                Publisher_id = bookDto.Publisher_id,
                Language_id = bookDto.Language_id,
                ImageUrl = path
            };

             _context.Books.Add(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception) //DbUpdateException
            {
                if (BookExists(book.Book_id))
                {
                    return Conflict();
                }
                else
                {
                    return Conflict();
                }
            }

            return CreatedAtAction("GetBook", new { id = book.Book_id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
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

        private async Task<string> UploadImage(IFormFile file)
        {
            if (file != null)
            {
                var filePath = _environment.WebRootPath + "\\Images\\" + file.FileName;
                //if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

                using (FileStream ms = new(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(ms);
                }
                return "https://localhost:7159/Images/" + file.FileName;
            }
            return "https://localhost:7159/Images/0.png";
        }
    }
}
