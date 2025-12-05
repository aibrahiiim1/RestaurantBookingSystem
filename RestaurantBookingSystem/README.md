# Restaurant Booking System - Complete OpenTable Clone

A comprehensive restaurant reservation system built with ASP.NET Core 8, featuring admin panels for restaurant management and a guest-facing booking interface.

## 🌟 Features

### Guest/Customer Features
- **Homepage**
  - Featured restaurants slider
  - Restaurant search (by location, date, time, party size)
  - Quick access to cuisines and filters
  - How it works guide
  - Recent reviews showcase

- **Restaurant Search & Discovery**
  - Advanced filtering (cuisine, price range, features)
  - Sorting options (featured, newest, highest rated, distance, child-friendly)
  - Horizontal scroll filters (cuisines, occasions, special features)
  - Real-time availability checking

- **Restaurant Detail Page**
  - Photo gallery with primary image
  - Complete restaurant information
  - Reviews and ratings (overall, food, service, ambiance)
  - Opening hours and special events
  - Active promotions
  - Interactive map location
  - Real-time booking widget

- **Booking System**
  - Date and time selection
  - Party size configuration
  - Table location preference (standard, outdoor, terrace, beach view)
  - Dynamic time slot availability
  - Full calendar view modal
  - Occasion selection (birthday, anniversary, business, etc.)
  - Special requests field
  - Email/Phone verification with OTP
  - Booking confirmation with reference number

- **Reservation Management**
  - View upcoming and past reservations
  - Cancel reservations (based on restaurant policy)
  - Modify reservations
  - Get directions to restaurant
  - Message restaurant directly
  - Add/update occasion and special requests

- **Customer Account**
  - Registration and login
  - Profile management
  - Allergy tracking
  - Favorite restaurants
  - Reservation history
  - Loyalty points (extensible)

- **Reviews**
  - Write verified reviews (for completed reservations)
  - Multi-aspect ratings (food, service, ambiance)
  - Browse restaurant reviews

### Admin Panel Features

- **Dashboard**
  - Today's reservations overview
  - Monthly statistics
  - Pending reservations count
  - Unread messages
  - Revenue analytics (extensible)
  - Reservation status breakdown

- **Restaurant Management**
  - Multi-restaurant support
  - Brand/chain management (multiple branches)
  - Complete restaurant profile
  - Logo and photo management
  - Cuisine assignment
  - Location with coordinates
  - Contact information
  - Capacity management
  - Amenities (child-friendly, wheelchair access, etc.)
  - Booking configuration (duration, time slots, cancellation policy)

- **Table Management**
  - Add/edit/delete tables
  - Table numbers and capacity
  - Location types (standard, outdoor, terrace, beach view, bar, private, window)
  - Availability status
  - View active reservations per table

- **Reservation Management**
  - View all reservations (filter by date, status)
  - Reservation details
  - Update status (confirmed, seated, completed, no-show)
  - Cancel reservations with reason
  - Customer information
  - Special requests and allergies
  - Table assignment
  - Reservation timeline

- **Opening Hours Management**
  - Set hours for each day of the week
  - Mark closed days
  - Special closure dates
  - Holiday management

- **Menu Management**
  - Add/edit/delete menu items
  - Categories (appetizers, mains, desserts, etc.)
  - Pricing
  - Item descriptions and images
  - Availability toggle

- **Photo Gallery**
  - Upload multiple photos
  - Set primary photo
  - Display order management
  - Photo categories (interior, exterior, food, etc.)

- **Promotions & Offers**
  - Create limited-time promotions
  - Discount percentages
  - Promo codes
  - Valid date ranges
  - Active/inactive toggle

- **Special Events**
  - Birthday packages
  - Wedding events
  - Romantic dinners
  - Business events
  - Event-specific reservations

- **Reviews Management**
  - View all reviews
  - Rating analytics
  - Rating distribution
  - Respond to reviews (extensible)

- **Messages**
  - Customer message inbox
  - Reply to customer inquiries
  - Unread message notifications
  - Per-reservation messaging

- **Staff Management**
  - Add/edit staff users
  - Role assignment (SuperAdmin, RestaurantAdmin, Manager, Staff)
  - Access control
  - Activity tracking

- **Analytics**
  - Reservation trends
  - Popular time slots
  - Party size distribution
  - No-show and cancellation rates
  - Top customers
  - Revenue reports (extensible)

## 🏗️ Technical Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server (Entity Framework Core)
- **Authentication**: Cookie-based authentication (separate for customers and admin)
- **Frontend**: Razor Views with modern CSS and JavaScript
- **Architecture**: MVC with Repository Pattern

### Project Structure
```
RestaurantBookingSystem/
├── Controllers/
│   ├── HomeController.cs
│   ├── RestaurantController.cs
│   ├── BookingController.cs
│   ├── AdminController.cs
│   └── CustomerController.cs
├── Models/
│   ├── Restaurant.cs
│   ├── Reservation.cs
│   ├── Customer.cs
│   ├── Table.cs
│   ├── User.cs
│   └── SupportingModels.cs
├── ViewModels/
│   ├── GuestViewModels.cs
│   └── AdminViewModels.cs
├── Services/
│   ├── RestaurantService.cs
│   ├── ReservationService.cs
│   ├── CustomerAuthServices.cs
│   └── Interfaces/
├── Data/
│   └── ApplicationDbContext.cs
├── Views/
│   ├── Home/
│   ├── Restaurant/
│   ├── Booking/
│   ├── Admin/
│   └── Shared/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
└── Program.cs
```

### Database Schema

**Core Tables:**
- Restaurants
- Tables
- Customers
- Reservations
- Users
- Roles

**Supporting Tables:**
- Cuisines
- MealTypes
- Occasions
- OpeningTimes
- RestaurantClosures
- Attachments (Photos)
- MenuItems
- SpecialEvents
- Promotions
- RestaurantReviews
- CustomerAllergies
- FavoriteRestaurants
- ReservationMessages
- Principals
- Logs

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full installation)
- Visual Studio 2022 or VS Code
- Node.js (optional, for frontend tooling)

### Installation

1. **Clone or extract the project**
```bash
cd RestaurantBookingSystem
```

2. **Update Connection String**
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RestaurantBookingDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

For SQL Server, use:
```json
"DefaultConnection": "Server=localhost;Database=RestaurantBookingDb;User Id=your_user;Password=your_password;TrustServerCertificate=true"
```

3. **Restore NuGet packages**
```bash
dotnet restore
```

4. **Apply Database Migrations**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. **Run the application**
```bash
dotnet run
```

6. **Access the application**
- Guest Site: `https://localhost:7000`
- Admin Panel: `https://localhost:7000/Admin/Login`

### Default Admin Credentials
```
Username: admin
Password: Admin@123
```

**⚠️ IMPORTANT: Change these credentials immediately in production!**

## 📋 Configuration

### Booking Settings
Edit in `appsettings.json` or per-restaurant in admin panel:
```json
"AppSettings": {
  "DefaultBookingDuration": 120,        // minutes
  "TimeSlotInterval": 30,               // minutes
  "CancellationPolicyHours": 5,         // hours before reservation
  "VerificationCodeExpiry": 15          // minutes
}
```

### Email Configuration
To enable email notifications, integrate with your email provider (SendGrid, AWS SES, etc.):

1. Update `IEmailService` implementation in `Services/CustomerAuthServices.cs`
2. Add email provider credentials to `appsettings.json`
3. Implement actual email sending logic

## 💻 Usage Guide

### For Restaurant Administrators

1. **Initial Setup**
   - Login with admin credentials
   - Update restaurant settings
   - Upload logo and photos
   - Set opening hours
   - Configure cancellation policy

2. **Add Tables**
   - Navigate to Tables section
   - Add tables with numbers, capacity, and location types
   - Ensure sufficient tables for expected capacity

3. **Manage Reservations**
   - View daily reservations on dashboard
   - Update reservation statuses
   - Respond to customer messages
   - Handle no-shows and cancellations

4. **Add Content**
   - Upload menu items
   - Create promotions
   - Add special events
   - Update photos regularly

5. **Monitor Performance**
   - Check analytics dashboard
   - Review customer feedback
   - Analyze booking patterns
   - Optimize time slots and capacity

### For Customers

1. **Search Restaurants**
   - Enter location, date, time, and party size
   - Apply filters (cuisine, features)
   - Sort results as needed

2. **Make Reservation**
   - Select restaurant
   - Choose date and time
   - Select preferred table location
   - Enter personal details
   - Choose occasion and add special requests
   - Verify email with OTP code
   - Receive confirmation

3. **Manage Bookings**
   - View upcoming reservations
   - Cancel or modify (if within policy)
   - Message restaurant
   - Get directions

4. **After Visit**
   - Write reviews
   - Rate food, service, and ambiance
   - Add to favorites

## 🎨 Customization

### Adding New Cuisines
```sql
INSERT INTO Cuisines (Name) VALUES ('Your Cuisine Name');
```

### Adding New Occasions
```sql
INSERT INTO Occasions (Name) VALUES ('Your Occasion Name');
```

### Customizing Roles
Edit seed data in `ApplicationDbContext.cs` and update role permissions in controllers.

### Styling
- Main stylesheet: `wwwroot/css/site.css`
- Admin stylesheet: `wwwroot/css/admin.css`
- Customize colors, fonts, and layouts as needed

## 🔐 Security Considerations

1. **Change Default Credentials**: Immediately change admin password
2. **Use HTTPS**: Enable HTTPS in production
3. **Secure Connection Strings**: Use Azure Key Vault or environment variables
4. **Implement Rate Limiting**: Protect APIs from abuse
5. **Enable CORS Properly**: Configure appropriate CORS policies
6. **SQL Injection Protection**: Entity Framework provides parameterized queries
7. **XSS Protection**: Razor automatically encodes output
8. **CSRF Protection**: ValidateAntiForgeryToken attributes in place

## 📱 Mobile Responsiveness

The application is designed to be mobile-responsive:
- Responsive grid layouts
- Touch-friendly buttons and inputs
- Mobile-optimized navigation
- Swipeable photo galleries

Test on various devices and browsers.

## 🔄 Future Enhancements

**Potential Features to Add:**
- SMS notifications
- Online payment integration
- Waitlist management
- Table merging for large parties
- Multi-language support
- Social media integration
- Advanced analytics and reporting
- Mobile apps (iOS/Android)
- Real-time table status updates
- Integration with POS systems
- Loyalty program automation
- Gift cards and vouchers
- Group booking management
- Virtual tours
- AI-powered recommendations

## 🐛 Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string format
- Ensure database exists
- Check user permissions

### Migration Errors
```bash
# Reset migrations if needed
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Authentication Issues
- Clear browser cookies
- Check authentication scheme names
- Verify claims are being set correctly

### Time Slot Issues
- Verify opening hours are set
- Check for restaurant closures
- Ensure tables exist and are available
- Verify booking duration and interval settings

## 📞 Support

For issues or questions:
1. Check this README
2. Review code comments
3. Examine Entity Framework logs
4. Check browser console for JavaScript errors

## 📄 License

This project is provided as-is for educational and commercial use.

## 👥 Contributing

To contribute:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 🙏 Acknowledgments

- Inspired by OpenTable
- Built with ASP.NET Core
- Uses Entity Framework Core
- Modern responsive design

---

**Built with ❤️ using ASP.NET Core 8**

For more information or custom development, please contact the development team.
