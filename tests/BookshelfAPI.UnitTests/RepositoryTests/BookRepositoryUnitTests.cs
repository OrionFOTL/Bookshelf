using BookshelfAPI.DbContexts;
using BookshelfAPI.Model;
using BookshelfAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BookshelfAPI.UnitTests.RepositoryTests
{
    public class BookRepositoryUnitTests
    {
        #region GetAll

        [Fact]
        public async void GetAll_ForEmptyBookRepo_ReturnsEmptyList()
        {
            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                // Act
                var result = await bookRepository.GetAll();

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async void GetAll_ForBooksInRepo_ReturnsBooksList()
        {
            // Arrange
            var book1 = MakeBook(1);
            var book2 = MakeBook(2);

            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            {
                await context.Books.AddAsync(book1);
                await context.Books.AddAsync(book2);
                await context.SaveChangesAsync();
            }

            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                // Act
                var result = await bookRepository.GetAll();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, ((List<Book>)result).Count);
            }
        }

        #endregion

        #region Get

        [Fact]
        public async void Get_ForNonExistingBook_ReturnsNull()
        {
            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                // Act
                var result = await bookRepository.Get(1);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async void Get_ForExistingBook_ReturnsBook()
        {
            // Arrange
            var book = MakeBook(1);

            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            {
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }
            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                // Act
                var result = await bookRepository.Get(1);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(book.Id, result.Id);
                Assert.Equal(book.Title, result.Title);
                Assert.Equal(book.Author, result.Author);
                Assert.Equal(book.ISBN, result.ISBN);
                Assert.Equal(book.Year, result.Year);
            }
        }

        #endregion

        #region Add

        [Fact]
        public async void Add_ForValidBook_AddsBook()
        {
            // Arrange
            var book = MakeBook(1);

            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                // Act
                await bookRepository.Add(book);

                // Assert
                var result = await context.Books.SingleOrDefaultAsync();
                Assert.NotNull(result);
                Assert.Equal(book.Id, result.Id);
                Assert.Equal(book.Author, result.Author);
                Assert.Equal(book.ISBN, result.ISBN);
                Assert.Equal(book.Title, result.Title);
                Assert.Equal(book.Year, result.Year);
            }
        }

        #endregion

        #region Update

        [Fact]
        public async void Update_ForValidParameters_UpdatesBook()
        {
            // Arrange
            var book = MakeBook(1);
            var updatedBook = new Book()
            {
                Id = 1,
                Title = "updated title",
                Author = "updated author",
                ISBN = "1412newisbn25235",
                Year = 2011
            };

            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            {
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                book = await context.Books.SingleOrDefaultAsync(b => b.Id == book.Id);

                await bookRepository.Update(book, updatedBook);

                // Assert
                var result = await context.Books.Where(b => b.Id == updatedBook.Id
                    && b.Title == updatedBook.Title
                    && b.Author == updatedBook.Author
                    && b.ISBN == updatedBook.ISBN
                    && b.Year == updatedBook.Year).SingleOrDefaultAsync();
                Assert.NotNull(result);
            }

        }

        #endregion

        #region Delete

        [Fact]
        public async void Delete_ForValidId_DeletesBook()
        {
            // Arrange
            var book = MakeBook(1);
            var options = MakeBookContextOptions();
            using (var context = new BookContext(options))
            {
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new BookContext(options))
            using (var bookRepository = new BookRepository(context))
            {
                await bookRepository.Delete(book);
                var books = context.Books;

                // Assert
                Assert.Empty(books);
            }
        }

        #endregion

        #region Helpers

        private DbContextOptions<BookContext> MakeBookContextOptions()
        {
            return new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private Book MakeBook(int id)
        {
            return new Book { Id = id, Title = "Example title", Author = "Example author", ISBN = "k2h523h5l2", Year = 2019 };
        }
        #endregion
    }
}
