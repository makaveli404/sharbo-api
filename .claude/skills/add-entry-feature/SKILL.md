---
name: add-entry-feature
description: Checklist for adding a new entity/feature (model, persistence, service, DTOs, validators, endpoints, tests) across the Clean Architecture layers of SharboAPI. Use when adding a new resource or CRUD feature.
---

Add the feature described in $ARGUMENTS following the existing Meme/Quote/Situation pattern. Read the Meme files of each layer first and mirror them.

1. **Domain** — `SharboAPI.Domain/Models/<Name>.cs`: private setters, private ctor, `static Create(...)`, mutation methods. Entries derive from `Entry`.
2. **Infrastructure persistence**
   - `Configurations/<Name>Configuration.cs` (`IEntityTypeConfiguration<T>`; picked up automatically).
   - Add `DbSet<Name>` to `SharboDbContext`.
   - `Repositories/<Name>Repository.cs` implementing the new interface.
   - Schema changed → STOP and ask the user before creating a migration.
3. **Application**
   - `Abstractions/Repositories/I<Name>Repository.cs`, `Abstractions/Services/I<Name>Service.cs`.
   - `DTO/<Name>/Create<Name>Request.cs`, `Update<Name>Request.cs`, `<Name>Result.cs` (records).
   - `Validators/<Name>/Create…Validator.cs`, `Update…Validator.cs` (auto-registered by assembly scan).
   - `Services/<Name>Service.cs` returning `Result`/`Result<T>`, calling `ValidateAndThrowAsync`, mapping DTOs by hand.
4. **DI** — register the repository in `SharboAPI.Infrastructure/Extensions/ServiceCollectionExtensions.cs` and the service in `SharboAPI.Application/Extensions/ServiceCollectionExtensions.cs`.
5. **API** — `SharboAPI/Endpoints/<Name>Endpoints.cs` with a `Map<Name>Endpoints` extension under `/api/groups/{groupId:guid}/<names>`; call it in `Program.cs`.
6. **Tests** — `SharboAPI.Domain.Tests/<Name>Tests.cs` and `SharboAPI.Application.Tests/Services/<Name>ServiceTests.cs` (xUnit, Moq, FluentAssertions, AAA).
7. Run `dotnet build --configuration Release` and `dotnet test`; fix failures before reporting done.
