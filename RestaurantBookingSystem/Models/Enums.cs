namespace RestaurantBookingSystem.Models
{
    public enum ReviewStatus
    {
        Pending = 0,
        Published = 1,
        Hidden = 2,
        Flagged = 3,
        Removed = 4
    }

    public enum ReservationStatus
    {
        Pending = 0,
        Confirmed = 1,
        Seated = 2,
        Completed = 3,
        Cancelled = 4,
        NoShow = 5
    }

    public enum TableStatus
    {
        Available = 0,
        Reserved = 1,
        Occupied = 2,
        Maintenance = 3
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2,
        Refunded = 3,
        Cancelled = 4
    }

    public enum BookingSource
    {
        Website = 0,
        MobileApp = 1,
        Phone = 2,
        WalkIn = 3,
        ThirdParty = 4
    }
}
