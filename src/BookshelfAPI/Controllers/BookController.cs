using BookshelfAPI.Model;
using BookshelfAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Gets all books.
        /// </summary>
        /// <returns>All books in repository.</returns>
        /// <response code="200">Returns all books</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAll()
        {
            var books = await _bookRepository.GetAll();

            return Ok(books);
        }
        /// <summary>
        /// Gets book of specific id given.
        /// </summary>
        /// <returns>Book with given id.</returns>
        /// <response code="200">Returns book with given id</response>
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Get(long id)
        {
            var book = await _bookRepository.Get(id);

            if (book == null) return NotFound("The book record couldn't be found.");
            return Ok(book);
        }

        /// <summary>
        /// Adds new book to database.
        /// </summary>
        /// <returns>The created book.</returns>
        /// <response code="201">Returns newly created book.</response>
        /// <response code="400">If the book to add is null</response>
        /// <response code="409">If the book of given id already exists</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Post(Book book)
        {
            if (book == null) return BadRequest("Book is null.");

            //var check = await _bookRepository.Get(book.Id);
            //if (check.Id == book.Id) return Conflict("Book with specified ID already exists.");

            try
            {
                await _bookRepository.Add(book);
            }
            catch (Exception)
            {
                return Conflict("Book with specified ID already exists.");
            }

            return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
        }

        /// <summary>
        /// Updates data of book with given id.
        /// </summary>
        /// <returns>Success code</returns>
        /// <response code="200">Returns code 200, OK.</response>
        /// <response code="404">If a book with given id doesn't exist.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Put(long id, Book book)
        {
            var bookToUpdate = await _bookRepository.Get(id);

            if (bookToUpdate == null) return NotFound("The book to update record couldn't be found.");

            await _bookRepository.Update(bookToUpdate, book);

            return Ok();
        }

        /// <summary>
        /// Deletes book of given id.
        /// </summary>
        /// <returns>Success code</returns>
        /// <response code="200">Returns code 200, OK.</response>
        /// <response code="404">If a book with given id doesn't exist.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var bookToDelete = await _bookRepository.Get(id);

            if (bookToDelete == null) return NotFound("The book to delete record couldn't be found.");

            await _bookRepository.Delete(bookToDelete);

            return Ok();
        }

    }
}
