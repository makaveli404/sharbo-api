# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

- Build like CI: `dotnet build --configuration Release`
- Test: `dotnet test` (single test: `dotnet test --filter "FullyQualifiedName~MemeServiceTests.GetByIdAsync"`)
- Run: `docker compose up -d db`, then `dotnet run --project SharboAPI` (Swagger only in Development)
- API + Postgres 18: `docker compose up --build`
- Config split: `.env` is read only by docker compose; .NET reads `appsettings*.json`. `appsettings.Development.json` holds the dev connection string (`127.0.0.1:5434`, password `sharbo_dev`) matching the compose defaults, so keep them in sync. It's a local-only dev password; real secrets never go into appsettings.

## Local setup

- Firebase: accounts and sign-in live entirely in Firebase (clients use the Firebase SDK); the API has no Firebase SDK and no service-account key. It only needs the public `Firebase:ProjectId` (dev project `sharbo-1`): `dotnet user-secrets set "Firebase:ProjectId" "<id>" --project SharboAPI` or `FIREBASE_PROJECT_ID` in `.env`.
- Auth: every endpoint needs `Authorization: Bearer <Firebase ID token>` (fallback authorization policy; a public endpoint must call `AllowAnonymous()`, and Swagger is mapped before auth so it stays open in Development), verified with Google's public keys in `SharboAPI/Extensions/AuthenticationExtensions.cs` (issuer = `Firebase:TokenIssuerBaseUrl` from `appsettings.json` + `Firebase:ProjectId`). A missing project ID stops startup outside Development; in Development protected endpoints just return 401. Get a dev token via the Firebase Auth REST API (`accounts:signInWithPassword` with the dev project's Web API key, use `idToken`) and paste it into Swagger's Authorize. The UID (`sub`) and e-mail (`email`) are read from the token by `ICurrentUser` (`SharboAPI/Services/CurrentUser.cs`). There is no sign-up/login endpoint: after the first Firebase sign-in the client calls `POST /api/me` with a nickname to create the `Users` row (ID and e-mail from the token); `GET /api/me` returns 404 until then. Users have no password column.
- PostgreSQL only (Npgsql), configured in `SharboAPI.Infrastructure/Extensions/ServiceCollectionExtensions.cs`. `dotnet ef` builds the API host, so it needs the connection string configured (user secrets or env).
- Pending migrations are auto-applied on startup and `Seeder` seeds roles. In Development `DevDataSeeder` (`SharboAPI.Infrastructure/Seeding/`) then adds demo data: profiles of the 10 shared Firebase test accounts (tester01–10@example.com, fixed UIDs in `DevData.cs`) and 5 groups with members, roles, quotes, situations and memes. It is idempotent (skips existing profiles and groups) and can be turned off with `Seed:DevData=false`. Test account passwords are in the git-ignored `test-accounts.json`, passed between developers outside git; `DevDataTests` keeps the data within validator limits.

## Migrations

Do NOT add, edit, or regenerate EF Core migrations without asking first (a hook enforces this for `SharboAPI.Infrastructure/Migrations/`). When approved: `dotnet tool restore`, then `dotnet ef migrations add <Name> -p SharboAPI.Infrastructure -s SharboAPI`. Migrations are also applied automatically on API startup.

## Architecture conventions

Clean Architecture: Domain ← Application ← Infrastructure ← SharboAPI (Minimal API). The README tech stack is partly aspirational — there is no AutoMapper, CQRS/MediatR, or SignalR in the code; don't assume they exist.

- Domain models: private setters, private parameterless ctor, static `Create(...)` factory and mutation methods; entries (Meme/Quote/Situation) derive from `Entry` and call `Entry.Set` / `Update`.
- Repository and service interfaces live in `SharboAPI.Application/Abstractions/`; implementations are registered manually in the two `ServiceCollectionExtensions.cs` files (Application and Infrastructure). New endpoint groups must be mapped in `Program.cs`.
- Services return `Result` / `Result<T>` with `Error.NotFound(...)` etc.; DTOs are mapped by hand. Validators are FluentValidation, injected as `IValidator<T>` and called with `ValidateAndThrowAsync`.
- Services take the user from `ICurrentUser` (never a hardcoded ID). Everything under a group (`GroupService`, `GroupParticipantService`, Quote/Meme/Situation services) first checks that the current user is a member; a non-member or missing group is `Error.NotFound` and endpoints answer 404, so other groups aren't revealed. Entry repositories look entries up by `(groupId, id)` so an ID from another group is not found. Roles (Admin/Moderator) are not enforced yet: any member can manage the group.
- `.editorconfig` sets tabs for C#, but many existing files still use spaces — use tabs in new files, match the surrounding file in existing ones, and don't run a repo-wide `dotnet format` (it would rewrite whitespace everywhere).

## Tests

xUnit + Moq + FluentAssertions, Arrange/Act/Assert comments, names like `Method_Scenario_ExpectedResult`. Service tests in `SharboAPI.Application.Tests/Services`, model tests in `SharboAPI.Domain.Tests`.

## Gotchas

- The Dockerfile restores only `SharboAPI.csproj` after copying the `.csproj` files it references; a new project referenced by the API needs its own `COPY` line there or the image build breaks (test projects don't).

## Git

- Commits: `#<issue>: <message>` for issue work, `EX: <message>` for misc/chores.
- Branches: `feature/#<issue>-<name>` or `issue/#<issue>`; PRs target `main` (squash merge, PR number appended).
