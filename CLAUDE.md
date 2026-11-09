# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

- Build like CI: `dotnet build --configuration Release`
- Test: `dotnet test` (single test: `dotnet test --filter "FullyQualifiedName~MemeServiceTests.GetByIdAsync"`)
- Run: `dotnet run --project SharboAPI` (needs `ConnectionStrings:SharboDbConnection` in user secrets, e.g. pointing at the compose database on `127.0.0.1:$POSTGRES_PORT`; Swagger only in Development)
- API + Postgres 18: `cp .env.example .env`, fill in `POSTGRES_PASSWORD` (and `POSTGRES_PORT` if 5432 is taken), then `docker compose up --build`

## Local setup

- Firebase (`Firebase:Credentials` service-account JSON, optional `Firebase:ProjectId`) is created lazily on first use (`AddFirebase` in `SharboAPI.Infrastructure/Extensions/FirebaseServiceCollectionExtensions.cs`). Outside Development a missing key blocks startup; in Development the API starts with a warning and only user registration/lookups fail. Use a separate dev Firebase project; set its key via `dotnet user-secrets set "Firebase:Credentials" "<json>" --project SharboAPI` or `FIREBASE_CREDENTIALS` in `.env`. Never commit it.
- PostgreSQL only (Npgsql), configured in `SharboAPI.Infrastructure/Extensions/ServiceCollectionExtensions.cs`. `dotnet ef` builds the API host, so it needs the connection string configured (user secrets or env).
- Pending migrations are auto-applied on startup and `Seeder` seeds roles.

## Migrations

Do NOT add, edit, or regenerate EF Core migrations without asking first (a hook enforces this for `SharboAPI.Infrastructure/Migrations/`). When approved: `dotnet tool restore`, then `dotnet ef migrations add <Name> -p SharboAPI.Infrastructure -s SharboAPI`. Migrations are also applied automatically on API startup.

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

- The Dockerfile restores only `SharboAPI.csproj` after copying the `.csproj` files it references; a new project referenced by the API needs its own `COPY` line there or the image build breaks (test projects don't).

## Git

- Commits: `#<issue>: <message>` for issue work, `EX: <message>` for misc/chores.
- Branches: `feature/#<issue>-<name>` or `issue/#<issue>`; PRs target `main` (squash merge, PR number appended).
