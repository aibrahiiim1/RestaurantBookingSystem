using System.ComponentModel.DataAnnotations;

namespace RestaurantBookingSystem.Models
{
    public class Table
    {
        public int Id { get; set; }
        
        public int TableNumber { get; set; }
        
        [Range(1, 50)]
        public int SeatingCapacity { get; set; }
        
        public TableLocation Location { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public int RestaurantId { get; set; }

        // Navigation property
        public Restaurant Restaurant { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }

    public enum TableLocation
    {
        Standard = 1,
        Outdoor = 2,
        Terrace = 3,
        BeachView = 4,
        BarArea = 5,
        Private = 6,
        Window = 7
    }
}
