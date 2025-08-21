# BTG Fondos API

Plataforma de prueba técnica que permite a los clientes gestionar fondos de inversión sin necesidad de un asesor, construida en .NET 9 / Minimal APIs con una arquitectura limpia y desacoplada.

>  Esta entrega cubre **Parte 1 – Fondos (80%)** del reto.
--
## Funcionalidades Implementadas

Suscribirse a un fondo (apertura).

Cancelar suscripción a un fondo activo.

Historial de transacciones (aperturas y cancelaciones).

Notificaciones por email/SMS (mockeadas en consola según preferencia del cliente).

Autenticación y autorización con JWT + roles (admin, cliente).

Persistencia en MongoDB con modelo NoSQL.

Pruebas unitarias con xUnit + Moq.
---

##  Prerrequisitos

- **.NET SDK 9** (o 8 si tu entorno aún no soporta 9)
- **MongoDB** local o en contenedor
- Opcional: **MongoDB Compass** para inspeccionar datos

### MongoDB
Crea una base y usuario de prueba (ejemplo):
- DB: `btg`
- Usuario: `test`
- Password: `test12345`

Conexión usada (ajústala en `appsettings.json`):
```
mongodb://test:test12345@localhost:27017/btg
```

Índices recomendados:
- `Clientes`: índice sobre `Id` (Guid)
- `Fondos`: índice sobre `_id` (string “1..5”)
- `Transacciones`: índice sobre `clienteId` (Guid)

---

##  Configuración (appsettings.json)

```json
{
  "Mongo": {
    "ConnectionString": "mongodb://test:test12345@localhost:27017/btg",
    "Database": "btg"
  },
  "Jwt": {
    "Key": "el-super-password-top-secret-jwt1234567890",
    "Issuer": "BTG.Api",
    "Audience": "BTG.Client",
    "AccessMinutes": 15,
    "RefreshDays": 7
  },
  "Swagger": {
    "Title": "BTG API",
    "Version": "v1"
  },
  "AllowedHosts": "*"
}
```

> En producción: usar **User Secrets**, **Variables de Entorno** o **AWS Secrets Manager**.

---

##  Ejecución

Desde la raíz de la solución:

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/BTG.Api
```

Abrir Swagger: `https://localhost:7064/swagger`

---

##  Autenticación y roles

- Registro/Login en `/auth/register` y `/auth/login`
- El **access token** es un JWT con:
  - `sub`: Id de usuario (se usa como `cliente.Id` potencial)
  - `role`: `admin` o `cliente`
  - `iss`: `BTG.Api`
  - `aud`: `BTG.Client`

**Flujo recomendado (self‑service):**  
1) Registrar usuario como `cliente` → recibe JWT (`sub` = nuevo userId).  
2) Crear **Cliente** (perfil) en `/api/clientes` usando ese JWT (el `Id` del cliente será el `sub` del token).  
3) Suscribirse/Cancelar fondos y ver historial.

---

##  Endpoints (resumen)

>  = Requiere JWT / Rol

### Auth
- `POST /auth/register` – crea usuario (`cliente` o `admin`)
- `POST /auth/login` – retorna `accessToken`, `refreshToken`
- `POST /auth/refresh` – refresca tokens (si se habilita)
- `POST /auth/revoke` – revoca refresh tokens (si se habilita)

### Clientes
- `POST /api/clientes`  `role: cliente`  
  Crea el **perfil Cliente** con `Id` = `sub` del token.
- `GET /api/clientes/{id}`  `admin` o el mismo cliente

### Fondos
- `GET /api/fondos` (admin)
- `POST /api/fondos/suscribirse`  `role: cliente`  
  Body: `{ "fondoId": "1..5", "monto": 125000 }`  
  Reglas: valida monto mínimo, saldo disponible, suscripción duplicada.  
  **Error saldo**: “No tiene saldo disponible para vincularse al fondo {Nombre}”

- `POST /api/fondos/cancelar`  `role: cliente`  
  Body: `{ "fondoId": "1..5" }`  
  Reglas: reintegra saldo y registra transacción.

### Transacciones
- `GET /api/transacciones/historial/{clienteId}`  `admin` o el mismo cliente

---

##  Pruebas

- Proyecto: `tests/BTG.Tests`
- Frameworks: `xUnit`, `Moq`
- Cubre: reglas de negocio de **suscripción**, **cancelación** y **errores**

Ejecutar:
```bash
dotnet test
```

---

##  Modelo de datos NoSQL (Mongo)

**Colección `Fondos`** (IDs fijos del reto):
```json
{ "_id": "1", "nombre": "FPV_BTG_PACTUAL_RECAUDADORA", "montoMinimo": 75000, "categoria": "FPV" }
{ "_id": "2", "nombre": "FPV_BTG_PACTUAL_ECOPETROL",  "montoMinimo": 125000, "categoria": "FPV" }
{ "_id": "3", "nombre": "DEUDAPRIVADA",               "montoMinimo": 50000,  "categoria": "FIC" }
{ "_id": "4", "nombre": "FDO-ACCIONES",               "montoMinimo": 250000, "categoria": "FIC" }
{ "_id": "5", "nombre": "FPV_BTG_PACTUAL_DINAMICA",   "montoMinimo": 100000, "categoria": "FPV" }
```

**Colección `Clientes`**
```json
{
  "_id": "GUID",
  "nombre": "string",
  "email": "string",
  "telefono": "string",
  "saldo": 500000,
  "preferenciaNotificacion": 0, // Email=0, SMS=1
  "fondosActivos": [
    { "fondoId": "1..5", "nombre": "string", "monto": 125000, "fechaVinculacion": "2025-08-20T00:00:00Z" }
  ]
}
```

**Colección `Transacciones`**
```json
{
  "_id": "GUID",
  "clienteId": "GUID",
  "fondoId": "1..5",
  "tipo": "SUSCRIPCION|CANCELACION",
  "monto": 125000,
  "fecha": "2025-08-20T00:00:00Z"
}
```

---

## Arquitectura 

Se utilizó Clean Architecture con las siguientes capas:

BTG.Domain → Entidades del negocio (Cliente, Fondo, Transaccion).

BTG.Application → Interfaces de repositorios, servicios de dominio y DTOs.

BTG.Infrastructure → Implementaciones (MongoDB, JWT, notificaciones).

BTG.Api → Endpoints minimalistas (Minimal APIs) y configuración.

Esto asegura separación de responsabilidades, facilidad de pruebas y mantenibilidad.

- **Domain**: Entidades (`Cliente`, `Fondo`, `Transaccion`, `FondoActivo`), Enums.
- **Application**: Interfaces (`IClienteRepository`, `IFondoRepository`, `ITransaccionRepository`, `INotificacionService`, `IAuthService`, `ITokenProvider`), DTOs, Servicios (`FondoService`, `AuthService`), Excepciones.
- **Infrastructure**: `MongoContext`, Repositorios Mongo, `ConsoleNotificacionService`, `TokenProvider` (JWT).
- **Api**: Minimal APIs (`*.Endpoints.cs`), DI, Swagger, Middlewares.

Diagramas de flujo:

Suscribirse a fondo
----------------

```mermaid
sequenceDiagram
    participant U as Usuario<br/>(JWT=cliente)
    participant API as /api/fondos/suscribirse
    participant S as FondoService
    participant R1 as ClienteRepo
    participant R2 as FondoRepo
    participant R3 as TxRepo
    participant N as NotifSvc

    U->>API: POST {fondoId,monto} + JWT
    API->>S: SuscribirseAsync(clienteId, req)
    S->>R1: GetByIdAsync(clienteId)
    R1-->>S: Cliente
    S->>R2: GetByIdAsync(fondoId)
    R2-->>S: Fondo
    S->>S: Validar mínimo/saldo/duplicado
    S->>R1: UpdateAsync(cliente)
    S->>R3: AddAsync(Transacción)
    S->>N: EnviarSuscripcion(cliente,fondo,monto)
    S-->>API: txId
    API-->>U: 200 OK
```

Cancelar suscripción
------------------------

```mermaid
sequenceDiagram
    participant U as Usuario<br/>(JWT=cliente)
    participant API as /api/fondos/cancelar
    participant S as FondoService
    participant R1 as ClienteRepo
    participant R3 as TxRepo
    participant N as NotifSvc

    U->>API: POST {fondoId} + JWT
    API->>S: CancelarAsync(req)
    S->>R1: GetByIdAsync(req.ClienteId)
    R1-->>S: Cliente
    S->>S: Buscar fondo activo y validar
    S->>R1: UpdateAsync(Devolver saldo + remover activo)
    S->>R3: AddAsync(Transacción CANCELACION)
    S->>N: EnviarSuscripcion(cliente,fondo,-monto)
    S-->>API: txId
    API-->>U: 200 OK
```
