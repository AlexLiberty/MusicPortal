﻿using MusicPortal.Models.DataBase;
using Microsoft.EntityFrameworkCore;

namespace MusicPortal.Models.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly MusicPortalContext _context;

        public GenreRepository(MusicPortalContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllGenres()
        {
            return await _context.Genres.ToListAsync() ?? Enumerable.Empty<Genre>();
        }

        public async Task<bool> AddGenre(string name)
        {
            // Проверка на существование жанра
            var genreExists = _context.Genres
                .AsEnumerable()  // Переводит данные на клиентскую сторону
                .Any(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (genreExists)
            {
                return false;  // Жанр уже существует
            }

            // Добавление нового жанра
            var genre = new Genre { Name = name };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateGenre(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Genre> GetGenreById(int id)
        {
            return await _context.Genres.FindAsync(id);
        }
    }
}