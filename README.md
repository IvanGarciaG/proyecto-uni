# Microservices Solution

Arquitectura de microservicios construida con **.NET 8 / C#**, diseñada para gestión de usuarios, órdenes, archivos, geolocalización, autenticación con Google y notificaciones por email.

---

## Tecnologías

### Backend
| Tecnología | Uso |
|---|---|
| **.NET 8 / ASP.NET Core** | Framework base de todos los microservicios |
| **YARP** | API Gateway — routing, rate limiting, SSL termination |
| **MediatR** | Patrón CQRS — Commands y Queries |
| **MassTransit + RabbitMQ** | Bus de mensajes — eventos asíncronos entre servicios |
| **Entity Framework Core** | ORM para PostgreSQL y SQL Server |
| **MongoDB Driver** | Base de datos de documentos (Products Service) |
| **StackExchange.Redis** | Caché distribuido |
| **MailKit + Scriban** | Envío de emails con plantillas HTML |
| **Google.Apis.Auth** | Validación de tokens de Google OAuth2 |
| **Serilog + Seq** | Logging estructurado centralizado |
| **OpenTelemetry** | Trazabilidad distribuida |

### Infraestructura
| Tecnología | Uso |
|---|---|
| **Docker + Docker Compose** | Contenedores de todos los servicios e infraestructura |
| **PostgreSQL 16** | BD de Users, Identity, Location, Files |
| **SQL Server 2022** | BD de Orders |
| **MongoDB 7** | BD de Products |
| **Redis 7** | Caché |
| **RabbitMQ 3.13** | Message broker |
| **Seq** | Visor de logs centralizado |

---

## Servicios

| Servicio | Puerto | Descripción |
|---|---|---|
| **API Gateway** | `5000` | Punto de entrada único — YARP + JWT |
| **Identity Service** | `5001` | Google OAuth2 + emisión de JWT propio |
| **Users Service** | interno | Registro y gestión de usuarios |
| **Orders Service** | interno | Gestión de órdenes |
| **Files Service** | interno | Subida de PNG, JPG y PDF |
| **Location Service** | interno | GPS — guardar posición y búsqueda por radio |
| **Geocoding Service** | interno | Google Maps — dirección ↔ coordenadas |
| **Notifications Service** | interno | Emails de alertas con Mailtrap |
| **RabbitMQ UI** | `15672` | Panel de administración del broker |
| **Seq (Logs)** | `5341` | Visor de logs en tiempo real |

---

## Requisitos previos

Antes de clonar, asegúrate de tener instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) con el workload **"Desarrollo de ASP.NET y web"**
- [Git](https://git-scm.com/)

---

## Clonación del proyecto

```bash
git clone https://github.com/IvanGarciaG/proyecto-uni.git
cd proyecto-uni
git checkout development
```

---

## Configuración de variables de entorno

Copia el archivo de ejemplo y completa tus credenciales:

```bash
cp .env.example .env
```

Edita el archivo `.env` con tus valores reales:

```env
# Google OAuth2
# Obtener en: https://console.cloud.google.com → APIs y servicios → Credenciales
GOOGLE_CLIENT_ID=tu-client-id.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=tu-client-secret

# Google Maps API
# Obtener en: https://console.cloud.google.com → Maps JavaScript API
GOOGLE_MAPS_API_KEY=tu-google-maps-api-key

# JWT Secret — genera una clave aleatoria con:
# openssl rand -base64 64
JWT_SECRET=clave_aleatoria_de_al_menos_64_caracteres

# Mailtrap (para emails de desarrollo)
# Obtener en: https://mailtrap.io → Email Testing → Inboxes → SMTP Settings
MAILTRAP_USER=tu-mailtrap-user
MAILTRAP_PASSWORD=tu-mailtrap-password
```

> **Importante:** El archivo `.env` está en `.gitignore` y nunca se sube al repositorio.

---

## Arranque del proyecto

### Paso 1 — Levantar la infraestructura y servicios

```bash
docker compose --env-file .env up -d
```

Esto levanta automáticamente:
- PostgreSQL, SQL Server, MongoDB, Redis
- RabbitMQ
- Seq (logs)
- API Gateway
- Todos los microservicios

### Paso 2 — Verificar que los servicios están corriendo

```bash
docker compose ps
```

Todos los servicios deben aparecer con estado `Up`.

### Paso 3 — Acceder a los servicios

| Servicio | URL |
|---|---|
| **API Gateway** | http://localhost:5000 |
| **Swagger — Identity** | http://localhost:5001/swagger |
| **RabbitMQ UI** | http://localhost:15672 (admin / admin123) |
| **Seq — Logs** | http://localhost:5341 |

---

## Abrir en Visual Studio

### Paso 1 — Crear la solución (solo la primera vez)

Abre una terminal en la carpeta del proyecto y ejecuta:

```powershell
dotnet new sln --name MicroservicesSolution

dotnet sln add src/Shared/Shared.Kernel/Shared.Kernel.csproj
dotnet sln add src/Shared/Shared.Contracts/Shared.Contracts.csproj
dotnet sln add src/ApiGateway/ApiGateway.API/ApiGateway.API.csproj
dotnet sln add src/Services/Users/Users.Domain/Users.Domain.csproj
dotnet sln add src/Services/Users/Users.Application/Users.Application.csproj
dotnet sln add src/Services/Users/Users.Infrastructure/Users.Infrastructure.csproj
dotnet sln add src/Services/Users/Users.API/Users.API.csproj
dotnet sln add src/Services/Orders/Orders.Domain/Orders.Domain.csproj
dotnet sln add src/Services/Orders/Orders.Application/Orders.Application.csproj
dotnet sln add src/Services/Files/Files.Domain/Files.Domain.csproj
dotnet sln add src/Services/Files/Files.Application/Files.Application.csproj
dotnet sln add src/Services/Files/Files.Infrastructure/Files.Infrastructure.csproj
dotnet sln add src/Services/Files/Files.API/Files.API.csproj
dotnet sln add src/Services/Identity/Identity.API/Identity.API.csproj
dotnet sln add src/Services/Location/Location.API/Location.API.csproj
dotnet sln add src/Services/Geocoding/Geocoding.API/Geocoding.API.csproj
dotnet sln add src/Services/Notifications/Notifications.Application/Notifications.Application.csproj
dotnet sln add src/Services/Notifications/Notifications.Infrastructure/Notifications.Infrastructure.csproj
dotnet sln add src/Services/Notifications/Notifications.API/Notifications.API.csproj
```

### Paso 2 — Abrir la solución

```powershell
start MicroservicesSolution.sln
```

O desde Visual Studio: **Archivo → Abrir → Proyecto o solución** → selecciona `MicroservicesSolution.sln`

### Paso 3 — Restaurar paquetes NuGet

Visual Studio lo hace automáticamente. Si no, click derecho en la solución → **Restaurar paquetes NuGet**.

---

## Flujo de autenticación con Google

```
1. El cliente obtiene un idToken desde Google Sign-In SDK
2. POST http://localhost:5000/api/auth/google
   Body: { "idToken": "..." }
3. El sistema valida el token con Google
4. Retorna un JWT propio + refresh token
5. Usar el JWT en el header: Authorization: Bearer <token>
```

---

## Subida de archivos

```
POST http://localhost:5000/api/files/upload
Authorization: Bearer <token>
Content-Type: multipart/form-data

Formatos aceptados: PNG, JPG, PDF
Tamaño máximo: 10 MB
```

---

## Detener los servicios

```bash
docker compose down
```

Para detener y eliminar los volúmenes (borra los datos):

```bash
docker compose down -v
```

---

## Estructura del proyecto

```
proyecto-uni/
├── src/
│   ├── ApiGateway/
│   │   └── ApiGateway.API          # YARP Gateway
│   ├── Shared/
│   │   ├── Shared.Kernel           # Entity, Result<T>, IDomainEvent
│   │   └── Shared.Contracts        # Eventos de integración
│   └── Services/
│       ├── Users/                  # Registro y gestión de usuarios
│       ├── Orders/                 # Gestión de órdenes
│       ├── Files/                  # Subida de archivos
│       ├── Identity/               # Google OAuth2 + JWT
│       ├── Location/               # GPS + búsqueda por radio
│       ├── Geocoding/              # Google Maps API
│       └── Notifications/          # Emails con Mailtrap
├── docker-compose.yml
├── .env.example
└── .gitignore
```

---

## Contribución

1. Crea una rama desde `development`: `git checkout -b feature/nombre-feature`
2. Realiza tus cambios y haz commit
3. Abre un Pull Request hacia `development`
