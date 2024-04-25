# Prerequisites
Before you begin, make sure you have the following installed:

- .NET SDK (version 5.0 or later)
- Visual Studio or Visual Studio Code

## If using Visual Studio Code:
Install the following extensions as they are required for working with .Net:
- [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) **(Required)**
- [.NET Extension Pack](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscode-dotnet-pack) **(Required)**
- [ASP.NET core VS Code Extension Pack](https://marketplace.visualstudio.com/items?itemName=temilaj.asp-net-core-vs-code-extension-pack) **(Optional but good to have)**
- [C# Extensions](https://marketplace.visualstudio.com/items?itemName=jchannon.csharpextensions) *(Optional)*
- [C# Namespace Autocompletion](https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.namespace) *(Optional)*
- [Entity Framework](https://marketplace.visualstudio.com/items?itemName=richardwillis.vscode-entity-framework) **(Optional but good to have)**
- [IntelliCode for C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscodeintellicode-csharp) *(Optional)*

# Getting Started
Clone the Repository:
```
git clone https://github.com/DiegoAvmor/todo-list-dot-net.git
git cd todo-list-dot-net
```

## Build the Solution:
Run the command ```dotnet build``` this should build and download any depedencies required by the application.

## Run the Application:
### If using Visual Studio:
Open the solution (TodoListApi.sln) in Visual Studio.
Set the startup project to the desired project (e.g., AwesomeDotNetApp.Web).
Press F5 to run the application.

### If using Visual Studio Code:
- Open the project folder in VS Code.
- Open a terminal and run:

```
dotnet run --project ./TodoListApi/TodoListApi.csproj --environment Local
```

This will run the application with and embedded database.

#### Running the app with a MySQL Database (Optional)
- Open the ```appsettings.Development.json``` file and update the following fields to match your MySQL configurations:
```
"DbConfig": {
    "Server": "localhost",
    "Port": "3306",
    "User": "root",
    "Password": "root",
    "Database": "todo-api"
},
```

- Install the dotnet-ef tool by running the command 
```
dotnet tool install dotnet-ef
```
- Run the following command to create the database and run the migrations
```
dotnet ef database update
```
- Run the following command:
```
dotnet run --project ./TodoListApi/TodoListApi.csproj
```
By default the app is going to pickup the Development appsettings environment file if we dont specify the environment.

## Access the App:
Open your web browser and navigate to http://localhost:8080/swagger/index.html (or the specified port).
You should see the Swagger web page for the API.

## Run Tests
To run the API tests, you must be situated under the ```../TodoListApi/TodoListApi.Tests``` folder and use the following command:

```
dotnet test -e ASPNETCORE_ENVIRONMENT=Test
```

This will run the tests with an embedded database, if you want to run the test on a MySQL database server just run ```dotnet test``` and make sure your database configuration in ```appsettings.Development.json``` is correct.