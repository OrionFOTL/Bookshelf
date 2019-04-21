using BookshelfAPI.DbContexts;
using BookshelfAPI.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookshelfAPI.Repository
{
    public class BookRepository : IBookRepository<Book>
    {
        private readonly BookContext _dbContext;

        public BookRepository(BookContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<Book> Get(long id)
        {
            return await _dbContext.Books.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task Add(Book entity)
        {
            await _dbContext.Books.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Book book, Book entity)
        {
            book.Title = entity.Title;
            book.Author = entity.Author;
            book.ISBN = entity.ISBN;
            book.Year = entity.Year;

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(Book book)
        {
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
