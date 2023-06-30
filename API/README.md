# REST API TEMPLATE

This is a template for building RESTful APIs using .NET. It includes a basic project structure and some common components such as authentication, authorization, logging, and database connectivity. 

To get started, clone this repository and run the application using Visual Studio or the .NET CLI. 

## Project Structure

- `rest-api`: This folder contains the source code for the web API.
  - `Controllers`: This folder contains the controllers that handle incoming requests.
  - `DTO`: This folder contains the data transfer objects (DTOs) used for request and response payloads.
  - `Mappings`: This folder contains the AutoMapper mappings between DTOs and entity models.
  - `Middleware`: This folder contains custom middleware components.
  - `Services`: This folder contains the business logic services.
- `Data`: This folder contains the data access layer components.
  - `EntityFramework`: This folder contains the Entity Framework configuration files.
- `Entities`: This folder contains the entity models.
- `Business`: This folder contains the business logic components.
  - `IBusiness`: This folder contains the interfaces for the business logic components.
- `Infrastructure`: This folder contains the cross-cutting concerns components such as authentication, authorization, and logging.
  - `Auth`: This folder contains the authentication components.
  - `Auth/Interfaces`: This folder contains the authentication interfaces.
  - `Auth/JWT`: This folder contains the JWT authentication components.
  - `Auth/JWT/Interfaces`: This folder contains the JWT authentication interfaces.
  - `Authorization`: This folder contains the authorization components.
  - `Authorization/Interfaces`: This folder contains the authorization interfaces.
  - `Logging`: This folder contains the logging components.

## Components

### Logging

The logging component uses the built-in `Microsoft.Extensions.Logging` API. The logging level can be configured through the `appsettings.json` file.

### Authentication and Authorization

The authentication and authorization components have been implemented using JWT. The token generation and validation are handled by the `JwtService` class. Authorization is done using policies that are defined in `Startup`.

### Database Connectivity

The database connectivity is handled by Entity Framework Core. The database connection string can be configured through the `appsettings.json` file.

### DTOs

DTOs are used to prevent exposing entity models directly to the API consumers. It allows maintaining a clear separation between the application and the database. AutoMapper library is used to map data between DTOs and entity models.

### Middleware

Custom middleware components have been added to handle global exception handling and logging.
s