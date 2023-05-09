using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestProject.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace RestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly RestProjectContext _context;

        public PublishersController(RestProjectContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublisherToReceive>>> GetPublishers()
        {
          if (_context.Publishers == null)
          {
              return NotFound();
          }

          var publishers = from p in _context.Publishers.Include(p=>p.Books)
                           select new PublisherToReceive()
                           {
                               Id = p.Id, Publisher_name = p.Publisher_name,
                               Books = (from b in p.Books
                                       select new BookToReceive()
                                       {
                                           Id = b.Id, Isbn13 = b.Isbn13, Num_pages = b.Num_pages, Title = b.Title,
                                           Publication_date = b.Publication_date, Publisher_name = p.Publisher_name,
                                           Authors = (from a in b.Authors
                                                      select new AuthorToTransfer()
                                                      {
                                                          Id = a.Id, Author_name = a.Author_name
                                                      }).ToList()
                                       }).ToList()
                           };

            return await publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PublisherToReceive>> GetPublisher(int id)
        {
          if (_context.Publishers == null)
          {
              return NotFound();
          }
            var publisher = await (from p in _context.Publishers.Include(b=>b.Books)
                             select new PublisherToReceive()
                             {
                                 Publisher_name = p.Publisher_name, Id = p.Id,
                                 Books = (from b in p.Books
                                          select new BookToReceive()
                                          {
                                              Id = b.Id,
                                              Isbn13 = b.Isbn13,
                                              Num_pages = b.Num_pages,
                                              Title = b.Title,
                                              Publication_date = b.Publication_date,
                                              Publisher_name = p.Publisher_name,
                                              Authors = (from a in b.Authors
                                                         select new AuthorToTransfer()
                                                         {
                                                             Id = a.Id,
                                                             Author_name = a.Author_name
                                                         }).ToList()
                                          }).ToList()
                             }).FirstOrDefaultAsync(i => i.Id == id);

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, PublisherToTransfer publisher)
        {
            if (id != publisher.Id)
            {
                return BadRequest();
            }

            var newPublisher = await _context.Publishers.FindAsync(id);
            if (newPublisher == null)
            {
                return NotFound();
            }
            newPublisher.Publisher_name = publisher.Publisher_name;
            _context.Entry(newPublisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
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

        // POST: api/Publishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Publisher>> PostPublisher(PublisherToTransfer publisherDto)
        {
          if (_context.Publishers == null)
          {
              return Problem("Entity set 'RestProjectContext.Publishers'  is null.");
          }

            var publisher = new Publisher()
            {
                Id = publisherDto.Id, Publisher_name = publisherDto.Publisher_name
            }; /*_mapper.Map<Publisher>(publisherDto);*/
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new { id = publisher.Id }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            if (_context.Publishers == null)
            {
                return NotFound();
            }
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublisherExists(int id)
        {
            return (_context.Publishers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
