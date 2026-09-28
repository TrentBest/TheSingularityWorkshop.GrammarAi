# GrammarAI QuickStart

The shortest executable demonstration of the current GrammarAI alpha.

From the repository root:

```bash
dotnet run --project examples/GrammarAi.QuickStart/GrammarAi.QuickStart.csproj
```

The example defines one grammar with two alternatives over externally owned protocol symbols.

It demonstrates the boundary:

- **ProtocolAI owns WHAT** the symbols mean.
- **GrammarAI owns HOW** those symbols may be connected.

For a real consumer:

```bash
dotnet add package TheSingularityWorkshop.GrammarAi --version 0.1.0-alpha.1
```

The current example uses a project reference so the repository exercises its own source directly.
