using BookshelfAPI.DbContexts;
using BookshelfAPI.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookshelfAPI.UnitTests
{
    public static class BookContextExtensions
    {
        public static void FillDatabase(this BookContext dbContext)
        {
            dbContext.Books.Add
            (
                new Book
                {
                    Id = 1,
                    Title = "Zbrodnia i kara",
                    Author = "Leopold Staff",
                    ISBN = "498864865224",
                    Year = 1866
                }
            );

            dbContext.Add
            (
                new Book
                {
                    Id = 2,
                    Title = "Ostatnie życzenie",
                    Author = "Andrzej Sapkowski",
                    ISBN = "745684215575",
                    Year = 2001
                }
            );

            dbContext.Add
            (
                new Book
                {
                    Id = 3,
                    Title = "1984",
                    Author = "George Orwell",
                    ISBN = "35214865422584",
                    Year = 1949
                }
            );

            dbContext.SaveChanges();
        }
    }
}
