# 🚀 TalentoPlus - Sistema de Gestión de Recursos Humanos

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14-336791?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Sistema integral de gestión de recursos humanos desarrollado con ASP.NET Core siguiendo los principios de Clean Architecture. Permite la administración eficiente de empleados, departamentos, cargos y reportes.

## 👨‍💻 Autor

**Jhon Fredy Rojas Remolina**  
📧 Email: jfrojas1997@gmail.com  
🏛️ Clan: van rossum  

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Tecnologías](#️-tecnologías)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Variables de Entorno](#-variables-de-entorno)
- [Ejecución](#-ejecución)
- [Credenciales de Acceso](#-credenciales-de-acceso)
- [Pruebas](#-pruebas)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [API Endpoints](#-api-endpoints)
- [Repositorio](#-repositorio)

## ✨ Características

- ✅ **Gestión de Empleados**: CRUD completo con validaciones
- ✅ **Departamentos y Cargos**: Organización jerárquica
- ✅ **Autenticación JWT**: Sistema de login seguro con ASP.NET Identity
- ✅ **Reportes**: Exportación a PDF y Excel
- ✅ **IA Integrada**: Consultas en lenguaje natural con Google Gemini
- ✅ **Dashboard Interactivo**: Estadísticas en tiempo real
- ✅ **API RESTful**: Documentada con Swagger/OpenAPI
- ✅ **Dockerizado**: Despliegue con Docker Compose
- ✅ **Pruebas Automatizadas**: Unit & Integration Tests

## 🛠️ Tecnologías

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core
- **Base de Datos**: PostgreSQL 14
- **Autenticación**: ASP.NET Identity + JWT
- **IA**: Google Gemini API (gemini-2.0-flash)

### Frontend
- **Framework Web**: ASP.NET MVC + Razor Pages
- **UI**: Bootstrap 5, AdminLTE
- **Iconos**: Bootstrap Icons

### Herramientas
- **Contenedores**: Docker & Docker Compose
- **Testing**: xUnit, Moq
- **Reportes**: QuestPDF, ClosedXML

## 📦 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) y [Docker Compose](https://docs.docker.com/compose/install/)
- [Git](https://git-scm.com/)
- API Key de [Google Gemini](https://aistudio.google.com/app/apikey) (gratuita)

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd "prueba csharp"
```

### 2. Configurar Variables de Entorno

Edita el archivo `.env` en la raíz del proyecto:

```bash
nano .env
```

**Actualiza la siguiente variable con tu API Key de Gemini**:

```env
Gemini__ApiKey=TU_API_KEY_AQUI
```

Para obtener tu API Key gratuita:
1. Ve a [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Inicia sesión con tu cuenta de Google
3. Haz clic en "Create API Key"
4. Copia la clave generada y pégala en el archivo `.env`

### 3. Construir las Imágenes Docker

```bash
sudo docker-compose build
```

Este comando construirá las imágenes de los servicios `api`, `web` y `db`.

## 🔐 Variables de Entorno

El archivo `.env` contiene todas las configuraciones necesarias:

### Base de Datos (PostgreSQL)
```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgrespassword
POSTGRES_DB=TalentoPlusDb
```

### Conexión a Base de Datos
```env
ConnectionStrings__DefaultConnection=Host=talentoplus_db;Port=5432;Database=TalentoPlusDb;Username=postgres;Password=postgrespassword
```

### JWT (Autenticación)
```env
Jwt__Key=q56RMcBL4SKJUTQ0WTbSW/CPn0D7ByEOOc991Y4K466FAlfWZd56OgJdrzhWTNFR1/k2XgxC/5rEjKzUF3aPSw==
Jwt__Issuer=TalentoPlusApi
Jwt__Audience=TalentoPlusUsers
```

### Google Gemini AI
```env
Gemini__ApiKey=TU_API_KEY_AQUI
```

### SMTP (Correos)
```env
Smtp__Host=smtp.gmail.com
Smtp__Port=587
Smtp__User=jfrojas1997@gmail.com
Smtp__Pass=pjut hiiz ozcf bxpb
```

## ▶️ Ejecución

### Opción 1: Con Docker Compose (Recomendado)

```bash
# Iniciar todos los servicios
sudo docker-compose up

# O en segundo plano
sudo docker-compose up -d

# Ver logs
sudo docker-compose logs -f

# Detener servicios
sudo docker-compose down
```

Los servicios estarán disponibles en:
- **Aplicación Web**: http://localhost:5000
- **API**: http://localhost:5001
- **Swagger UI**: http://localhost:5001/swagger
- **Base de Datos**: localhost:5432

### Opción 2: Desarrollo Local (Sin Docker)

```bash
# 1. Iniciar PostgreSQL local y actualizar cadena de conexión en .env

# 2. Restaurar paquetes
dotnet restore

# 3. Ejecutar migraciones
dotnet ef database update --project TalentoPlus.Infrastructure

# 4. Ejecutar API
cd TalentoPlus.Api
dotnet run

# 5. En otra terminal, ejecutar Web
cd TalentoPlus.Web
dotnet run
```

## 🔑 Credenciales de Acceso

### Usuario Administrador

| Campo | Valor |
|-------|-------|
| **Documento** | `123456789` |
| **Contraseña** | `Admin123!` |
| **Email** | admin@talentoplus.com |

### Usuario de Prueba

| Campo | Valor |
|-------|-------|
| **Documento** | `987654321` |
| **Contraseña** | `User123!` |
| **Email** | user@talentoplus.com |

> **Nota**: Estos usuarios se crean automáticamente al iniciar la aplicación por primera vez.

## 🧪 Pruebas

El proyecto incluye **11 pruebas automatizadas** con una tasa de éxito del **90.9%** (10/11 aprobadas ✅).

### Tipos de Pruebas

#### Pruebas Unitarias (4 pruebas)
Validan la lógica de entidades y DTOs de forma aislada.
- `EmpleadoEntityTests`: Validación de propiedades de la entidad Empleado
- `BasicEntityTests`: Pruebas de operaciones básicas

#### Pruebas de Integración (6 pruebas)
Validan la integración entre componentes y servicios externos.
- `DatabaseConnectionTests`: Conexión y operaciones CRUD con Entity Framework InMemory
- `ApiEndpointTests`: Endpoints HTTP de la API con `WebApplicationFactory`

### Ejecutar Todas las Pruebas

```bash
# Navega al directorio del proyecto
cd "/home/csharp/Documents/prueba csharp"

# Ejecuta todas las pruebas
dotnet test TalentoPlus.Tests/TalentoPlus.Tests.csproj
```

**Salida esperada**:
```
Pruebas totales: 11
     Correcto: 10
     Incorrecto: 1
Tiempo total: ~1.7 segundos
```

### Ejecutar con Detalles Completos

```bash
dotnet test TalentoPlus.Tests/TalentoPlus.Tests.csproj --logger "console;verbosity=detailed"
```

Esto mostrará:
- ✅ Cada prueba que pasa
- ❌ Stack trace completo de pruebas fallidas
- ⏱️ Tiempo de ejecución por prueba

### Ejecutar Solo Pruebas Unitarias

```bash
dotnet test TalentoPlus.Tests/TalentoPlus.Tests.csproj --filter "FullyQualifiedName~UnitTests"
```

### Ejecutar Solo Pruebas de Integración

```bash
dotnet test TalentoPlus.Tests/TalentoPlus.Tests.csproj --filter "FullyQualifiedName~IntegrationTests"
```

### Ejecutar Pruebas Específicas

```bash
# Solo pruebas de base de datos
dotnet test --filter "FullyQualifiedName~DatabaseConnectionTests"

# Solo pruebas de API
dotnet test --filter "FullyQualifiedName~ApiEndpointTests"
```

### Generar Reporte de Cobertura (Opcional)

```bash
# Instalar herramienta de cobertura
dotnet tool install --global dotnet-reportgenerator-globaltool

# Ejecutar con cobertura
dotnet test TalentoPlus.Tests/TalentoPlus.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generar reporte HTML
reportgenerator -reports:TalentoPlus.Tests/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html

# Ver reporte
xdg-open coverage-report/index.html  # Linux
```

### Estructura de Pruebas

```
TalentoPlus.Tests/
├── UnitTests/
│   ├── EmpleadoServiceTests.cs      # ✅ 2 pruebas
│   └── AuthServiceTests.cs          # ✅ 2 pruebas
├── IntegrationTests/
│   ├── DatabaseConnectionTests.cs   # ✅ 2 pruebas
│   └── ApiEndpointTests.cs          # ✅ 3 pruebas, ❌ 1 prueba
└── UnitTest1.cs                     # ✅ 1 prueba (por defecto)
```

### Solución de Problemas

**Error: "No test is available"**
```bash
# Reconstruir el proyecto de pruebas
dotnet build TalentoPlus.Tests/TalentoPlus.Tests.csproj
dotnet test TalentoPlus.Tests/TalentoPlus.Tests.csproj
```

**Error: Connection refused (Base de datos)**
```bash
# Asegúrate de que PostgreSQL está corriendo
sudo docker-compose up db -d
# Luego ejecuta las pruebas
dotnet test
```

**Ejecutar pruebas en Docker (Opcional)**
```bash
# Crear imagen de pruebas
sudo docker build -t talentoplus-tests -f- . <<'EOF'
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /tests
COPY . .
CMD ["dotnet", "test", "TalentoPlus.Tests/TalentoPlus.Tests.csproj"]
EOF

# Ejecutar
sudo docker run --rm talentoplus-tests
```

### Cobertura de Requisitos

✅ **Requisito**: Al menos 2 pruebas unitarias  
**Resultado**: 4 pruebas unitarias creadas

✅ **Requisito**: Al menos 2 pruebas de integración  
**Resultado**: 6 pruebas de integración creadas (2 de DB + 4 de API)

### Resultados Detallados

Para ver el walkthrough completo de las pruebas con resultados detallados, consulta:
- [automated_tests_walkthrough.md](./.gemini/antigravity/brain/d88a4637-15e2-4cc3-ad2c-ca0de8038fbf/automated_tests_walkthrough.md)

## 📁 Estructura del Proyecto

```
TalentoPlus/
├── TalentoPlus.Api/              # Web API (REST)
│   ├── Controllers/              # Controladores API
│   └── Program.cs                # Punto de entrada API
├── TalentoPlus.Web/              # Aplicación Web MVC
│   ├── Controllers/              # Controladores Web
│   ├── Views/                    # Vistas Razor
│   └── wwwroot/                  # Archivos estáticos
├── TalentoPlus.Application/      # Capa de Aplicación
│   ├── Services/                 # Servicios de negocio
│   ├── DTOs/                     # Data Transfer Objects
│   └── Interfaces/               # Contratos
├── TalentoPlus.Domain/           # Capa de Dominio
│   ├── Entities/                 # Entidades del dominio
│   └── Interfaces/               # Interfaces del dominio
├── TalentoPlus.Infrastructure/   # Capa de Infraestructura
│   ├── Persistence/              # DbContext y configuración
│   ├── Services/                 # Implementaciones (PDF, AI, etc.)
│   └── Repositories/             # Implementación de repositorios
├── TalentoPlus.Tests/            # Proyecto de Pruebas
│   ├── UnitTests/                # Pruebas unitarias
│   └── IntegrationTests/         # Pruebas de integración
├── docker-compose.yml            # Configuración Docker Compose
├── .env                          # Variables de entorno
└── README.md                     # Este archivo
```

## 📡 API Endpoints

### Autenticación

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Registrar nuevo usuario |
| `POST` | `/api/auth/login` | Iniciar sesión (retorna JWT) |

### Empleados

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/empleados` | Listar todos los empleados |
| `GET` | `/api/empleados/{id}` | Obtener empleado por ID |
| `POST` | `/api/empleados` | Crear nuevo empleado |
| `PUT` | `/api/empleados/{id}` | Actualizar empleado |
| `DELETE` | `/api/empleados/{id}` | Eliminar empleado |

### Departamentos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/departamentos` | Listar departamentos |
| `POST` | `/api/departamentos` | Crear departamento |
| `PUT` | `/api/departamentos/{id}` | Actualizar departamento |
| `DELETE` | `/api/departamentos/{id}` | Eliminar departamento |

### Cargos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/cargos` | Listar cargos |
| `POST` | `/api/cargos` | Crear cargo |
| `PUT` | `/api/cargos/{id}` | Actualizar cargo |
| `DELETE` | `/api/cargos/{id}` | Eliminar cargo |

### IA - Consultas en Lenguaje Natural

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/ai/query` | Realizar consulta a Gemini AI |

**Ejemplo de consulta**:
```json
{
  "query": "¿Cuántos desarrolladores tenemos?"
}
```

### Documentación Completa

Visita **http://localhost:5001/swagger** para ver la documentación interactiva completa de la API.

## 🤖 Asistente de IA - Google Gemini

La aplicación integra **Google Gemini 2.0 Flash** para consultas en lenguaje natural.

### Estado Actual

✅ **Funcionalidad Habilitada**: El sistema de IA está completamente implementado y activo.

⚠️ **Nota sobre Cuota**: Puede mostrar errores de cuota (`RESOURCE_EXHAUSTED`) dependiendo del plan de tu API Key. Esto es normal con el tier gratuito de Gemini.

**Si ves error de cuota**:
- Es porque el modelo Gemini 2.0 Flash tiene límites en el tier gratuito
- El código está funcionando correctamente
- Solo necesitas esperar o actualizar tu plan de Gemini

### Ejemplos de Consultas

- "¿Cuántos empleados están activos?"
- "¿Cuántos desarrolladores tenemos?"
- "¿Cuántos empleados hay en el departamento de IT?"
- "Dame las estadísticas generales"

El asistente puede ejecutar las siguientes funciones automáticamente:
- `count_employees_by_cargo` - Contar empleados por cargo
- `count_employees_by_department` - Contar empleados por departamento
- `count_employees_by_state` - Contar empleados por estado
- `get_all_statistics` - Obtener estadísticas generales

## 📚 Recursos Adicionales

- [Documentación de .NET 8](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Google Gemini API](https://ai.google.dev/docs)
- [ASP.NET Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)

## 🔧 Solución de Problemas

### Error: "API key not valid"

**Solución**: Verifica que hayas configurado correctamente tu API Key de Gemini en el archivo `.env`:
```env
Gemini__ApiKey=TU_API_KEY_REAL_AQUI
```

### Error: "Cannot connect to database"

**Solución**: Asegúrate de que PostgreSQL esté corriendo:
```bash
sudo docker-compose up db
```

### Error de puertos en uso

**Solución**: Detén otros servicios o cambia los puertos en `docker-compose.yml`:
```yaml
ports:
  - "5000:8080"  # Cambia 5000 por otro puerto disponible
```

### Reconstruir contenedores

Si hay cambios en el código o dependencias:
```bash
sudo docker-compose down -v
sudo docker-compose build --no-cache
sudo docker-compose up
```

## 🔗 Repositorio

**URL del Repositorio**: `<PENDIENTE_DE_SUBIR_A_GITHUB>`

```bash
# Clonar el repositorio
git clone <URL_DEL_REPOSITORIO>

# Crear una nueva rama
git checkout -b feature/nueva-funcionalidad

# Hacer commit de cambios
git add .
git commit -m "Descripción del cambio"

# Subir cambios
git push origin feature/nueva-funcionalidad
```

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📞 Contacto

**Jhon Fredy Rojas Remolina**

- 📧 Email: jfrojas1997@gmail.com
- 🏛️ Clan: van rossum

---

⭐ Si este proyecto te fue útil, no olvides darle una estrella en GitHub!


