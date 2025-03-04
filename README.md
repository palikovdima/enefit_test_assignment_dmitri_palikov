# BakeSale

BakeSale is a web application for managing a bake sale in multiple sessions, including product listings, cart management, and checkout processes. The project is built with .NET 8 for the backend and a React/TypeScript client application.

## Table of Contents

- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Usage](#usage)
- [License](#license)

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


git clone https://github.com/palikovdima/enefit_test_assignment_dmitri_palikov.git


2. Navigate to the solution folder:

cd enefit_test_assignment_dmitri_palikov


2. Navigate to the API project folder:
   
cd BakeSale.API


3. Restore the .NET dependencies:

dotnet restore


4. Update the database:

dotnet ef database update


5. Run the API:

dotnet run


### Client Setup


1. Navigate to the client application folder:

cd ClientApp/clientapp.client


2. Install the npm dependencies:

npm install


3. Start the client application:

dotnet run


## Usage


This application uses cookies and session-based storage for managing user sessions. Some browsers might have strict privacy settings that could block cookies or session storage. 
Try using multiple browsers to see how one user's actions affect other sessions.
Cart cookie's lifespan is 30 minutes after the creation date.

The API runs at: https://localhost:7190 <br>
The Frontend runs at: https://localhost:62170 <br>
Swagger UI for API documentation: https://localhost:7190/swagger


## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
