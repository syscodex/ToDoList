📋 ToDoList Web Application
Show Image
Show Image
Show Image
🌟 Overview
ToDoList is a powerful task management web application designed to streamline your productivity. Built with cutting-edge technologies, it offers an intuitive and efficient way to manage your tasks from start to finish.
✨ Key Features
🚀 Task Management Capabilities

Comprehensive CRUD Operations

Effortlessly create, read, update, and delete tasks
Detailed task tracking with rich information
Multiple status options (Pending, In Progress, On Hold, Completed)


🎨 User-Friendly Interface

Flexible Visualization

Kanban-style task organization
Intelligent filtering mechanisms
Responsive design for all devices


🔧 Advanced Functionality

Productivity Boosters

One-click task export to Excel
Floating task creation form
Confirmation dialogs for critical actions



🛠 Technology Stack
TechnologyVersionPurposeASP.NET Core MVC9.0.0Web Application FrameworkEntity Framework Core9.0.0Database ORMBootstrapLatestResponsive UI DesignSQL Server2022Database Management
🚦 Prerequisites
Before diving in, ensure you have:

💻 .NET Core SDK 9.0.0 or later
🗃️ Microsoft SQL Server 2022
📝 Code Editor (Visual Studio, VS Code, etc.)

📦 Installation Guide

1️⃣ Clone the Repository
bashCopygit clone https://github.com/your-username/ToDoList.git
cd ToDoList
2️⃣ Configure Database Connection
Update appsettings.json:
jsonCopy{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=ToDoListDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
  }
}
3️⃣ Initialize Database
bashCopy# Restore dependencies
dotnet restore

# Apply migrations
dotnet ef database update
4️⃣ Launch Application
bashCopy# Run the application
dotnet run
🗂 Project Structure
CopyToDoList/
├── 📂 Controllers/    # Request handling logic
├── 📂 Data/           # Database configurations
├── 📂 Helpers/        # Utility classes
├── 📂 Migrations/     # Database schema changes
├── 📂 Models/         # Data models
└── 📂 Views/          # User interface components


🛠️ Troubleshooting
Common Challenges

Database Connection Issues

Verify SQL Server is running
Double-check connection string
Validate server authentication


Dependency Problems

Run dotnet restore
Confirm .NET SDK installation


Migration Errors

Remove existing migrations
Regenerate with dotnet ef migrations add InitialCreate
Update database: dotnet ef database update



