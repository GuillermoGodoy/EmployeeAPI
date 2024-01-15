# EmployeeAPI

API con CRUD de Empleados y Login, requiere autenticación para poder acceder a los endpoints.
El usuario y contraseña para acceder a la API es: admin/admin
{
    "username": "admin",
    "password": "admin"
}

## Requisitos

- [Dotnet SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) 

## Configuración del Proyecto

1. Clona este repositorio
2. Instalar el SDK de Dotnet 8
3. Instalar MongoDB
4. Tener una instancia de MongoDB corriendo en el puerto 27017 (mongodb://localhost:27017/)
    a. En caso de no tener una instancia de MongoDB corriendo en el puerto 27017, puedes cambiar la cadena de conexión en el archivo appsettings.json
5. En la carpeta raíz del proyecto ejecuta el comando `dotnet restore`
6. En la carpeta raíz del proyecto ejecuta el comando `dotnet run`

## Documentación de la API
Se puede visualizar la documentación al ejecutar el proyecto en la siguiente url:
<url>/swagger/index.html