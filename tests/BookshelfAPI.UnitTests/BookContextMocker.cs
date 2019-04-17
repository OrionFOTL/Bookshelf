using BookshelfAPI.DbContexts;
using BookshelfAPI.Model;
using BookshelfAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookshelfAPI.UnitTests
{
    public static class BookContextMocker
    {
        public static IBookRepository<Book> GetInMemoryBookRepository(string dbName)
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            BookContext bookContext = new BookContext(options);
            bookContext.FillDatabase();

            return new BookRepository(bookContext);
        }
    }
}
