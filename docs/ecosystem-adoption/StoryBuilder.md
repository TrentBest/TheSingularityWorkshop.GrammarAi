# GrammarAi assessment for StoryBuilder

**Consumer coordination:** [StoryBuilder issue #8](https://github.com/TrentBest/StoryBuilder/issues/8)  
**Current recommendation:** document-only; no direct GrammarAi dependency justified by the current contract  
**Status:** corrected source-based assessment; awaiting consumer-owner review

## What the current source contract actually contains

StoryBuilder's current domain contract is centered on `StoryWorkspace`, not the `StoryProject` / `TimelineEvent` / `PassageAnchor` names used in the earlier draft of this proposal.

The current model includes:

- `TimelineDefinition`, `TimelinePlanningPoint`, and `StoryEvent` for world history and timeline placement;
- `SceneRecord` and ordered `SceneBeat` records for captured action, reaction, dialogue, pose, and related scene elements;
- `ManuscriptEdition` and `ManuscriptSection` for reading-order prose and references to portrayed scenes/events;
- stable record IDs and explicit links for participants, causes, consequences, sources, observer knowledge, and visual composition.

The current `CompositionManifestImporter` is a concrete source-driven JSON import boundary. Its documented contract checks ordered, contiguous page numbering (1–33 for the Lenny pilot), rejects missing/empty/out-of-order pages, maps supported cast and location records, and creates visual-asset targets. It maps into the existing `StoryWorkspace`; it is not intended to create a second project format.

## Why GrammarAi is not justified yet

The current importer and StoryBuilder domain already own the relevant validation:

- the importer owns the fixed manifest shape and its page-order requirements;
- StoryBuilder owns record identity, workspace membership, cross-record references, canon/review status, causal and temporal meaning, observer knowledge, provenance, and persistence;
- `StoryWorkspaceValidator.Validate` checks assembled/restored cross-record integrity and reports problems without repairing data.

Adding GrammarAi to the current importer would risk duplicating those checks or introducing a second schema for one fixed JSON format. GrammarAi does not parse JSON, replace DTO deserialization, establish canon, validate chronology, or determine narrative correctness. A typed C# record being structurally well-formed is not by itself a reason to introduce a grammar dependency.

**Recommendation today: document-only.** Keep the existing importer and domain validators authoritative. Do not add a GrammarAi package or source dependency to StoryBuilder on the basis of this proposal.

## A future trigger for reconsideration

Revisit GrammarAi only if StoryBuilder introduces a reusable, externally authored structural language that is genuinely more general than the current fixed manifest—for example, multiple independently authored composition manifests or declarative scene/experience structures that need the same bounded grammar across producers and consumers.

If that need appears, a safe pilot would:

1. Name the exact versioned external structure and its producer/consumer boundary.
2. Reuse the existing `StoryWorkspace` domain records; do not invent `StoryProject`, `TimelineEvent`, or `PassageAnchor` parallel models.
3. Keep JSON parsing/deserialization in the importer adapter. If useful, map the parsed candidate into a GrammarAi structure for bounded structural checks before domain mapping.
4. Keep page semantics, canonical IDs, chronology, causality, provenance, canon, observer knowledge, narrative quality, and persistence in StoryBuilder.
5. Test real manifest fixtures: valid input; missing, empty, duplicated, or out-of-order pages; unknown references; over-limit nesting/collections; and structurally valid but domain-invalid records.
6. Compare the GrammarAi checks against the existing importer and `StoryWorkspaceValidator` so the new layer proves non-duplicative value.
7. Pin an immutable GrammarAi revision and record exact consumer commit and CI evidence before calling it adopted.

## Ownership and safety boundary

GrammarAi owns grammar identity, node kinds, slots, productions, typed composition, and bounded structural validation. StoryBuilder owns story meaning and truth, timeline interpretation, causality, canon, observer knowledge, source provenance, narrative quality, persistence, and runtime behavior. A structurally valid proposal is not thereby true, canonical, narratively consistent, safe, or authorized.

## Decision requested from StoryBuilder

Please review this corrected assessment and record one disposition on [StoryBuilder issue #8](https://github.com/TrentBest/StoryBuilder/issues/8):

- **Document-only / no current use** — recommended for the current implementation;
- **Revisit later** — if a reusable external structural language emerges;
- **Adopt** — only with a concrete shared contract and real-model tests demonstrating value beyond the existing importer and domain validator.

No StoryBuilder source changes, dependency changes, or package changes are included or requested.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
