Open Atomic Orbitals
=====================

Overview
--------
- Modular, domain‑driven MF core built as a set of .NET microservices.
- Focuses on correctness and traceability: SAGA‑based orchestration, immutable double‑entry ledger, and clear service boundaries.
- Ships with Docker support for local development and a shared Protobuf package for typed service contracts.

Core Services
-------------
- Customer Service (KYC): Identity verification, eligibility assessment, and lifecycle status.
- Account Service (Balances): Wallet and account state machine; manages ledger vs. available balances.
- Loan Service (Credit): Origination, disbursement, schedules, repayments, delinquency tracking.
- Ledger Service (GL): Immutable, append‑only double‑entry general ledger as the system of record.
- Transaction Engine (Orchestrator): SAGA coordinator for multi‑service, atomic business workflows.
- Interest Accrual Engine: Daily accrual and periodic posting to GL and accounts.
- Audit Log Service: Immutable audit trail for compliance and operational forensics.
- EOD Processing: Reconciliation, statements, fee assessments, and business date rollovers.

Architecture
------------
- Technology
  - .NET microservices (targets modern .NET; see csproj files in services).
  - gRPC contracts via shared Protobufs in BuildingBlocks/Protos.
  - API Gateway for external access and aggregation.
- Patterns
  - SAGA orchestration in Transaction Engine for distributed consistency.
  - Ledger enforces double‑entry invariants (Total Debits = Total Credits).
  - Idempotent operations and immutable event/entry persistence where applicable.
- Deployability
  - Each service has its own Dockerfile; a docker‑compose file wires up local dependencies.

Repository Structure
--------------------
- src/Services/
  - ApiGateway
  - AccountService
  - CustomerService
  - LoanService
  - LedgerService
  - TransactionEngine
- src/BuildingBlocks/Protos
- docker/ and docker‑compose.yml
- system-design.md (high‑level system blueprint)
- MFCore.slnx (solution entry point)
- .env files (root and per‑service overrides as needed)

Local Development
-----------------
Prerequisites:
- .NET SDK (8 or later recommended)
- Docker and Docker Compose
- Bash or PowerShell

Build:
- Open the solution [MFCore.slnx] in Visual Studio/VS Code, or run:
  - dotnet restore
  - dotnet build MFCore.slnx

Run (all services via Docker):
- Ensure environment variables are set.
- From the repo root:
  - docker compose up --build

Run (single service):
- Example to run the API Gateway without Docker:
  - dotnet run --project src/Services/ApiGateway/ApiGateway.csproj
- Each service includes launchSettings and appsettings files to control ports and bindings.

Configuration
-------------
- Per‑service configuration lives in appsettings.json and appsettings.Development.json.
- Kestrel and port settings are defined per service; see launchSettings.json for local profiles.
- Root [.env]  and service‑level .env files provide connection strings and secrets for local runs.

Business Workflows (Examples)
-----------------------------
- Loan Disbursement (coordinated by Transaction Engine):
  - AccountService: verify/create accounts and available balance state.
  - LedgerService: post balanced entries (e.g., Debit Cash at Bank / Credit Loan Receivable).
  - CustomerService: update exposure and active‑loan counters.
  - LoanService: transition application to Active.

Learn More
----------
- System blueprint and domain responsibilities: [system-design.md]

Contributing
------------
- Use conventional C#/.NET coding patterns per existing services.
- Keep service boundaries clean; prefer contract evolution via Protobufs.
- Validate SAGA flows end‑to‑end and preserve ledger immutability.

License
-------
- License information will be added to the repository. Until then, treat this as “All rights reserved” for non‑production use.
