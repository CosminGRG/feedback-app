# Feedback App
Web application where users can leave and browser feedback.
## Overview
FeedbackApp is a web application built with ASP.NET Core, Blazor, MSSQL, and Entity Framework. It allows users to leave feedback on various topics as determined by the owner. 
The app offers user authentication via GitHub, filtering, and search capabilities, and administrative functions for managing feedback and users.

### Features
- GitHub Authentication
- Feedback Creation / Deletion
- Filtering by User
- Search Feedback
- View Own Feedback
### Admin Features
- Delete Feedback
- Block Users

### Technologies Used
- ASP.NET Core
- Blazor
- MSSQL
- Entity Framework Core

## Getting Started
### Prerequisites
- .NET 8.0 or later
- SQL Server or SQL Server Express
- GitHub OAuth credentials (Client ID and Client Secret)

### Installation
1. Clone the repository:
`git clone https://github.com/CosminGRG/feedback-app.git`
2. Open the project in Visual Studio.
3. Configure the database connection:
   - In Visual Studio, open the appsettings.json file.
   - Update the connection string to point to your MSSQL database.
4. Set up GitHub OAuth:
   - Register your application on GitHub to get the Client ID and Client Secret.
   - In appsettings.json, update the GitHub authentication settings with your Client ID and Client Secret.
5. Apply Migrations and Seed Data:
   - Open the Package Manager Console in Visual Studio (Tools > NuGet Package Manager > Package Manager Console).
   - Run the following command: `Update-Database`
6. Run the application.
7. Open your browser and navigate to: `https://localhost:5001`
