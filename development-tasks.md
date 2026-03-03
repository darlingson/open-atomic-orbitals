# FLOW 1: Customer Onboarding (KYC → Eligible)

## High-Level Goal

A new person becomes:

`Non-existent → Registered → KYC Verified → Eligible`

---

# STEP 1 — Register Basic Profile

Atomic outcome: Customer created with `PendingKYC`

---

## EPIC 1 — Customer Registration Pipeline

---

###  TICKET 1.1

**Title:** Validate Registration Payload
**Owner:** ApiGateway
**Type:** Validation

**Description**
Validate required fields before calling TransactionEngine.

**Acceptance Criteria**

* FullName required
* NationalId required
* PhoneNumber required
* DateOfBirth required
* Reject invalid formats
* Return 400 on failure

**Definition of Done**
Gateway forwards only valid requests to TransactionEngine.

---

###  TICKET 1.2

**Title:** Define RegisterCustomer gRPC Contract
**Owner:** Shared Protos

**Type:** Contract

Define:

```plaintext
RegisterCustomerRequest
RegisterCustomerResponse
```

Used between:
ApiGateway → TransactionEngine
TransactionEngine → CustomerService

**Definition of Done**
.proto compiled and referenced by both services.

---

###  TICKET 1.3

**Title:** Implement RegisterCustomer Orchestration
**Owner:** TransactionEngine
**Type:** Orchestration

**Description**
TransactionEngine receives registration request and:

* Calls CustomerService.CheckCustomerExists
* If not exists → Call CreateCustomer
* Returns final response

**Definition of Done**
TransactionEngine contains orchestration flow (no DB logic).

---

###  TICKET 1.4

**Title:** Implement CheckCustomerExists
**Owner:** CustomerService
**Type:** gRPC

**Definition of Done**
CustomerService exposes:

* CheckByNationalId
* CheckByPhone

Returns Exists flag.

---

###  TICKET 1.5

**Title:** Implement CreateCustomer
**Owner:** CustomerService
**Type:** gRPC

**Behavior**

* Create customer
* Set Status = PendingKYC
* Generate CustomerId

**Definition of Done**
Returns:

* CustomerId
* Status

---

###  TICKET 1.6

**Title:** Emit CustomerCreated Event
**Owner:** CustomerService
**Type:** RabbitMQ Event

Event: `CustomerCreated`

Payload:

* CustomerId
* NationalId
* CreatedAt

**Definition of Done**
Event published after successful creation.

---

At this point:

`Non-existent → Registered (PendingKYC)` works.

---

## STEP 2 — Capture KYC Documents

Atomic outcome: Documents linked to customer.

---

## EPIC 2 — KYC Submission

---

###  TICKET 2.1

**Title:** Define SubmitKycDocuments gRPC Contract
**Owner:** Shared Protos

---

###  TICKET 2.2

**Title:** Implement SubmitKycDocuments Orchestration
**Owner:** TransactionEngine

Flow:

* Validate Customer exists
* Call CustomerService.StoreKycMetadata

---

###  TICKET 2.3

**Title:** Implement StoreKycMetadata
**Owner:** CustomerService

Stores:

* DocumentType
* StoragePath
* UploadedAt

---

###  TICKET 2.4

**Title:** Emit KycSubmitted Event
**Owner:** CustomerService

---

At this point:

Customer = PendingKYC
Documents = Attached

---

## STEP 3 — KYC Verification

Atomic outcome: Status → Verified OR Rejected

---

## EPIC 3 — Compliance Verification

---

###  TICKET 3.1

**Title:** Define VerifyKyc gRPC Contract
**Owner:** Shared Protos

---

###  TICKET 3.2

**Title:** Implement VerifyKyc Orchestration
**Owner:** TransactionEngine

Flow:

* Retrieve customer
* Call CustomerService.VerifyKyc

---

###  TICKET 3.3

**Title:** Implement VerifyKyc
**Owner:** CustomerService

Behavior:

* Update KycStatus
* Update CustomerStatus

---

###  TICKET 3.4

**Title:** Emit CustomerVerified Event
**Owner:** CustomerService

---

Now:

Customer = Verified

---

## STEP 4 — Eligibility Evaluation

Atomic outcome: Customer = Eligible / Ineligible

---

## EPIC 4 — Eligibility Engine

---

###  TICKET 4.1

**Title:** Define EvaluateEligibility gRPC Contract
**Owner:** Shared Protos

---

###  TICKET 4.2

**Title:** Implement Eligibility Orchestration
**Owner:** TransactionEngine

Flow:

* Call CustomerService.GetCustomer
* Call LoanService.GetActiveLoanCount
* Call CustomerService.EvaluateEligibility

---

###  TICKET 4.3

**Title:** Implement GetActiveLoanCount
**Owner:** LoanService

---

###  TICKET 4.4

**Title:** Implement EvaluateEligibility Logic
**Owner:** CustomerService

Logic:

* Age check
* Active loan count
* Credit score calculation

---

###  TICKET 4.5

**Title:** Emit CustomerEligible Event
**Owner:** CustomerService

---

# Final Ticket Count

| Step   | Tickets |
| ------ | ------- |
| Step 1 | 6       |
| Step 2 | 4       |
| Step 3 | 4       |
| Step 4 | 5       |

Total: **19 tickets**
