# BookingApp

## Overview

BookingApp is a modular booking application built with C# and .NET, leveraging an **InMemory database** for development and testing. This simplifies setup and allows for rapid prototyping without requiring a persistent database.

## Project Structure

- **src/**: Contains core application logic, including models, services, and controllers.
- **tests/**: Includes unit and integration tests.
- **resources/**: Contains additional configuration files and assets.

## Prerequisites

Ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or higher)
- [Postman](https://www.postman.com/) (optional, for testing APIs)

## Getting Started

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/aklazy17/BookingApp.git
   cd BookingApp
   ```
   
2. **Restore Dependencies**:

   Navigate to the src directory and restore the necessary packages:

   ```bash
   cd src
   dotnet restore
   ```
   
3. **Run the Application**:
   
   Since the application is configured to use an InMemory database, no additional setup is needed for the database. Simply run:
   
   ```bash
   dotnet run
   ```
4. **Access Swagger API Documentation**:

   After running the application, open Swagger UI in your browser:

   `Swagger UI` (for HTTP)

   Swagger allows you to view, test, and interact with all API endpoints in a user-friendly interface.

## CSV File Processing with CSVHelper
The **CSVHelper** library is used to import and export booking data in CSV format efficiently.

## Running Tests

To execute the tests, navigate to the tests directory and run:

```bash
dotnet test
```

This command will run all tests, ensuring application stability.

## Acknowledgements
- **InMemory Database**: Used for quick development and testing.
- **CSVHelper**: Provides fast and reliable CSV file processing.
- **Swagger UI**: Enables easy API exploration and testing.
- **.NET & C# Community**: For resources and support.
