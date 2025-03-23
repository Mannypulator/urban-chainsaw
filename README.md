# EPS+ (Pension Contribution Management System)

## Overview

EPS+ is a comprehensive Pension Contribution Management System built with .NET Core 7.0, following Clean Architecture and Domain-Driven Design principles. The system manages pension contributions, member information, and automated background processes for a robust pension management solution.

## Features

- Member Management (Registration, Updates, Retrieval)
- Contribution Processing (Monthly and Voluntary)
- Automated Background Jobs
- Comprehensive Data Validation
- Benefit Eligibility Tracking
- Interest Calculations

## Technical Stack

- .NET Core 7.0
- Entity Framework Core
- SQL Server
- Hangfire (Background Jobs)
- Swagger/OpenAPI
- xUnit (Testing)

## Project Structure

The solution follows Clean Architecture principles with the following structure:

```
EPS/
├── src/
│   ├── Core/
│   │   ├── EPS.Domain        # Domain entities, interfaces, domain logic
│   │   └── EPS.Application   # Application services, interfaces, DTOs
│   ├── Infrastructure/
│   │   ├── EPS.Infrastructure    # External services, email, logging
│   │   └── EPS.Persistence      # Database context, repositories
│   └── Presentation/
│       └── EPS.API          # API Controllers, middleware
```

## Prerequisites

- .NET Core SDK 7.0 or later
- SQL Server
- Visual Studio 2022 or VS Code

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/EPS.git
cd EPS
```

### 2. Database Setup

- Update the connection string in `appsettings.json`
- Run database migrations:

```bash
dotnet ef database update
```

### 3. Run the Application

```bash
cd src/Presentation/EPS.API
dotnet run
```

The API will be available at `https://localhost:5001`

## API Documentation

- Swagger UI: `https://localhost:5001/swagger`
- Detailed API documentation is available in the `/docs` folder

## Testing

To run the tests:

```bash
dotnet test
```

## Architecture Decisions

- **Clean Architecture**: Ensures separation of concerns and maintainability
- **Domain-Driven Design**: Focus on core business logic and domain models
- **CQRS Pattern**: Separation of read and write operations
- **Repository Pattern**: Abstraction of data persistence
- **Unit of Work**: Ensures data consistency
- **Background Processing**: Hangfire for reliable job scheduling

## Security Considerations

- Input validation
- Authentication and Authorization
- Data encryption
- Secure communication
- Audit logging

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
