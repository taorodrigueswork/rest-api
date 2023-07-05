[![Build and Test](https://github.com/taorodrigueswork/rest-api/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/taorodrigueswork/rest-api/actions/workflows/dotnet.yml)

![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/taorodrigueswork/76adb9c1322aed1e5ba941d7550e7007/raw/test.json)

# REST API TEMPLATE WEB API .NET CORE 7 

This is a template for building RESTful APIs using .NET 7. It includes a basic project structure and some common components such as logging, and database connectivity. 

To get started, clone this repository and run the application using Visual Studio 2022 or the .NET CLI. 

## Requirements (required)

This project requires the following technologies:
- .NET 7
- C#
- SQL Server
- Entity Framework Core 7
- Serilog
- Seq or Elastic Search
- Automapper
- MSTest
- Swagger
- Global Error Middleware
- Repository Pattern
- Azure Key Vault

## Future Implementations

- OAuth
- Coverlet
- Docker File
- AKS
  

## Project Structure

- `API`: This folder contains the source code for the web API.
  - `Controllers`: This folder contains the controllers that handle incoming requests.
  - `Program`: This file is responsible for configuring and running the web host for the Rest API.
- `Entities`: This folder contains the entity models.
  - `DTO`: This folder contains the data transfer objects (DTOs) used for request and response payloads.
  - `Entity`: This folder contains the database entities. There is a many-to-many relationship example with an explicit class to handle it. There is also a BaseEntity class with a generic validate method for the entities.
  - `MapperProfile`: This file contains the AutoMapper mappings between DTOs and entity models.
- `Entities.Test`: This folder contains the entity tests using MSTest.
  - `MappingProfileTests`: It tests if all the properties are mapped correctly. Not a single property should be ignored. Helps to avoid mistakes when mapping entities and DTOs.
- `Persistency`: This folder contains the data access layer components, using Entity Framework Core ORM.
  - `Context`: It has an ApiContext that inherits from DbContext, which is a class provided by Entity Framework Core that represents a session with the database and allows you to query and save instances of your entity classes.
  - `Migrations`: Contain all migration files defining the changes to the model that should be applied to the database.
- `Business`: This folder contains the business logic components.
  - `IBusiness`: This folder contains the interfaces for the business logic components.

## Components

### Web API Project

All the configuration is done in the Program.cs file. After .NET6 we don't have to use Startup.cs anymore. And the code in the Program file is more concise and simple.

### Business

This project uses a generic Interface in all classes, in order to make it simple to make dependency injection. We only need to register one time in the Program.cs file and it is going to inject all business classes into the system commented.
All classes receive an AutoMapper and a Log via dependency injection.

### Logging

The logging component uses the built-in `Serilog` library. The logging level can be configured through the `appsettings.json` file. The logs are sent to Seq, but the configuration to log in an ElasticSearch sink is already in the Program.cs file.

### Database Connectivity

The database connectivity is handled by Entity Framework Core. The database connection string can be configured through the `appsettings.json` file.
All the migrations are going to run automatically when the application runs.

### DTOs

DTOs are used to prevent exposing entity models directly to the API consumers. It allows for maintaining a clear separation between the application and the database. AutoMapper library is used to map data between DTOs and entity models.

### Azure Key Vault

Azure Key Vault is a cloud service that can be used to store secrets and configuration data for .NET or ASP.NET Core applications. Azure Key Vault securely stores secret values and allows access to those values without any explicit code change to both applications hosted in Azure and users based on their Azure credentials.
