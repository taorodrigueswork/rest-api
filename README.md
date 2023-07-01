# REST API TEMPLATE .NET 7 

This is a template for building RESTful APIs using .NET 7. It includes a basic project structure and some common components such as logging, and database connectivity. 

To get started, clone this repository and run the application using Visual Studio 2022 or the .NET CLI. 

## Project Structure

- `API`: This folder contains the source code for the web API.
  - `Controllers`: This folder contains the controllers that handle incoming requests.
- `Entities`: This folder contains the entity models.
  - `DTO`: This folder contains the data transfer objects (DTOs) used for request and response payloads.
  - `Entity`: This folder contains the database entities. There is a many-to-many relationship example with an explicit class to handle it. There is also a BaseEntity class with a generic validate method for the entities.
  - `MapperProfile`: This folder contains the AutoMapper mappings between DTOs and entity models.
- `Persistency`: This folder contains the data access layer components, using Entity Framework Core ORM.
  - `Context`: It has an ApiContext that inherits from DbContext, which is a class provided by Entity Framework Core that represents a session with the database and allows you to query and save instances of your entity classes.

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
