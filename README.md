# FYA Credit API

Backend desarrollado para la prueba técnica de registro y consulta de créditos.

## Tecnologías

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 16
- Docker
- Swagger / OpenAPI
- Hangfire
- MailKit

## Requisitos

Para ejecutar el proyecto se necesita:

- .NET SDK 8
- Docker
- Git

## Base de datos

El proyecto utiliza PostgreSQL. Para crear la base de datos local con Docker:

```bash
docker run --name fya-postgres \
  -e POSTGRES_USER=fya_user \
  -e POSTGRES_PASSWORD=fya_password \
  -e POSTGRES_DB=fya_credit_db \
  -p 5432:5432 \
  -d postgres:16
```

En PowerShell también puede ejecutarse en una sola línea:

```powershell
docker run --name fya-postgres -e POSTGRES_USER=fya_user -e POSTGRES_PASSWORD=fya_password -e POSTGRES_DB=fya_credit_db -p 5432:5432 -d postgres:16
```

## Configuración

La conexión de desarrollo se encuentra en `appsettings.Development.json` y utiliza PostgreSQL en `localhost:5432`, la base de datos `fya_credit_db` y el usuario `fya_user`.

Las credenciales configuradas son únicamente para el entorno local de desarrollo.

## Instalación

Restaurar las dependencias, aplicar las migraciones y ejecutar la aplicación:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

## Swagger

Con la aplicación ejecutándose, Swagger puede consultarse en:

```text
http://localhost:5136/swagger
```

El puerto puede variar dependiendo de la configuración local.

## Endpoints implementados

### Registrar crédito

```http
POST /api/credits
```

Ejemplo:

```json
{
  "clientName": "Pepito Perez",
  "clientDocument": "123456789",
  "amount": 7800000,
  "interestRate": 2,
  "termMonths": 10,
  "salesperson": "Carlos Rodriguez"
}
```

Una creación correcta devuelve `201 Created`.

### Consultar créditos

```http
GET /api/credits
```

Filtros disponibles:

- `clientName`
- `clientDocument`
- `salesperson`

Opciones de ordenamiento:

- `sortBy=amount`
- `sortBy=createdAt`
- `sortOrder=asc`
- `sortOrder=desc`

Ejemplo:

```http
GET /api/credits?salesperson=carlos&sortBy=amount&sortOrder=desc
```

## Notificaciones por correo

Cuando se registra un nuevo crédito, la API crea un trabajo en segundo plano utilizando Hangfire.

El registro del crédito no espera a que termine el envío del correo. Hangfire procesa posteriormente el trabajo y MailKit realiza el envío mediante SMTP.

El correo incluye:

- Nombre del cliente.
- Valor del crédito.
- Comercial que registró el crédito.
- Fecha de registro.

Las credenciales SMTP no se almacenan en el repositorio. Durante el desarrollo se utilizan .NET User Secrets.

### Configuración local del correo

```powershell
dotnet user-secrets set "Email:Username" "correo@gmail.com"
dotnet user-secrets set "Email:FromEmail" "correo@gmail.com"
dotnet user-secrets set "Email:Password" "APP_PASSWORD"
```

El dashboard de Hangfire está disponible durante desarrollo en:

```text
http://localhost:5136/hangfire
```

El puerto puede variar dependiendo de la configuración local.

## Validaciones

Actualmente se realizan validaciones en el backend para:

- Nombre del cliente requerido, con máximo 120 caracteres.
- Documento requerido, con máximo 30 caracteres.
- Valor del crédito mayor a cero.
- Tasa de interés entre 0 y 100.
- Plazo entre 1 y 600 meses.
- Comercial requerido, con máximo 120 caracteres.

## Estructura actual

```text
Controllers/
Data/
DTOs/
Entities/
Migrations/
Properties/
Program.cs
```

- `Controllers`: endpoints HTTP de la aplicación.
- `Data`: configuración de Entity Framework y `AppDbContext`.
- `DTOs`: objetos para recibir y enviar información a través de la API.
- `Entities`: modelos persistidos en la base de datos.
- `Migrations`: migraciones generadas por Entity Framework Core.

## Seguridad y manejo de errores

La API incluye manejo global de excepciones utilizando `ProblemDetails`.

Las respuestas producidas por errores inesperados utilizan un formato consistente e incluyen un `traceId` que puede utilizarse para relacionar la respuesta con los logs de la aplicación.

También se implementa rate limiting por dirección IP.

Actualmente cada cliente puede realizar un máximo de:

```text
60 solicitudes por minuto
```

Cuando se supera este límite, la API responde con:

```text
429 Too Many Requests
```

Las validaciones de los datos recibidos se realizan en el backend antes de procesar las solicitudes.

## Próximos pasos

- Implementar autenticación JWT si el tiempo de desarrollo lo permite.

## Estado del proyecto

Proyecto actualmente en desarrollo como parte de una prueba técnica.
