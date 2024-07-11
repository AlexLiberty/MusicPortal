using System.ComponentModel.DataAnnotations.Schema;

namespace MusicPortal.Models.DataBase
{
    public class Music
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public string? FilePath { get; set; }
        [NotMapped]
        public IFormFile? MusicFile { get; set; }
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }
    }
}
