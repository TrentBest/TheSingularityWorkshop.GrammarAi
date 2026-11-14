# GrammarAi assessment for Gestures

**Consumer coordination:** [Gestures issue #35](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/issues/35)  
**Disposition:** no current use  
**Status:** source-reviewed assessment; future authoring opportunity only

## Source-backed finding

The current repository has a deterministic, stateless pose evaluator, caller-driven single-gesture and fixed ordered-sequence playback, provider-offer eligibility, and an initial immutable ActivityDefinition builder. The activity builder validates local IDs, versions, duplicate classifications/phases/roles/references, participant bounds, and nonempty phases. The documented activity model explicitly distinguishes reusable Gesture procedures, versioned ActivityDefinitions, live sessions, assignments, runtime patches, observations, and presentation.

That is meaningful domain structure, but the current evidence does not establish a shared serialized grammar boundary. The implemented builder is a local typed authoring API; the documented broader activity graph, serialization, provider resolution, live session coordinator, deterministic role assignment, and atomic session patching remain future work. GestureSequencePlayback is a fixed ordered list, not a branching or parallel choreography language. Introducing GrammarAi now would risk duplicating local C# contracts before a second producer or consumer exists.

## Reconsider only when

A real authoring or interchange surface needs to submit versioned, nested activity/gesture proposals independently of the Gestures assembly—for example, an editor or AI authoring client producing the same serialized graph consumed by more than one tool. At that point, test one actual ActivityDefinition against a pinned GrammarAi grammar, including qualified Gesture/provider references, cardinality, ordering, depth, stale identities, and structurally valid but domain-invalid cases.

## Ownership boundary

Gestures retains pose data, deterministic evaluation, playback lifecycle, provider eligibility, recognition, and temporal/spatial interpretation. Physics retains contact, obstruction, reachability, and physical outcomes. The activity host retains intent, participation policy, authorization, session coordination, and application of runtime changes. A structurally valid sequence is not proof of intent, recognition, permission, or physical success.

No consumer source or dependency changes are proposed.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
