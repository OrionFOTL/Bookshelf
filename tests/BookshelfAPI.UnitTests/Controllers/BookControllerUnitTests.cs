using BookshelfAPI.Controllers;
using BookshelfAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookshelfAPI.UnitTests.Controllers
{
    public class BookControllerUnitTests
    {
        [Fact]
        public async Task get_all_books()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(get_all_books));
            var controller = new BookController(repository);

            // Act
            var response = await controller.GetAll() as ObjectResult;
            var books = response.Value as List<Book>;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(3, books.Count);
        }

        [Fact]
        public async Task get_book_with_existing_id()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(get_book_with_existing_id));
            var controller = new BookController(repository);
            var expectedTitle = "Zbrodnia i kara";

            // Act
            var response = await controller.Get(1) as ObjectResult;
            var book = response.Value as Book;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(expectedTitle, book.Title);
        }

        [Fact]
        public async Task get_book_with_nonexisting_id()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(get_book_with_nonexisting_id));
            var controller = new BookController(repository);
            var expectedMessage = "The book record couldn't be found.";

            // Act
            var response = await controller.Get(9999999) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
            Assert.Equal(expectedMessage, response.Value);
        }

        [Fact]
        public async Task post_new_book()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(post_new_book));
            var controller = new BookController(repository);
            var bookToInsert = new Book()
            {
                Id = 4 ,
                Title = "Ender's Game",
                Author = "Orson Scott Card",
                ISBN = "312159785",
                Year = 1985
            };
            var expectedTitle = "Ender's Game";

            // Act
            var response = await controller.Post(bookToInsert) as ObjectResult;
            var bookreceived = response.Value as Book;

            // Assert
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(expectedTitle, bookreceived.Title);
        }
    }
}
