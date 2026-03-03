
# End-to-End Core Flows
1. Customer Onboarding (KYC → Eligible)
2. Loan Application & Approval
3. Loan Disbursement
4. Loan Repayment
5. End-of-Day Processing (Interest Accrual + Ledger Reconciliation)

These five flows cover:

* Identity
* Credit
* Money Movement
* Accounting
* Regulatory compliance


## Customer Onboarding

### Scenario

A field officer registers a new customer.

### End-to-End Flow

1. API Gateway receives request
2. TransactionEngine orchestrates
3. CustomerService:

   * Create customer
   * Set status = PendingKYC
4. LedgerService:

   * No financial entry yet
5. Event published: CustomerCreated

---

### Services Involved

* ApiGateway
* TransactionEngine
* CustomerService

---

### Required Inputs

```plaintext
FullName
NationalId
PhoneNumber
DateOfBirth
Address
EmploymentStatus
```

---

### Expected Outputs

```plaintext
CustomerId
CustomerStatus
CreditScore (initial)
CreatedAt
```

---

---

## FLOW 2 — Loan Application & Approval

### Scenario

Customer applies for a loan.

---

### End-to-End Flow

1. API → TransactionEngine
2. TransactionEngine calls:

   * CustomerService → Check eligibility
   * AccountService → Verify no active freeze
3. LoanService:

   * Create LoanApplication
   * Calculate repayment schedule
4. Approval decision
5. Event: LoanApproved

---

### Required Inputs

```plaintext
CustomerId
ProductType
PrincipalAmount
TenureMonths
InterestRate
```

---

### Expected Outputs

```plaintext
LoanId
AmortizationSchedule
MonthlyInstallment
TotalInterest
ApprovalStatus
```

---

---

## FLOW 3 — Loan Disbursement

This is the first real money movement.

---

### End-to-End Flow

1. API → TransactionEngine
2. LoanService:

   * Validate approved loan
3. AccountService:

   * Create Loan Account
4. LedgerService:

   * Debit: Loan Receivable
   * Credit: Cash at Bank
5. CustomerService:

   * Update ActiveLoanCount

---

### Required Inputs

```plaintext
LoanId
DisbursementAmount
DisbursementDate
```

---

### Expected Outputs

```plaintext
TransactionReference
LoanAccountId
LedgerEntryIds
DisbursementStatus
```

---

---

## FLOW 4 — Loan Repayment

Most important daily operation.

---

### End-to-End Flow

1. API → TransactionEngine
2. LoanService:

   * Calculate allocation:

     * Interest portion
     * Principal portion
3. AccountService:

   * Confirm payment account
4. LedgerService:

   * Debit: Cash
   * Credit: Interest Income
   * Credit: Loan Principal
5. LoanService:

   * Update outstanding balance
6. Event: RepaymentCompleted

---

### Required Inputs

```plaintext
LoanId
PaymentAmount
PaymentMethod
PaymentDate
```

---

### Expected Outputs

```plaintext
PrincipalPaid
InterestPaid
RemainingBalance
ReceiptNumber
LedgerEntries
```

---

---

## FLOW 5 — End of Day (EOD)

This separates amateurs from professionals.

---

### End-to-End Flow

Triggered by background scheduler.

1. LoanService:

   * Accrue daily interest on active loans
2. LedgerService:

   * Record accrued interest
3. LedgerService:

   * Generate trial balance
4. AuditLogService:

   * Store EOD report

---

### Required Inputs

```plaintext
BusinessDate
```

---

### Expected Outputs

```plaintext
TotalAccruedInterest
TrialBalanceReport
EODStatus
```

---
# Service - Responsibility Map

| Service           | Owns                            |
| ----------------- | ------------------------------- |
| CustomerService   | Identity + eligibility + status |
| AccountService    | Wallets + freeze/lien logic     |
| LoanService       | Loan lifecycle + amortization   |
| LedgerService     | Double-entry accounting         |
| TransactionEngine | Orchestration only              |
