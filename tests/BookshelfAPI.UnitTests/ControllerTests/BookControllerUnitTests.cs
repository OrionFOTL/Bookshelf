﻿using BookshelfAPI.Controllers;
using BookshelfAPI.Model;
using BookshelfAPI.Repository;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BookshelfAPI.UnitTests.ControllerTests
{
    public class BookControllerUnitTests
    {
        #region GetAll

        [Fact]
        public async void GetAll_EndpointCalled_ReturnsBooksList()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            IEnumerable<Book> books = new List<Book>()
            {
                new Book() { Id = 1},
                new Book() { Id = 2},
                new Book() { Id = 3}
            };

            A.CallTo(() => bookRepository.GetAll()).Returns(Task.FromResult(books));

            // Act
            var result = await bookController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(3, ((List<Book>)((OkObjectResult)result).Value).Count);
        }

        #endregion

        #region Get

        [Fact]
        public async void Get_ForNonExistingId_Returns400WithMessage()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);

            A.CallTo(() => bookRepository.Get(1)).Returns(Task.FromResult<Book>(null));

            // Act
            var result = await bookController.Get(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The book record couldn't be found.", (string)((NotFoundObjectResult)result).Value);
        }

        [Fact]
        public async void Get_ForValidId_ReturnsBook()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            var book = new Book { Id = 1, Author = "aa", Title = "bb", ISBN = "ih", Year = 2019 };

            A.CallTo(() => bookRepository.Get(1)).Returns(Task.FromResult(book));

            // Act
            var result = await bookController.Get(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(book.Id, ((Book)((OkObjectResult)result).Value).Id);
            Assert.Equal(book.Title, ((Book)((OkObjectResult)result).Value).Title);
            Assert.Equal(book.Author, ((Book)((OkObjectResult)result).Value).Author);
            Assert.Equal(book.ISBN, ((Book)((OkObjectResult)result).Value).ISBN);
            Assert.Equal(book.Year, ((Book)((OkObjectResult)result).Value).Year);
        }

        #endregion

        #region Post

        [Fact]
        public async void Post_ForNullArgument_Returns400WithMessage()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);

            // Act
            var result = await bookController.Post(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Book is null.", (string)((BadRequestObjectResult)result).Value);
        }

        [Fact]
        public async void Post_ForExistingId_Returns409WithMessage()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            var newBook = new Book { Id = 1, Author = "aa", Title = "bb", ISBN = "ih", Year = 2019 };

            A.CallTo(() => bookRepository.Add(newBook)).Throws(new System.InvalidOperationException());

            // Act
            var result = await bookController.Post(newBook);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Book with specified ID already exists.", (string)((ConflictObjectResult)result).Value);
        }

        [Fact]
        public async void Post_ForValidId_Returns201WithBook()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            var newBook = new Book { Id = 1, Author = "aa", Title = "bb", ISBN = "ih", Year = 2019 };

            A.CallTo(() => bookRepository.Get(newBook.Id)).Returns(Task.FromResult(newBook));

            // Act
            var result = await bookController.Post(newBook);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(newBook.Id, ((Book)((CreatedAtActionResult)result).Value).Id);
            Assert.Equal(newBook.Title, ((Book)((CreatedAtActionResult)result).Value).Title);
            Assert.Equal(newBook.Author, ((Book)((CreatedAtActionResult)result).Value).Author);
            Assert.Equal(newBook.ISBN, ((Book)((CreatedAtActionResult)result).Value).ISBN);
            Assert.Equal(newBook.Year, ((Book)((CreatedAtActionResult)result).Value).Year);
        }

        #endregion

        #region Post

        [Fact]
        public async void Put_ForValidId_Returns200()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            var bookToUpdate = new Book { Id = 1, Author = "aa", Title = "bb", ISBN = "ih", Year = 2019 };
            var updatedBook = new Book { Id = 1, Author = "bb", Title = "cc", ISBN = "jk", Year = 2020 };

            A.CallTo(() => bookRepository.Get(1)).Returns(Task.FromResult(bookToUpdate));

            // Act
            var result = await bookController.Put(1, updatedBook);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Put_ForInvalidId_Returns404WithMessage()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            var updatedBook = new Book { Id = 1, Author = "bb", Title = "cc", ISBN = "jk", Year = 2020 };

            A.CallTo(() => bookRepository.Get(1)).Returns((Book) null);

            // Act
            var result = await bookController.Put(1, updatedBook);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The book to update record couldn't be found.", ((NotFoundObjectResult)result).Value);
        }

        #endregion

        #region Delete

        [Fact]
        public async void Delete_ForValidId_Returns200()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);
            var bookToDelete = new Book { Id = 1, Author = "bb", Title = "cc", ISBN = "jk", Year = 2020 };

            A.CallTo(() => bookRepository.Get(1)).Returns(Task.FromResult(bookToDelete));

            // Act
            var result = await bookController.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void Delete_ForInvalidId_Returns404WithMessage()
        {
            // Arrange
            var bookRepository = A.Fake<IBookRepository<Book>>();
            var bookController = MakeController(bookRepository);

            A.CallTo(() => bookRepository.Get(1)).Returns((Book) null);

            // Act
            var result = await bookController.Delete(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("The book to delete record couldn't be found.", ((NotFoundObjectResult)result).Value);
        }

        #endregion

        #region Helpers

        private static BookController MakeController(IBookRepository<Book> bookRepository)
        {
            var controller = new BookController(bookRepository);
            var validator = A.Fake<IObjectModelValidator>();
            controller.ObjectValidator = validator;
            return controller;
        }

        #endregion
    }
}
