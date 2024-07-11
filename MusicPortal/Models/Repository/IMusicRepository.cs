﻿using MusicPortal.Models.DataBase;

namespace MusicPortal.Models.Repository
{
    public interface IMusicRepository
    {
        Task<IEnumerable<Music>> GetAllMusic();
        Task AddMusic(Music music, byte[] fileData);
        Task UpdateMusic(Music music, byte[] fileData = null);
        Task DeleteMusic(int id);
    }
}