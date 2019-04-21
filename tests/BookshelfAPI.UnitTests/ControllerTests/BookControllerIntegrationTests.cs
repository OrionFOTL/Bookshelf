using BookshelfAPI.Controllers;
using BookshelfAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookshelfAPI.UnitTests.Controllers
{
    public class BookControllerIntegrationTests
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
        
        [Fact]
        public async Task post_new_book_with_null()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(post_new_book));
            var controller = new BookController(repository);
            Book bookToInsert = null;
            var expectedMessage = "Book is null.";

            // Act
            var response = await controller.Post(bookToInsert) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(expectedMessage, response.Value);
        }

        [Fact]
        public async Task post_new_book_on_existing_id()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(post_new_book_on_existing_id));
            var controller = new BookController(repository);
            var bookToInsert = new Book()
            {
                Id = 1,
                Title = "Ender's Game",
                Author = "Orson Scott Card",
                ISBN = "312159785",
                Year = 1985
            };
            var expectedMessage = "Book with specified ID already exists.";

            // Act
            var response = await controller.Post(bookToInsert) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
            Assert.Equal(expectedMessage, response.Value);
        }

        [Fact]
        public async Task update_existing_book()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(update_existing_book));
            var controller = new BookController(repository);
            var updatedBook = new Book()
            {
                Title = "Ender's Game",
                Author = "Orson Scott Card",
                ISBN = "312159785",
                Year = 1985
            };

            // Act
            var response = await controller.Put(1, updatedBook);

            // Assert
            Assert.IsType<OkResult>(response);
        }

        [Fact]
        public async Task update_nonexisting_id()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(update_nonexisting_id));
            var controller = new BookController(repository);
            var updatedBook = new Book()
            {
                Title = "Ender's Game",
                Author = "Orson Scott Card",
                ISBN = "312159785",
                Year = 1985
            };
            var expectedMessage = "The book to update record couldn't be found.";

            // Act
            var response = await controller.Put(999, updatedBook) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
            Assert.Equal(expectedMessage, response.Value);
        }

        [Fact]
        public async Task delete_existing_book()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(delete_existing_book));
            var controller = new BookController(repository);

            // Act
            var response = await controller.Delete(3);
            var getBooks = await controller.GetAll() as ObjectResult;
            var books = getBooks.Value as List<Book>;

            // Assert
            Assert.IsType<OkResult>(response);
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task delete_nonexisting_book()
        {
            // Arrange
            var repository = BookContextMocker.GetInMemoryBookRepository(nameof(delete_nonexisting_book));
            var controller = new BookController(repository);
            var expectedMessage = "The book to delete record couldn't be found.";

            // Act
            var response = await controller.Delete(999) as ObjectResult;
            var getBooks = await controller.GetAll() as ObjectResult;
            var books = getBooks.Value as List<Book>;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
            Assert.Equal(expectedMessage, response.Value);
            Assert.Equal(3, books.Count);
        }

        // Dekorator [ApiController] w BookController.cs sam sprawdza czy podano wszystkie
        // wymagane pola w żądaniu i zwraca kod 400, ale ta walidacja nie jest aktywna w testach;
        // Test sprawdza samą walidację
        [Fact]
        public async Task validate_incomplete_book()
        {
            // Arrange
            var obj = new Book()
            {
                Year = 1956
            };
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();

            // Act 
            var valid = Validator.TryValidateObject(obj, context, results, true);

            // Assert
            Assert.False(valid);
        }

        //[Fact]
        //public async Task update_book_without_required_data()
        //
        //{
        //    // Arrange
        //    var repository = BookContextMocker.GetInMemoryBookRepository(nameof(update_book_without_required_data));
        //    var controller = new BookController(repository);
        //    var updatedBook = new Book()
        //    {
        //        Year = 1985
        //    };

        //    // Act
        //    var response = await controller.Put(1, updatedBook) as ObjectResult;

        //    // Assert
        //    Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        //}
    }
}
