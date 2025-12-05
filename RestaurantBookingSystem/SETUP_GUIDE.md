# Restaurant Booking System - Complete Setup Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Initial Setup](#initial-setup)
3. [Database Configuration](#database-configuration)
4. [Running the Application](#running-the-application)
5. [First-Time Configuration](#first-time-configuration)
6. [Adding Sample Data](#adding-sample-data)
7. [Deployment](#deployment)
8. [Troubleshooting](#troubleshooting)

## Prerequisites

### Required Software
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (one of the following):
  - SQL Server 2019 or later (recommended for production)
  - SQL Server Express (free, suitable for development)
  - SQL Server LocalDB (included with Visual Studio)
- **Visual Studio 2022** (recommended) or **VS Code**
- **Git** (for version control)

### Optional Tools
- **SQL Server Management Studio (SSMS)** - For database management
- **Azure Data Studio** - Alternative database management tool
- **Postman** - For API testing

## Initial Setup

### Step 1: Extract Project Files

```bash
# Extract the project to your desired location
cd C:\Projects\RestaurantBookingSystem
```

### Step 2: Verify Project Structure

Ensure you have the following structure:
```
RestaurantBookingSystem/
├── Controllers/
├── Models/
├── ViewModels/
├── Services/
├── Data/
├── Views/
├── wwwroot/
├── Program.cs
├── appsettings.json
└── RestaurantBookingSystem.csproj
```

### Step 3: Restore NuGet Packages

Open terminal in the project directory:

```bash
dotnet restore
```

This will download all required packages:
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Authentication.Cookies

## Database Configuration

### Option 1: SQL Server LocalDB (Development)

**For Windows with Visual Studio:**

1. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RestaurantBookingDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. No additional setup needed - LocalDB is included with Visual Studio.

### Option 2: SQL Server Express (Development)

1. Download and install SQL Server Express from Microsoft.

2. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=RestaurantBookingDb;Integrated Security=true;TrustServerCertificate=true"
  }
}
```

### Option 3: SQL Server (Production)

1. Ensure SQL Server is installed and running.

2. Create a database user (optional but recommended):
```sql
CREATE LOGIN RestaurantAppUser WITH PASSWORD = 'YourStrongPassword123!';
CREATE USER RestaurantAppUser FOR LOGIN RestaurantAppUser;
ALTER ROLE db_owner ADD MEMBER RestaurantAppUser;
```

3. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RestaurantBookingDb;User Id=RestaurantAppUser;Password=YourStrongPassword123!;TrustServerCertificate=true"
  }
}
```

### Option 4: Azure SQL Database (Cloud)

1. Create Azure SQL Database through Azure Portal.

2. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=RestaurantBookingDb;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Running the Application

### Step 1: Build the Project

```bash
dotnet build
```

Fix any compilation errors if they occur.

### Step 2: Create and Apply Database Migrations

```bash
# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

**Expected Output:**
```
Build started...
Build succeeded.
Done. To undo this action, use 'dotnet ef migrations remove'

Build started...
Build succeeded.
Applying migration '20240101000000_InitialCreate'.
Done.
```

### Step 3: Run the Application

```bash
dotnet run
```

Or with hot reload:
```bash
dotnet watch run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shutdown.
```

### Step 4: Access the Application

Open your browser and navigate to:
- **Guest Site**: https://localhost:7000
- **Admin Panel**: https://localhost:7000/Admin/Login

## First-Time Configuration

### Default Admin Login

The system automatically creates a default admin user:

```
Username: admin
Password: Admin@123
```

**⚠️ SECURITY WARNING:** Change this password immediately!

### Step 1: Login as Admin

1. Navigate to https://localhost:7000/Admin/Login
2. Enter the default credentials
3. Click "Sign In"

### Step 2: Update Restaurant Information

1. Click on "Restaurant Settings" in the admin panel
2. Update the following:
   - Restaurant Name
   - Brand Name (if part of a chain)
   - Description
   - Contact Information (Phone, Email)
   - Address and Location
   - Select Cuisine Type
   - Set Total Seating Capacity

3. Configure Booking Settings:
   - Default Booking Duration: 120 minutes (recommended)
   - Time Slot Interval: 30 minutes (recommended)
   - Cancellation Policy: 5 hours (adjust as needed)

4. Set Restaurant Features:
   - ☑ Child Friendly
   - ☑ Wheelchair Access
   - ☑ Outdoor Seating (if applicable)
   - ☑ Bar Area (if applicable)

5. Click "Save Changes"

### Step 3: Set Opening Hours

1. Go to "Opening Hours" section
2. For each day of the week:
   - Set open time (e.g., 11:00 AM)
   - Set close time (e.g., 10:00 PM)
   - Check "Closed" for days you're not open
3. Save changes

### Step 4: Add Tables

1. Navigate to "Tables" section
2. For each table, add:
   - Table Number (e.g., 1, 2, 3...)
   - Seating Capacity (2, 4, 6, 8...)
   - Location Type (Standard, Outdoor, Terrace, etc.)
3. Example setup for a 50-seat restaurant:
   ```
   Table 1: 2 seats, Standard
   Table 2: 2 seats, Standard
   Table 3: 4 seats, Standard
   Table 4: 4 seats, Window
   Table 5: 4 seats, Outdoor
   Table 6: 6 seats, Standard
   Table 7: 8 seats, Private
   ```

### Step 5: Upload Photos

1. Go to "Photos" section
2. Upload restaurant photos:
   - Upload logo (recommended size: 200x200px)
   - Add interior photos
   - Add food photos
   - Set one photo as primary (main display)

### Step 6: Add Menu Items (Optional)

1. Navigate to "Menu" section
2. Add menu items with:
   - Name
   - Description
   - Price
   - Category (Appetizer, Main Course, etc.)
   - Photo (optional)

### Step 7: Change Admin Password

1. Click on your username in the top right
2. Select "Change Password"
3. Enter current password: Admin@123
4. Enter new strong password
5. Save changes

## Adding Sample Data

### Manual Method

Use the admin panel to add:
- Multiple restaurants (if managing multiple locations)
- Sample special events
- Promotional offers
- Staff users

### SQL Script Method

You can also run SQL scripts directly to add sample data:

```sql
-- Add sample cuisines (already seeded)
-- Add sample occasions (already seeded)

-- Add sample promotional offer
INSERT INTO Promotions (Title, Description, DiscountPercentage, StartDate, EndDate, IsActive, RestaurantId)
VALUES ('Early Bird Special', 'Get 20% off dinner reservations between 5-6 PM', 20, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 1, 1);

-- Add sample special event
INSERT INTO SpecialEvents (EventName, Description, StartDate, EndDate, RequiresReservation, RestaurantId)
VALUES ('Valentine''s Day Dinner', 'Special romantic dinner menu for Valentine''s Day', '2024-02-14', '2024-02-14', 1, 1);
```

## Deployment

### Deploying to IIS (Windows Server)

1. **Publish the Application**
```bash
dotnet publish -c Release -o ./publish
```

2. **Install IIS and Required Features**
   - Open Server Manager
   - Add Roles and Features
   - Select Web Server (IIS)
   - Include ASP.NET Core Hosting Bundle

3. **Create IIS Site**
   - Open IIS Manager
   - Right-click Sites → Add Website
   - Set Physical Path to your publish folder
   - Configure bindings (port 80/443)

4. **Configure Application Pool**
   - Set .NET CLR Version to "No Managed Code"
   - Set Identity to appropriate user

5. **Update Connection String**
   - Edit appsettings.json in publish folder
   - Use production connection string

### Deploying to Azure App Service

1. **Create Azure App Service**
```bash
az webapp create --resource-group MyResourceGroup --plan MyAppServicePlan --name MyRestaurantApp --runtime "DOTNET|8.0"
```

2. **Configure Connection String**
```bash
az webapp config connection-string set --name MyRestaurantApp --resource-group MyResourceGroup --settings DefaultConnection="Server=..." --connection-string-type SQLAzure
```

3. **Deploy**
```bash
dotnet publish -c Release -o ./publish
cd publish
zip -r ../deploy.zip .
az webapp deployment source config-zip --resource-group MyResourceGroup --name MyRestaurantApp --src ../deploy.zip
```

### Deploying to Docker

1. **Create Dockerfile**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RestaurantBookingSystem.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RestaurantBookingSystem.dll"]
```

2. **Build and Run**
```bash
docker build -t restaurant-booking .
docker run -p 8080:80 -e "ConnectionStrings__DefaultConnection=YourConnectionString" restaurant-booking
```

## Troubleshooting

### Problem: Cannot connect to database

**Solution:**
1. Verify SQL Server is running
2. Check connection string format
3. Test connection using SSMS
4. Ensure firewall allows connection
5. Verify user permissions

### Problem: Migration fails

**Solution:**
```bash
# Drop database and start fresh
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Problem: Cannot login as admin

**Solution:**
1. Check if user was created by checking database
2. Ensure password is exactly: Admin@123
3. Try resetting password through database:
```sql
-- Password hash for "Admin@123"
UPDATE Users SET PasswordHash = 'YOUR_HASH_HERE' WHERE Username = 'admin';
```

### Problem: Time slots not showing

**Solution:**
1. Verify opening hours are set for the restaurant
2. Check that tables exist and are available
3. Ensure date is not marked as closure date
4. Verify booking configuration (duration, interval)

### Problem: Photos not displaying

**Solution:**
1. Check that photos are uploaded to wwwroot/images
2. Verify file paths in database are correct
3. Ensure web server has permission to serve static files
4. Check browser console for 404 errors

### Problem: Email verification not working

**Solution:**
1. Email service is mock by default
2. Check console output for verification codes
3. To enable real emails:
   - Integrate SendGrid, AWS SES, or SMTP
   - Update EmailService implementation
   - Add credentials to appsettings.json

### Problem: Performance is slow

**Solution:**
1. Add indexes to frequently queried columns
2. Enable response caching
3. Optimize database queries
4. Add Redis for session storage
5. Use CDN for static files
6. Enable output caching

## Support and Maintenance

### Regular Maintenance Tasks

1. **Database Backup** (Daily)
```sql
BACKUP DATABASE RestaurantBookingDb 
TO DISK = 'C:\Backups\RestaurantBookingDb.bak'
```

2. **Log Cleanup** (Weekly)
```sql
DELETE FROM Logs WHERE CreatedTime < DATEADD(MONTH, -3, GETDATE());
```

3. **Review Analytics** (Weekly)
   - Check reservation trends
   - Monitor no-show rates
   - Review customer feedback

4. **Update Dependencies** (Monthly)
```bash
dotnet list package --outdated
dotnet add package Microsoft.EntityFrameworkCore --version X.X.X
```

### Monitoring

Set up monitoring for:
- Application availability
- Response times
- Error rates
- Database performance
- Disk space
- Memory usage

### Logging

Logs are stored in the Logs table. Configure external logging:
- Azure Application Insights
- Seq
- Elasticsearch
- Serilog

## Next Steps

1. ✅ Complete initial setup
2. ✅ Configure restaurant details
3. ✅ Add tables and opening hours
4. ✅ Upload photos
5. ✅ Test booking flow
6. 📧 Configure email service
7. 💳 Integrate payment gateway (optional)
8. 📊 Set up analytics
9. 🔒 Configure SSL certificate
10. 🚀 Deploy to production

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [SQL Server Documentation](https://docs.microsoft.com/sql)

---

**Congratulations!** Your restaurant booking system is now ready to use. For questions or issues, refer to the README.md file or check the troubleshooting section.
