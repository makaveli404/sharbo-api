# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

- Build like CI: `dotnet build --configuration Release`
- Test: `dotnet test` (single test: `dotnet test --filter "FullyQualifiedName~MemeServiceTests.GetByIdAsync"`)
- Run: `dotnet run --project SharboAPI` (Development → SQLite file `sharbolocaldb.db`; Swagger only in Development)
- Full stack (API + Postgres 17): `docker compose up --build`

## Local setup

- Startup throws unless `Firebase:Credentials` (service-account JSON string) is configured. Locally it comes from user secrets: `dotnet user-secrets set "Firebase:Credentials" "<json>" --project SharboAPI`. Never commit it to appsettings.
- `DatabaseProvider` (`SQLite` | `PostgreSQL`) selects the EF provider in `SharboAPI.Infrastructure/Extensions/ServiceCollectionExtensions.cs`; both share one `SharboDbContext` and one migration set.
- Pending migrations are auto-applied on startup and `Seeder` seeds roles.

## Migrations

Do NOT add, edit, or regenerate EF Core migrations without asking first (a hook enforces this for `SharboAPI.Infrastructure/Migrations/`). When approved: `dotnet ef migrations add <Name> -p SharboAPI.Infrastructure -s SharboAPI`.

## Architecture conventions

Clean Architecture: Domain ← Application ← Infrastructure ← SharboAPI (Minimal API). The README tech stack is partly aspirational — there is no AutoMapper, CQRS/MediatR, or SignalR in the code; don't assume they exist.

- Domain models: private setters, private parameterless ctor, static `Create(...)` factory and mutation methods; entries (Meme/Quote/Situation) derive from `Entry` and call `Entry.Set` / `Update`.
- Repository and service interfaces live in `SharboAPI.Application/Abstractions/`; implementations are registered manually in the two `ServiceCollectionExtensions.cs` files (Application and Infrastructure). New endpoint groups must be mapped in `Program.cs`.
- Services return `Result` / `Result<T>` with `Error.NotFound(...)` etc.; DTOs are mapped by hand. Validators are FluentValidation, injected as `IValidator<T>` and called with `ValidateAndThrowAsync`.
- Services currently use a hardcoded placeholder user ID (TODO: read from claims) — keep it unless the task is implementing auth.
- `.editorconfig` sets tabs for C#, but many existing files still use spaces — use tabs in new files, match the surrounding file in existing ones, and don't run a repo-wide `dotnet format` (it would rewrite whitespace everywhere).

## Tests

xUnit + Moq + FluentAssertions, Arrange/Act/Assert comments, names like `Method_Scenario_ExpectedResult`. Service tests in `SharboAPI.Application.Tests/Services`, model tests in `SharboAPI.Domain.Tests`.

## Gotchas

- The Dockerfile copies each `.csproj` explicitly before `dotnet restore`; adding a new project to the solution requires adding a `COPY` line there or the image build breaks.

## Git

- Commits: `#<issue>: <message>` for issue work, `EX: <message>` for misc/chores.
- Branches: `feature/#<issue>-<name>` or `issue/#<issue>`; PRs target `main` (squash merge, PR number appended).
