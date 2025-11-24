using SQLite;

namespace Gestion_RDV.Models
{
    [Table("rendezvous")]
    public class RendezVous
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [MaxLength(100)]
        public string Titre { get; set; }
        
        public DateTime Date { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
    }
}