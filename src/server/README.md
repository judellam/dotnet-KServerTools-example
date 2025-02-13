# KServerTools Example Project

This repository contains an example set of code to demonstrate how to use the KServerTools NuGet package.

## Setup Instructions

Follow these steps to set up the project:

1. **Clone the repository:**
    ```sh
    git clone https://github.com/yourusername/dotnet-KServerTools-example.git
    cd dotnet-KServerTools-example/src/server
    ```

2. **Install dependencies:**
    ```sh
    dotnet restore
    ```

3. **Build the project:**
    ```sh
    dotnet build
    ```

4. **Run the project:**
    ```sh
    dotnet run
    ```

5. **Add KServerTools NuGet package:**
    ```sh
    dotnet add package KServerTools
    ```

## Detailed Setup Instructions

1. **Download and install .NET SDK:**
    Ensure you have the .NET SDK installed on your machine. You can download it from the official [.NET website](https://dotnet.microsoft.com/download).

2. **Database setup:**
    If your project requires a database, set up the database and update the connection strings in your configuration files.

3. **Install Docker:**
    Download and install Docker from the official [Docker website](https://www.docker.com/products/docker-desktop).

4. **Run MSSQL Docker container:**
    Note password should be changed in two places: here and appsettings.json
    ```sh
    docker pull mcr.microsoft.com/mssql/server:2022-latest
    docker run  --name mysqlinstance -v sql_data:/var/opt/mssql -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=somePassword1" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
    ```

5. **Run setup.sql script:**
    Ensure you're in the same direcctory as `setup.sql` and run the following command.
    Note: If you update the image to a new/older version, sqlcmd path might need to be updated.
    ```sh
    docker cp setup.sql mysqlinstance:/tmp/setup.sql
    docker exec -i mysqlinstance /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P 'somePassword1' -d master -C -i /tmp/setup.sql
    ```

## License

This project is licensed under the MIT License. // TODO: See the `LICENSE` file for more details.
