# GrammarAi assessment for FSM_Foundry

**Consumer coordination:** [FSM_Foundry issue #37](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/37)  
**Disposition:** no current use; revisit only for a separately authored declarative workflow language  
**Status:** source-informed assessment for consumer-owner review; not an integration proposal

## What exists today

The current Foundry workflow contract is a data-only, version-1 definition containing an identifier, name, schema version, and ordered steps. Each step has an identifier and a host-registered handler key. Foundry validates the complete definition before invoking handlers, then owns sequential execution, lifecycle, cancellation, outcomes, and retained run snapshots.

This is a concrete and deliberately narrow contract. The inspected source does not establish a reusable external grammar for nested groups, typed input/output ports, dependency edges, triggers, or a general orchestration language. The broader proposal would therefore design a new language before identifying a real producer/consumer boundary.

## Recommendation

Do not add GrammarAi to Foundry for the current workflow definition. The existing typed C# contract and Foundry validator are the clearer owners of its fixed shape. GrammarAi would add a second schema layer without a demonstrated structural gap.

Reconsider only if Foundry introduces a real externally authored/interchanged plan format whose structure is shared across independent producers and consumers—for example, typed step composition with explicit ports and references. In that case, GrammarAi may validate the authored structure before conversion to Foundry's canonical workflow types.

Foundry must continue to own workflow semantics, scheduling, state progression, retries, lifecycle, cancellation, resource limits, and execution. Structural validity must never imply that a handler is registered, permitted, safe, or executable.

## Evidence and ownership boundaries

- [WorkflowDefinition.cs](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/blob/development/src/Foundry.Workflow/WorkflowDefinition.cs) defines the current version-1 data-only contract.
- [Foundry domain and ownership](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/blob/development/docs/DOMAIN.md) assigns validation and execution semantics to Foundry.
- [Contract inspection ledger](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/blob/development/docs/CONTRACT_INSPECTION.md) explicitly distinguishes Foundry workflows from FSM_COS RuntimeManifest/RuntimeAssembly.

FSM_COS composition is not Foundry workflow execution. CommandAi owns atomic command contracts. CiAi owns CI evidence and gate decisions. GrammarAi must not become a workflow engine, scheduler, authorization layer, or executor.

## Decision requested

Please confirm **no current use** for the existing version-1 workflow contract, or identify a concrete independently authored plan format that is not already covered by Foundry's typed model. No consumer source changes or dependency additions are proposed.
