# GrammarAi assessment for FSM_Execution

**Consumer coordination:** [FSM_Execution issue #37](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Execution/issues/37)  
**Disposition:** no current use; revisit only for a separately authored declarative workflow language  
**Status:** source-informed assessment for consumer-owner review; not an integration proposal

## What exists today

The current FSM_Execution workflow contract is a data-only, version-1 definition containing an identifier, name, schema version, and ordered steps. Each step has an identifier and a host-registered handler key. FSM_Execution validates the complete definition before invoking handlers, then owns sequential execution, lifecycle, cancellation, outcomes, and retained run snapshots.

This is a concrete and deliberately narrow contract. The inspected source does not establish a reusable external grammar for nested groups, typed input/output ports, dependency edges, triggers, or a general orchestration language. The broader proposal would therefore design a new language before identifying a real producer/consumer boundary.

## Recommendation

Do not add GrammarAi to FSM_Execution for the current workflow definition. The existing typed C# contract and FSM_Execution validator are the clearer owners of its fixed shape. GrammarAi would add a second schema layer without a demonstrated structural gap.

Reconsider only if FSM_Execution introduces a real externally authored/interchanged plan format whose structure is shared across independent producers and consumers—for example, typed step composition with explicit ports and references. In that case, GrammarAi may validate the authored structure before conversion to FSM_Execution's canonical workflow types.

FSM_Execution must continue to own workflow semantics, scheduling, state progression, retries, lifecycle, cancellation, resource limits, and execution. Structural validity must never imply that a handler is registered, permitted, safe, or executable.

## Evidence and ownership boundaries

- [WorkflowDefinition.cs](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Execution/blob/0561be668e3986c671d34b1e4a7b9da33fa68cd6/src/Execution.Workflow/WorkflowDefinition.cs) defines the current version-1 data-only contract.
- [FSM_Execution domain and ownership](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Execution/blob/0561be668e3986c671d34b1e4a7b9da33fa68cd6/docs/DOMAIN.md) assigns validation and execution semantics to FSM_Execution.
- [Contract inspection ledger](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Execution/blob/0561be668e3986c671d34b1e4a7b9da33fa68cd6/docs/CONTRACT_INSPECTION.md) explicitly distinguishes FSM_Execution workflows from FSM_COS RuntimeManifest/RuntimeAssembly.

FSM_COS composition is not FSM_Execution workflow execution. CommandAi owns atomic command contracts. CiAi owns CI evidence and gate decisions. GrammarAi must not become a workflow engine, scheduler, authorization layer, or executor.

## Decision requested

Please confirm **no current use** for the existing version-1 workflow contract, or identify a concrete independently authored plan format that is not already covered by FSM_Execution's typed model. No consumer source changes or dependency additions are proposed.
