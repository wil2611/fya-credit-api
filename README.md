# FYA Credit API

Backend desarrollado en ASP.NET Core (.NET 8) para la prueba técnica de gestión y registro de créditos.

La API permite registrar créditos, consultarlos con filtros y ordenamiento, y procesar notificaciones por correo de manera asíncrona mediante trabajos en segundo plano con Hangfire y SendGrid.

## Tecnologías

- **.NET 8** (C#)
- **ASP.NET Core Web API**
- **Entity Framework Core** + **Npgsql**
- **PostgreSQL 16**
- **Hangfire** (Background Jobs)
- **SendGrid API** (Envío de correos)
- **Swagger / OpenAPI**
- **Docker**
- **Railway** (Hosting de la API)
- **Supabase** (PostgreSQL en la nube)

## Arquitectura y Flujo

### 1. Flujo general

```text
Ionic React / Android (Capacitor)
       ↓ HTTPS
ASP.NET Core API
       ↓
Entity Framework Core
       ↓
PostgreSQL (Supabase / Local)
```

### 2. Envío de correos desacoplado

Para no bloquear la respuesta HTTP al cliente, el envío de correos se procesa en segundo plano:

```text
POST /api/credits ──> Guarda crédito en BD ──> Responde 201 Created
                            │
                            └──> Encola trabajo en Hangfire
                                        │
                                        └──> SendGrid API ──> Notificación por correo
```

## Requisitos

- .NET SDK 8.0+
- Docker (opcional, para BD local)
- Herramienta de migraciones de EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Base de datos

### Opción 1: PostgreSQL local con Docker

Puedes levantar una instancia local en el puerto 5432:

```bash
docker run --name fya-postgres \
  -e POSTGRES_USER=fya_user \
  -e POSTGRES_PASSWORD=fya_password \
  -e POSTGRES_DB=fya_credit_db \
  -p 5432:5432 \
  -d postgres:16
```

*(En PowerShell es el mismo comando en una sola línea).*

La cadena de conexión para desarrollo local ya viene configurada por defecto en `appsettings.Development.json`.

### Opción 2: Base de datos en producción (Supabase)

En producción la aplicación se conecta a una instancia de PostgreSQL en Supabase mediante la variable de entorno `ConnectionStrings__DefaultConnection`.

### Migraciones

Para aplicar las migraciones existentes a la base de datos:

```bash
dotnet ef database update
```

Si realizas cambios en las entidades y necesitas generar una nueva migración:

```bash
dotnet ef migrations add NombreDeLaMigracion
```

## Ejecución local

1. Restaurar dependencias:

   ```bash
   dotnet restore
   ```
2. Aplicar migraciones:

   ```bash
   dotnet ef database update
   ```
3. Iniciar la API:

   ```bash
   dotnet run
   ```

Swagger estará disponible en:

```text
http://localhost:5136/swagger
```

El dashboard de Hangfire (solo habilitado en desarrollo) se encuentra en:

```text
http://localhost:5136/hangfire
```

## API en Producción

- **Base URL:** `https://fya-credit-api-production.up.railway.app`
- **Swagger UI:** `https://fya-credit-api-production.up.railway.app/swagger`

*(Por seguridad, el dashboard de Hangfire no está expuesto en producción).*

## Endpoints

### 1. Crear crédito

`POST /api/credits`

**Body (JSON):**

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

**Respuesta:** `201 Created`

### 2. Consultar créditos

`GET /api/credits`

**Parámetros opcionales:**

- `clientName`: Filtrar por nombre del cliente (coincidencia parcial).
- `clientDocument`: Filtrar por documento.
- `salesperson`: Filtrar por comercial.
- `sortBy`: Campo de ordenamiento (`amount` o `createdAt`, por defecto `createdAt`).
- `sortOrder`: Dirección (`asc` o `desc`, por defecto `desc`).

**Ejemplo:**

```http
GET /api/credits?salesperson=carlos&sortBy=amount&sortOrder=desc
```

## Configuración de SendGrid

Para que el envío de correos funcione localmente sin guardar credenciales en el repositorio, utiliza User Secrets de .NET:

```powershell
dotnet user-secrets set "SendGrid:ApiKey" "TU_SENDGRID_API_KEY"
dotnet user-secrets set "SendGrid:FromEmail" "tu-correo-verificado@dominio.com"
dotnet user-secrets set "SendGrid:FromName" "FYA Credit App"
```

El correo de notificación se envía automáticamente a:

`fyasocialcapital@gmail.com`

El correo incluye:

- Cliente
- Valor del crédito
- Comercial
- Fecha de registro

## Variables de Entorno (Producción / Railway)

- `ASPNETCORE_ENVIRONMENT`: `Production`
- `ASPNETCORE_URLS`: `http://+:${PORT}`
- `ConnectionStrings__DefaultConnection`: Cadena de conexión a Supabase.
- `SendGrid__ApiKey`: API Key de SendGrid.
- `SendGrid__FromEmail`: Remitente validado en SendGrid.
- `SendGrid__FromName`: Nombre que aparece en el remitente.

## Validaciones y Manejo de Errores

- **Validaciones:** Se validan campos obligatorios, longitudes máximas, formato de nombres y documento, monto mayor a cero, tasa entre 0 y 100 y plazo entre 1 y 600 meses.
- **Manejo de excepciones:** Se utiliza `ProblemDetails` para devolver respuestas de error estandarizadas. Las excepciones no controladas generan un código 500 con un `traceId` para depuración sin revelar detalles internos al cliente.
- **Rate Limiting:** Se configuró un límite de 60 peticiones por minuto por IP. Si se excede, retorna `429 Too Many Requests`.
- **CORS:** Habilitado para permitir peticiones desde el cliente web local, la aplicación móvil (Capacitor/Android) y el frontend desplegado en Vercel.

## Docker

Para construir y probar la imagen localmente:

```bash
docker build -t fya-credit-api .
```

## Estructura del Proyecto

```text
Configuration/   # Configuración de SendGrid
Controllers/     # Endpoints de la API
Data/            # DbContext y configuración de EF Core
DTOs/            # Modelos de entrada y salida
Entities/        # Entidades de base de datos
Migrations/      # Migraciones de Entity Framework Core
Services/        # Servicios de envío de correo
database.sql     # Script de creación de la base de datos
Dockerfile       # Configuración Docker
Program.cs       # Configuración principal de la aplicación
```

## Estado del proyecto

- [x] Registro de créditos.
- [x] Consulta con filtros y ordenamiento.
- [x] Validaciones de datos.
- [x] PostgreSQL local y Supabase.
- [x] Script `database.sql`.
- [x] Envío de correos con SendGrid.
- [x] Procesamiento en segundo plano con Hangfire.
- [x] Swagger / OpenAPI.
- [x] Manejo de errores y Rate Limiting.
- [x] Docker.
- [x] Despliegue en Railway.
