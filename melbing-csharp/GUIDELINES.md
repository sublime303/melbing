# Melbing Ship Log — development guidelines

Preferences distilled from Melbing’s review of the C# port. Use these when adding features or refactoring.

## Layered architecture

Keep a clear split between projects so each layer does one job:

| Project | Owns | Does not own |
|---|---|---|
| **Domain** | Entities and core domain types | Persistence, HTTP, JSON adapters |
| **Application** | Use cases, DTOs/contracts, service interfaces, validation | EF Core, SQLite, device/JSON converters, UI |
| **Infrastructure** | Database, repositories, serialization/adapters for external formats | Business rules, HTTP endpoints |
| **Web** | Minimal APIs, Razor Pages, host/DI wiring | Domain rules, data access details |

**Dependency direction (never reverse):**

```
Web → Infrastructure → Application → Domain
Web → Application → Domain
```

Application must **not** reference Infrastructure. Domain must **not** reference anything above it.

## Technical adapters belong in Infrastructure

Things that exist because of *how* we talk to the outside world (database, ESP32 JSON quirks, file formats) live in Infrastructure — even when they feel like “logic”.

Example Melbing called out: flexible bool parsing (`0`/`1` ↔ `bool`) for ArduinoJson-style payloads belongs in Infrastructure, not next to Application use cases.

Prefer:

- Implement adapters in `Infrastructure` (e.g. `Serialization/`, `Data/`, `Repositories/`)
- Register them from the host (`Web` / `Program.cs`), e.g. `AddEsp32Converters()`
- Keep Application DTOs free of Infrastructure types (`[JsonConverter(typeof(...))]` that would pull Infrastructure into Application)

Avoid:

- Putting EF, SQLite, or format converters in Application “because it’s logic”
- Letting Application depend on Infrastructure to reuse those adapters

## Gray zones

Placement is sometimes ambiguous. That is fine — choose deliberately:

1. Prefer the layer that matches *why* the code exists (business rule vs technical adapter vs HTTP).
2. When unsure, keep Application thinner and push technical detail outward (Infrastructure / Web).
3. Stay consistent with existing patterns in this solution.

## API contract with the boat

The ESP32 firmware contract must stay compatible (`POST /api/log`, snake_case fields, numeric bools). Prefer adapting on the server (Infrastructure serialization) over changing device firmware.

## Tests

- Cover Application services with fakes/mocks for repository interfaces.
- Cover HTTP + ESP32-style payloads with integration tests against Web.
- When moving adapters between layers, update namespaces and registration, not only file location.
