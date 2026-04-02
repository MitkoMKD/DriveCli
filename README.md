# DriveCli
This is a .NET CLI application that integrates with the Google Drive API to provide file synchronization, search, and upload functionality.

The application demonstrates:
- OAuth 2.0 authentication
- Parallel file synchronization
- Thread-safe operations
- Clean architecture and separation of concerns
- Robust error handling

## Features

- Authenticate with Google Drive using OAuth 2.0
- Sync files from Google Drive to local machine (parallel downloads)
- Search files in Google Drive with local sync status
- Upload local files to Google Drive
- Progress bar for sync operations
- Folder structure preservation during sync

## Architecture

The application follows a layered architecture with clear separation of concerns:

- Commands → CLI interaction
- Services → Business logic
- Infrastructure → External integrations (Google API, File System)
- Models → Data structures

Key services:
- AuthService → Handles OAuth authentication
- GoogleDriveService → Encapsulates Google Drive API calls
- SyncService → Handles synchronization logic with parallel processing
- FileSystemService → Handles local file operations

## Parallelism & Thread Safety

File downloads are executed in parallel using SemaphoreSlim to limit concurrency and avoid overwhelming system resources.

Thread safety is ensured using Interlocked operations for updating shared counters (success/failure statistics).

## Setup

1. Clone the repository
2. Install .NET 8 SDK
3. Go to Google Cloud Console
4. Enable Google Drive API
5. Create OAuth Client (Desktop App)
6. Download client_secret.json
7. Rename it to client_secret.json
8. Add to Config folder (create if not exists)
9. Place it in the root directory of the project

## Usage

### Sync files
dotnet run sync

### Search files
dotnet run search "file-name"

### Upload file
dotnet run upload "local-file-path" "drive-folder-path"

## Notes

- The application runs in OAuth test mode (no Google verification required)
- Only authorized test users can authenticate
- client_secret.json and token.json are excluded from version control