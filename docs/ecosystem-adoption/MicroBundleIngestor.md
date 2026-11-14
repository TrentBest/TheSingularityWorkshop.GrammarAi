# GrammarAi assessment for MicroBundleIngestor

**Consumer coordination:** [MicroBundleIngestor issue #5](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleIngestor/issues/5)  
**Disposition:** no current use; reconsider only for a distinct shared envelope outside the OpenAPI ingestion path  
**Status:** source-informed assessment for consumer-owner review; not an approved integration

## What exists today

MicroBundleIngestor already has a defined ingestion pipeline. Its OpenAPI/Swagger JSON URL adapter produces a canonical API experience definition; a capability compiler maps that into a transport-neutral capability definition with operations, ports, and semantic hints. The README documents separate capability-API, content-producing-API, and repository-capability paths.

OpenAPI is already a formal, structured contract for the primary ingestion source. Introducing GrammarAi to describe or validate that same source would duplicate an existing schema language and ingestion path. The current domain documentation also separates source description, capability definition, and Experience composition.

## Recommendation

Do not add GrammarAi to the current OpenAPI ingestion path, and do not invent a generic ingestion request/result grammar without a concrete cross-repository consumer. GrammarAi is not needed merely because the input is structured JSON.

Reconsider only if multiple independent producers and consumers need a shared, versioned envelope that is distinct from the OpenAPI document—for example, a source-provenance/ingestion-job handoff or a normalized capability proposal exchanged across process boundaries. That envelope should contain references and bounded metadata, not replace the source document or claim that content is trustworthy.

MicroBundleIngestor retains source acquisition, OpenAPI parsing, canonicalization, capability compilation, inspection, validation behavior, error handling, and artifact lifecycle. The host retains credentials, network policy, approval, and publication authorization. A structurally valid envelope must not imply that its source or payload has been inspected, trusted, approved, or published.

## Minimal proof if a separate envelope appears

1. Identify two real independent producers/consumers and the exact handoff they share.
2. Keep OpenAPI and source payloads opaque; define only the distinct envelope.
3. Pin GrammarAi and map the envelope to existing Ingestor types.
4. Test missing provenance, malformed references, unsupported categories, stale identities, and size/depth bounds.
5. Separately prove that payload inspection and trust checks still reject malicious or invalid content after structural validation.

## Evidence and ownership boundaries

- [MicroBundleIngestor README](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleIngestor/blob/development/README.md) documents the OpenAPI adapter, canonical API definition, capability compiler, and distinct repository REST opt-in.
- [Domain model](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleIngestor/blob/development/docs/DOMAIN.md) separates source description, capability definition, and Experience composition, and states that policy metadata is not enforcement.

ProtocolAi may provide stable vocabulary identities and typed lifecycle envelopes if a cross-process consumer actually needs them. GrammarAi owns structural shape only; neither package acquires, inspects, trusts, authorizes, or publishes content.

## Decision requested

Please confirm **no current use** for the existing local OpenAPI ingestion pipeline, or point to a real multi-party envelope contract that is not already represented by OpenAPI or the Ingestor's canonical domain types. No consumer source changes or dependency additions are proposed.
