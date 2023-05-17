using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

          var publishers = from p in _context.Publishers
                           .Include(p=>p.Books)
                           select new PublisherToReceive()
                           {
                               Publisher_id = p.Publisher_id, Publisher_name = p.Publisher_name,
                               Books = (from b in p.Books
                                       select new BookToTransfer()
                                       {
                                           Book_id = b.Book_id, Isbn13 = b.Isbn13, Num_pages = b.Num_pages, Title = b.Title,
                                           Publication_date = b.Publication_date,
                                           Publisher_id = b.Publisher_id,
                                           Language_id = b.Language_id
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
                                 Publisher_name = p.Publisher_name,
                                 Publisher_id = p.Publisher_id,
                                 Books = (from b in p.Books
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
                             }).FirstOrDefaultAsync(i => i.Publisher_id == id);

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
            if (id != publisher.Publisher_id)
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
                Publisher_id = publisherDto.Publisher_id, Publisher_name = publisherDto.Publisher_name
            }; /*_mapper.Map<Publisher>(publisherDto);*/
            _context.Publishers.Add(publisher);

            await _context.SaveChangesAsync();
            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (PublisherExists(publisher.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return CreatedAtAction("GetPublisher", new { id = publisher.Publisher_id }, publisher);
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
            return (_context.Publishers?.Any(e => e.Publisher_id == id)).GetValueOrDefault();
        }
    }
}
