# Universal Unit Conversion API

A high-performance, enterprise-ready ASP.NET Core Web API built on **.NET 10** designed to dynamically process, validate, and convert numerical magnitudes across divergent measurement systems.

Rather than relying on brittle, hardcoded conversion matrices, this solution treats units as an interconnected network of nodes, using graph-theory algorithms to dynamically resolve complex or multi-step conversions at runtime.

---

## Repository Directory Structure

This repository uses a flat, decoupled multi-project structure sitting at the root directory:

```text
UnitConverter/ (Repository Root)
├── Models/                      # Core Data Contracts & Immutable Records
│   └── Models.csproj
├── Services/                    # Core Domain Graph Engine & Pathfinding Logic
│   └── Services.csproj
├── UnitConversionApi/           # Presentation Layer / ASP.NET Core Web API
│   └── UnitConversionApi.csproj
├── UnitConversionApi.Tests/     # Automated Quality Layer / xUnit v3 Test Suite
│   └── UnitConversionApi.Tests.csproj
└── UnitConversionApi.sln        # Visual Studio Solution File
```

---

## Technical Highlights & Features

### Graph-Driven Pathfinding

Automatically discovers multi-step transitive conversions by traversing connected unit relationships within the conversion graph.

Example:

```text
Kilometers → Meters → Feet → Centimeters
```

This allows the system to resolve indirect conversions dynamically without requiring every possible conversion pair to be explicitly defined.

### Linguistic Unit Normalization

Robust input normalization supports:

- Case-insensitive matching
- Singular and plural forms
- Common abbreviations

Examples:

| Input |
|---------|
| meter |
| meters |
| METER |
| m |

All resolve to the same canonical unit node.

### Categorical Dimensional Isolation

Units are grouped into isolated measurement domains.

Examples:

- Length ↔ Length ✅
- Temperature ↔ Temperature ✅
- Length ↔ Temperature ❌

If an API consumer attempts an invalid cross-category conversion (for example, `meters` to `celsius`), the graph search fails deterministically and returns a clear **400 Bad Request** response.

---

## Technical Highlights & Features

* **Graph-Driven Pathfinding:** Automatically discovers multi-step transitive conversions (e.g., converting `kilometers` to `centimeters` by traversing `Kilometers` $\rightarrow$ `Meters` $\rightarrow$ `Feet` $\rightarrow$ `Centimeters`).
* **Linguistic Unit Normalization:** Robust string parsing that seamlessly normalizes variations in case, grammatical plurality, and standard industry abbreviations (e.g., `meter`, `METERS`, and `m` map to the same node).
* **Categorical Dimensional Isolation:** Units exist on isolated structural "islands" within the graph network. If an API consumer attempts a cross-category operation (like `meters` to `celsius`), the pathfinder fails deterministically, generating an explicit `400 Bad Request` instead of a systemic failure.

---

## Getting Started & Local Execution (Windows)

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download) installed natively.
* You can install it quickly on Windows via **Windows Package Manager (winget)** using PowerShell or Command Prompt:
  ```powershell
  winget install Microsoft.DotNet.SDK.10

(Alternatively, download and run the standalone installer from the official Microsoft .NET website).

Verify the installation:

```powershell
dotnet --version
```

---

## 1. Compile the Solution

Open **PowerShell**, **Windows Terminal**, or **Command Prompt** from the repository root and run:

```powershell
dotnet build
```

---

## 2. Launch the Application Server

From the repository root:

```powershell
dotnet run --project UnitConversionApi\UnitConversionApi.csproj
```

Alternatively:

```powershell
cd UnitConversionApi
dotnet run
```

The API will start and listen on:

```text
http://localhost:5000
```

---

## 3. Interactive API Documentation

Once the server is running, open:

### Swagger UI

```text
http://localhost:5000
```

### OpenAPI Specification

```text
http://localhost:5000/swagger/v1/swagger.json
```

---

## Validating via Terminal (Smoke Testing)

Open a separate terminal while the API is running.

> **Windows PowerShell Note:** `curl` is aliased to `Invoke-WebRequest`, which behaves differently from the native curl executable. Use `curl.exe` explicitly or execute commands from CMD or Bash.

---

### Test a Valid Request (Celsius → Fahrenheit)

```bash
curl -i "http://localhost:5000/api/conversion?from=celsius&to=fahrenheit&value=25"
```

---

### Test Linguistic Normalization

```bash
curl -i "http://localhost:5000/api/conversion?from=m&to=kilometer&value=10"
```

---

### Test Cross-Category Isolation Error

```bash
curl -i "http://localhost:5000/api/conversion?from=meters&to=celsius&value=10"
```

---

## API Endpoint

### GET `/api/conversion`

Performs a unit conversion.

#### Query Parameters

| Parameter | Type | Required | Description |
|------------|--------|------------|-------------|
| from | string | Yes | Source unit |
| to | string | Yes | Destination unit |
| value | double | Yes | Numeric value to convert |

#### Example Request

```http
GET /api/conversion?from=kilometer&to=meter&value=5
```

#### Example Response

```json
{
  "from": "kilometer",
  "to": "meter",
  "originalValue": 5,
  "convertedValue": 5000
}
```

---

## Error Handling

### Missing Parameters

```http
400 Bad Request
```

```json
{
  "error": "Both 'from' and 'to' query parameters are required."
}
```

### Unsupported Conversion

```http
400 Bad Request
```

```json
{
  "error": "No valid conversion path exists between the requested units."
}
```

### Unexpected Server Error

```http
500 Internal Server Error
```

```json
{
  "error": "An unexpected error occurred while processing your request."
}
```

---

## Automated Test Pipeline

The automated test suite is built using **xUnit v3** and validates:

- Unit normalization behavior
- Identity conversions
- Multi-hop graph traversal
- Invalid conversion requests
- Edge-case handling
- Error response consistency

Run all tests from the repository root:

```bash
dotnet clean
dotnet test
```

## Architecture Overview

The solution follows a layered architecture:

```text
Client
   │
   ▼
ASP.NET Core API
   │
   ▼
Conversion Service
   │
   ▼
Graph-Based Conversion Engine
   │
   ▼
Unit Models & Conversion Relationships
```

### Projects

| Project | Responsibility |
|----------|---------------|
| Models | Domain contracts, records, and DTOs |
| Services | Conversion graph, pathfinding, normalization logic |
| UnitConversionApi | HTTP API endpoints and dependency injection |
| UnitConversionApi.Tests | Automated testing and validation |

---

## Technology Stack

- .NET 10
- ASP.NET Core Web API
- Swagger / OpenAPI
- xUnit v3
- Dependency Injection
- Graph-Based Pathfinding Algorithms