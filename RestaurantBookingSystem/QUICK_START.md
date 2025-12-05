# Quick Start Guide - Restaurant Booking System

## 🚀 Get Started in 5 Minutes

### Step 1: Prerequisites Check
```bash
# Verify .NET 8 is installed
dotnet --version
# Should show: 8.0.x
```

### Step 2: Navigate to Project
```bash
cd RestaurantBookingSystem
```

### Step 3: Restore Packages
```bash
dotnet restore
```

### Step 4: Update Database Connection

Edit `appsettings.json` if needed (default uses LocalDB):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RestaurantBookingDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Step 5: Create Database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 6: Run Application
```bash
dotnet run
```

### Step 7: Access Application

- **Guest Site**: https://localhost:7000
- **Admin Panel**: https://localhost:7000/Admin/Login

### Step 8: Login as Admin

```
Username: admin
Password: Admin@123
```

⚠️ **Change this password immediately!**

## ⚡ Quick Configuration

### Add Your First Table
1. Login to admin panel
2. Navigate to "Tables"
3. Click "Add Table"
4. Enter: Table Number: 1, Capacity: 4, Location: Standard
5. Click "Save"

### Set Opening Hours
1. Go to "Opening Hours"
2. Set hours for each day (e.g., 11:00 AM - 10:00 PM)
3. Click "Save"

### Upload a Photo
1. Go to "Photos"
2. Click "Upload"
3. Select an image
4. Mark as "Primary Photo"
5. Click "Upload"

## 🎯 Test a Booking

1. Open https://localhost:7000 in a new browser window
2. Enter search criteria (tomorrow's date, 2 people, 7:00 PM)
3. Click "Find Restaurants"
4. Select your restaurant
5. Choose available time slot
6. Fill in guest details
7. Enter verification code from console output
8. Complete booking!

## 📱 Common Commands

### Run with hot reload (for development)
```bash
dotnet watch run
```

### View all migrations
```bash
dotnet ef migrations list
```

### Remove last migration
```bash
dotnet ef migrations remove
```

### Reset database
```bash
dotnet ef database drop
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Build for production
```bash
dotnet publish -c Release -o ./publish
```

## 🐛 Quick Fixes

### Cannot connect to database?
- Ensure SQL Server/LocalDB is running
- Check connection string in appsettings.json

### No tables showing?
- Add at least one table in admin panel
- Set opening hours for the restaurant

### Cannot login?
- Username is: admin (lowercase)
- Password is: Admin@123 (case sensitive)
- Check database has Users table with data

### No time slots available?
- Set opening hours for the day of week
- Add available tables
- Check restaurant is not marked as closed

## 📚 Next Steps

For detailed setup, see [SETUP_GUIDE.md](SETUP_GUIDE.md)

For full feature list, see [README.md](README.md)

## 🆘 Need Help?

1. Check console output for errors
2. Review browser console (F12) for client-side errors
3. Check database was created successfully
4. Ensure all migrations are applied

---

**Happy Booking! 🍽️**
