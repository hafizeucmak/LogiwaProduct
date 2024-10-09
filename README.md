# Logiwa Product Backend

This project contains the backend infrastructure for the Logiwa product. It is built using ASP.NET Core and Entity Framework Core for data access, and it follows a layered architecture to separate concerns between different domains like infrastructure, business logic, and APIs.

## Prerequisites

Before getting started, ensure you have the following installed on your development machine:

- .NET SDK (8.0 or higher)
- MSSQL Server
- A configured connection string in the `appsettings.json`

## Project Structure

The project follows a structured approach to separate concerns:

- **App**: Contains the main entry point for the API project.
- **Business**: Contains the business logic for the project.
- **Common**: Contains shared resources used throughout the project.
- **Domain**: Represents the core domain model of the application.
- **Infrastructure**: Contains data access and infrastructure-related services (like DbContext, migrations, repositories).

### Main Folders in Infrastructure

- **DbContexts**: Contains the `BaseDbContext` class and other database-related configurations.
- **Migrations**: Stores the migration files.
- **Repositories**: Contains repository implementations for data access.

## Setting Up Entity Framework Core Migrations

In this project, we use Entity Framework Core for data access and migrations. To initialize and apply migrations, follow the steps below:

### Step 1: Add a Migration

1. Open the terminal or package manager console in Visual Studio.
2. Navigate to the `Logiwa.Infrastructure` project directory.

    ```bash
    cd src/backend/Logiwa.Infrastructure
    ```

3. Run the following command to add a new migration. Replace `InitialCreate` with the name of your migration.

    ```bash
    dotnet ef migrations add InitialCreate --context BaseDbContext --output-dir Migrations --project ../Logiwa.Infrastructure
    ```

    This command creates the migration in the `Migrations` folder within the `Logiwa.Infrastructure` project.

### Step 2: Update the Database

After adding a migration, you need to apply it to the database.

1. Still within the terminal or package manager console, ensure you're in the root folder of the `Logiwa.Infrastructure` project.

2. Run the following command to update the database:

    ```bash
    dotnet ef database update --context BaseDbContext --project ../Logiwa.Infrastructure --startup-project ../Logiwa.WebAPI
    ```

    This will apply all pending migrations to your database.

### Step 3: Verify the Migration

After running the `update` command, you can check your database to see if the schema has been updated according to the migration. You can also check the `__EFMigrationsHistory` table in the database to verify the applied migrations.

## Filters

The project uses a custom filter called `TransactionManagerFilter` to manage database transactions for each request. The filter ensures that database operations are wrapped in a transaction, and commits or rolls back depending on whether the action succeeds or fails.

## NOTES
- This project follows the Clean Architecture principle, and CQRS is implemented to decouple commands from queries.
- Due to time constraints, only a unit test for one command has been added as an example.
- The Mediator pattern has been implemented using MediatR to handle communication between various parts of the application.


