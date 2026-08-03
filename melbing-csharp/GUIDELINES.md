# Melbing Ship Log — development guidelines

Preferences distilled from Melbing’s review of the C# port. Use these when adding features or refactoring.

This is a small app — prefer branch-standard naming and simple host wiring over enterprise ceremony.

## Layered architecture

Keep a clear split between projects so each layer does one job:

| Project | Owns | Does not own |
|---|---|---|
| **Domain** | Entities, DTOs | Persistence, HTTP, JSON adapters |
| **Application** | Use cases/services, interfaces (`Interfaces/`), validation | EF Core, SQLite, device/JSON converters, UI, DI registration |
| **Infrastructure** | Database, repositories, serialization/adapters for external formats | Business rules, HTTP endpoints |
| **Web** | Minimal APIs, Razor Pages, host/`Program.cs` DI wiring | Domain rules, data access details |

**Dependency direction (never reverse):**

```
Web → Infrastructure → Application → Domain
Web → Application → Domain
```

Application must **not** reference Infrastructure. Domain must **not** reference anything above it.

## Naming and project layout

- Put service/repository contracts in **`Interfaces/`**, not `Abstractions/`.
- Put request/response DTOs under **Domain** (`Domain/Dtos/`), not Application.
- Register Application services in **`Program.cs`** for this small app. Do not add an `AddApplication()` DI helper in the Application project. Infrastructure may keep `AddInfrastructure()` when it wires EF and repositories.

## EF Core model configuration

`ShipLogDbContext` may keep entity mapping inline while there is a single aggregate. If more entities appear, extract configurations into separate files (e.g. `IEntityTypeConfiguration<T>`).

## Technical adapters belong in Infrastructure

Things that exist because of *how* we talk to the outside world (database, ESP32 JSON quirks, file formats) live in Infrastructure — even when they feel like “logic”.

Example: flexible bool parsing (`0`/`1` ↔ `bool`) for ArduinoJson-style payloads belongs in Infrastructure.

Prefer:

- Implement adapters in `Infrastructure` (e.g. `Serialization/`, `Data/`, `Repositories/`)
- Register JSON converters from the host (`Web` / `Program.cs`), e.g. `AddEsp32Converters()`
- Keep Domain DTOs free of Infrastructure types (`[JsonConverter(typeof(...))]` that would pull Infrastructure into Domain)

Avoid:

- Putting EF, SQLite, or format converters in Application “because it’s logic”
- Letting Application or Domain depend on Infrastructure to reuse those adapters

## Gray zones

Placement is sometimes ambiguous. That is fine — choose deliberately:

1. Prefer the layer that matches *why* the code exists (business rule vs technical adapter vs HTTP).
2. When unsure, keep Application thinner and push technical detail outward (Infrastructure / Web).
3. Stay consistent with existing patterns in this solution.
4. Do not over-engineer for a small codebase; follow common branch conventions first.

## API contract with the boat

The ESP32 firmware contract must stay compatible (`POST /api/log`, snake_case fields, numeric bools). Prefer adapting on the server (Infrastructure serialization) over changing device firmware.

## Tests

- Cover Application services with fakes/mocks for repository interfaces.
- Cover HTTP + ESP32-style payloads with integration tests against Web.
- When moving types between layers, update namespaces and registration, not only file location.
