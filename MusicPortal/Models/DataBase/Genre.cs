using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models.DataBase
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
