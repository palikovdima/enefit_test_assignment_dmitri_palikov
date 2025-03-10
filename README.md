# BakeSale

BakeSale is a web application for managing a bake sale in multiple sessions, including product listings, cart management, and checkout processes. The project is built with .NET 8 for the backend and a Vite/React/TypeScript client application.

## Table of Contents

- [Docker Images](#docker-images)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Usage](#usage)
- [Evaluation](#evaluation)
- [License](#license)


## Docker Images
!!! May encounter issues running the backend, due to "Config directory not found" exception. <br>

Backend image
```bash
docker pull dmpali/bakesale-backend:latest
```
Database image
```bash
docker pull dmpali/bakesale-db:latest
```
Frontend image
```bash
docker pull dmpali/bakesale-frontend:latest
```
Run the containers
```bash
docker run -d -p 5433:5432 --name bakesale-db dmpali/bakesale-db:latest
```
```bash
docker run -d -p 7190:7190 --name bakesale-backend  dmpali/bakesale-backend:latest
```
```bash
docker run -d -p 62170:80 --name bakesale-frontend  dmpali/bakesale-frontend:latest
```


## Project Structure

The project is organized into the following main folders:

- **BakeSale.API**: Contains the API controllers, helpers, and configuration files.
- **BakeSale.Domain**: Contains the domain entities, services, and interfaces.
- **BakeSale.Infrastructure**: Contains data access implementations, repositories, and migrations.
- **BakeSale.Tests**: Contains unit tests.
- **ClientApp**: Contains the frontend code (Vite + React + TypeScript).

## Setup Instructions

### Prerequisites

- **.NET 8 SDK**
- **Vite**
- **PostgreSQL**

### Backend Setup

1. Clone the repository:

```bash
git clone https://github.com/palikovdima/enefit_test_assignment_dmitri_palikov.git
```

2. Navigate to the solution folder:
   
```bash
cd enefit_test_assignment_dmitri_palikov
```

2. Navigate to the API project folder:

```bash
cd BakeSale.API
```

3. Change the variables(ImagePath, CSVPath, CurrenciesPath, CurrencyImagesPath) to your machine local paths in appsettings.json file. <br>

file location : BakeSale\BakeSale.API\appsettings.json<br>


4. Restore the .NET dependencies:

```bash
dotnet restore
```

5. Update the database:

```bash
dotnet ef database update
```

6. Run the API:

```bash
dotnet run
```

### Client Setup


1. Navigate to the client application folder:

```bash
cd ClientApp/clientapp.client
```

2. Install the npm dependencies:

```bash
npm install
```

3. Start the client application:

```bash
dotnet run
```

## Usage


This application uses cookies and session-based storage for managing user sessions.  <br>
Some browsers might have strict privacy settings that could block cookies or session storage.  <br>
Try using multiple browsers to see how one user's actions affect other sessions. <br>
Cart cookie's lifespan is 30 minutes after the creation date. <br>

The API runs at: https://localhost:7190 <br>
The Frontend runs at: https://localhost:62170 <br>
Swagger UI for API documentation: https://localhost:7190/swagger


## Evaluation


If I had more time, I would have made the following improvements to the project:<br>

Less Hardcoded Values: <br> (7.03 DONE)
* I would have used less hardcoded values and replaced remaining with configuration files, making the application more flexible and easier to maintain.<br>
* (7.03 update): I still have some hardcoded values like: information/warning/error strings and cookie/hub names hardcoded, but they are not so critical right now. <br>

Integration Tests: <br>
* I would have implemented integration tests to ensure the application's components work seamlessly together, and to cover end-to-end functionality more effectively.<br>

Admin Page and Input Fields: <br>
* I would have added an admin page where items could be inserted or updated via input fields, providing a more user-friendly interface for managing products and cart data.<br>

Docker Configuration: <br> (9.03 DONE with vulnerabilities ("Config directory not found" error on manual(process) testing))
* I would have configured Docker containers for both the backend application and the database to streamline deployment.<br>




## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
