# Employee Management System

A modern, feature-rich employee management desktop application built with C# and Windows Forms (.NET 10.0).

## Features

### Core Functionality
- **Employee Management**: Complete CRUD (Create, Read, Update, Delete) operations for employee records
- **Payroll Management**: Track and manage employee payroll records with bonuses, deductions, and payment status
- **Vacation Tracking**: Manage employee vacation requests with approval workflows and available days tracking
- **Statistics Dashboard**: View comprehensive analytics and insights about your workforce
- **Modern UI**: Clean, card-based interface with intuitive navigation and responsive design

### Key Capabilities
- Search and filter employees by name or ID
- Add vacation days to employee balances
- Generate payroll records with automatic net pay calculation
- Track vacation requests with approval dates and notes
- Backup and restore data functionality
- View detailed statistics about employees, payroll, and vacations

## Technology Stack

- **Framework**: .NET 10.0 (Windows Forms)
- **Language**: C# 13.0
- **Data Storage**: JSON-based local file storage
- **UI**: Modern Windows Forms with custom styling and graphics

## Data Storage

All application data is stored locally as JSON files in your system's AppData folder:

```
C:\Users\[YourUsername]\AppData\Roaming\EmployeeCRUD\
```

### Data Files
- **employees.json**: Contains all employee records with personal information, salary, and vacation balances
- **payroll.json**: Stores payroll records with payment history and calculations
- **vacations.json**: Tracks vacation requests, approvals, and related information

### Data Structure
Each JSON file maintains a clean, human-readable format with proper indentation. The data persists automatically after each operation (add, update, delete).

### Backup Your Data
You can backup your data in two ways:
1. **Using the App**: Go to Settings → Backup Data to create a timestamped backup folder
2. **Manual Copy**: Navigate to the data folder and copy the JSON files to a safe location

## Installation & Setup

### Prerequisites
- Windows 10 or later
- .NET 10.0 Runtime (included with the application or download from Microsoft)

### Running the Application

#### Option 1: Build from Source
1. Clone or download this repository
2. Open `EmployeeCRUD.sln` in Visual Studio 2022 or later
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

#### Option 2: Using the Batch File
1. Navigate to the project root directory
2. Double-click `rebuild-and-run.bat` to automatically build and launch the app
   - OR -
3. Double-click `run-app.bat` to run the previously built application

### First Launch
On first launch, the application will automatically:
- Create the data directory in AppData\Roaming\EmployeeCRUD
- Initialize empty JSON files for employees, payroll, and vacations
- Display the main dashboard ready for use

## Using the Application

### Navigation
Use the sidebar menu to access different sections:
- **Dashboard**: View and manage all employees
- **Add Employee**: Create new employee records
- **Payroll**: Manage payroll records and payments
- **Vacations**: Track and approve vacation requests
- **Statistics**: View workforce analytics
- **Settings**: Configure app settings and backup data

### Adding an Employee
1. Click "Add Employee" in the sidebar
2. Fill in the required information:
   - Roll Number (unique employee ID)
   - Name, Age, Department, Position
   - Email, Phone, Salary
   - Hire Date
   - Vacation days available
3. Click Save

### Managing Payroll
1. Click "Payroll" in the sidebar
2. View all payroll records or filter by employee
3. Click "Add Payroll Record" to create new entries
4. Enter pay period, base salary, bonuses, and deductions
5. Net pay is calculated automatically

### Tracking Vacations
1. Click "Vacations" in the sidebar
2. View all vacation requests
3. Click "Request Vacation" to create new requests
4. Select employee, dates, and vacation type
5. Add reason and approval information
6. Available vacation days are automatically checked

### Viewing Statistics
1. Click "Statistics" in the sidebar
2. View:
   - Total employees and average salary
   - Department distribution
   - Age demographics
   - Top performers
   - Payroll summaries
   - Vacation statistics

### Accessing Settings
1. Click "Settings" in the sidebar
2. View data storage location
3. See record counts and storage size
4. Open data folder in Explorer
5. Create timestamped backups

## Project Structure

```
EmployeeCRUD/
├── EmployeeCRUD.csproj          # Project configuration
├── Program.cs                    # Application entry point
├── ModernMainForm.cs             # Main dashboard interface
├── EmployeeFormEnhanced.cs       # Add/Edit employee form
├── PayrollForm.cs                # Payroll management interface
├── AddPayrollRecordForm.cs       # Add payroll record dialog
├── VacationForm.cs               # Vacation tracking interface
├── AddVacationRequestForm.cs     # Request vacation dialog
├── AddVacationDaysForm.cs        # Add vacation days dialog
├── StatisticsFormEnhanced.cs     # Statistics dashboard
├── SettingsForm.cs               # Settings and backup interface
├── LocalStorageRepository.cs     # Data access layer (JSON I/O)
├── DatabaseHelper.cs             # Storage path utilities
├── RepositoryFactory.cs          # Repository instantiation
├── IEmployeeRepository.cs        # Repository interface
└── Employee.cs                   # Data models (Employee, PayrollRecord, VacationRecord)
```

## Features in Detail

### Employee Records
- Unique roll number identification
- Personal information (name, age, email, phone)
- Employment details (department, position, hire date, salary)
- Vacation balance tracking (available days, used days)
- Soft delete functionality (maintains data integrity)
- Audit fields (creation date, last modified date)

### Payroll System
- Pay period tracking (start and end dates)
- Base salary, bonuses, and deductions
- Automatic net pay calculation
- Payment status (Pending, Paid, Cancelled)
- Payment date recording
- Notes and comments
- Employee association

### Vacation Management
- Multiple vacation types (Annual, Sick, Personal, Unpaid)
- Date range selection with automatic day calculation
- Request status (Pending, Approved, Denied)
- Approval workflow with approver name and date
- Reason and notes fields
- Automatic balance updates on approval
- Validation against available days

### Modern UI Design
- Card-based layout with rounded corners
- Gradient accents and hover effects
- Color-coded action buttons
- Intuitive navigation sidebar
- Responsive data grids
- Custom styled form controls
- Shadow effects and smooth animations

## Security & Data Privacy

- All data is stored locally on your machine
- No external connections or cloud services
- No telemetry or analytics collection
- Data remains under your complete control
- Regular backups recommended for data safety

## Troubleshooting

### Application Won't Start
- Ensure .NET 10.0 runtime is installed
- Check Windows Event Viewer for error details
- Verify AppData folder permissions

### Data Not Saving
- Check write permissions for AppData\Roaming\EmployeeCRUD
- Ensure sufficient disk space is available
- Verify JSON files are not locked by another process

### UI Display Issues
- Update Windows to the latest version
- Ensure display scaling is set correctly
- Check graphics driver updates

## Future Enhancements

Potential features for future versions:
- Report generation (PDF, Excel)
- Email notifications for approvals
- Advanced filtering and sorting
- Data import/export functionality
- Multi-user support with authentication
- Cloud synchronization option
- Mobile companion app

## Contributing

This is a standalone project. Feel free to fork and customize for your own needs.

## Version History

**Version 1.0** (Current)
- Complete employee CRUD operations
- Payroll management system
- Vacation tracking with approval workflow
- Statistics dashboard
- JSON-based local storage
- Modern Windows Forms UI
- Backup and restore functionality

## License

This project is provided as-is for educational and business use.

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review the application's Settings panel for system information
3. Verify data file integrity in the AppData folder

---

