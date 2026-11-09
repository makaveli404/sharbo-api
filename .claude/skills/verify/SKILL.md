---
name: verify
description: Build and test the SharboAPI solution the same way CI does. Use after making code changes, before committing or opening a PR.
---

Run from the repo root:

1. `dotnet build --configuration Release` — fail fast on errors; report new warnings in files you touched.
2. `dotnet test` (add `--filter "FullyQualifiedName~$ARGUMENTS"` if arguments were given).
3. If a new `.csproj` was added, confirm the Dockerfile has a matching `COPY` line before `dotnet restore`.

Report: build status, test pass/fail counts, and for each failure the test name plus the relevant assertion message. Don't claim success unless both steps passed.
