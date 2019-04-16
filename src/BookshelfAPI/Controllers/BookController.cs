using BookshelfAPI.Model;
using BookshelfAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookshelfAPI.Controllers
{
    [Route("api/book")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookRepository<Book> _bookRepository;

        public BookController(IBookRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookRepository.GetAll();

            return Ok(books);
        }

        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(long id)
        {
            var book = await _bookRepository.Get(id);

            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Book book)
        {
            await _bookRepository.Add(book);

            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, Book book)
        {
            var bookToUpdate = await _bookRepository.Get(id);
            await _bookRepository.Update(bookToUpdate, book);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var bookToDelete = await _bookRepository.Get(id);
            await _bookRepository.Delete(bookToDelete);

            return NoContent();
        }

    }
}
