# DiaryFriends
DiaryFriends is an ASP.NET Core MVC web application for keeping a personal diary and sharing entries with a single friend.
Live Demo: https://diaryfriends.runasp.net/
> Note: The live version is hosted on monsterasp.net for testing purposes. Please use fictional email addresses and passwords when trying out the demo.
---
## Tech Stack
* C# and ASP.NET Core MVC
* SQL Server and Entity Framework Core
* ASP.NET Core Identity (authentication and registration)
* HTML, CSS, Bootstrap
* Git and GitHub
---
## Features
* **Single-Friend System:**
  * Add a friend directly without sending requests or waiting for approval.
  * You can only have one friend at a time. To add a new friend, you must remove the current one first.
* **Shared Diary Access:**
  * Once connected, you can read your friend's diary entries.
* **Diary Management:**
  * Full functionality to add, view, edit, and delete your own entries.
  * Sorting buttons to organize entries by date (newest or oldest first).
* **Reactions:**
  * React to diary entries using emojis.
* **Streak System:**
  * Keep track of your daily writing habits. Maintain your streak by logging in and writing consistently!
* **Tic-Tac-Toe:**
  * Play a simple game of Tic-Tac-Toe with your friend directly from your page.
* **Profile Customization:**
  * Upload a profile picture (JPG, PNG, or WEBP, max 2MB).
  * Update your first name.
* **Security:**
  * Secure password and session handling via ASP.NET Core Identity.
---
## Update Log
**September 3, 2026**
* Added profile picture upload with validation (file type, size, and content checks).
* Added ability to update first name from the profile page.
* Visual redesign of the main page (profile cards, game board).
  
**August 30, 2026**
* Added **Tic-Tac-Toe**, a mini-game you can play with your friend.

**August 27, 2026**
* Added the **Reactions** feature, allowing users to leave emoji reactions on diary entries.
* Added **Streaks**.
---
## How to run locally
1. Clone the repo:
   git clone https://github.com/MihaelStipic/DiaryFriends.git
2. Add your local connection string to appsettings.Development.json.
3. Apply database migrations:
   dotnet ef database update
4. Run:
   dotnet run
