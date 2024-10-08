﻿using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MusicPortal.Models.DataBase
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public bool IsAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public string Timestamp { get; set; }
        public ICollection<Music> Musics { get; set; }
        public User()
        {
            Timestamp = DateTime.Now.ToString("f", CultureInfo.CurrentCulture);
            Musics = new List<Music>();
        }
    }
}
