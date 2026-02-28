# System Design: Financial Core Services

## 1. Customer Service (The KYC Domain)
**Responsibility**: Identity Verification, Eligibility Assessment, and Customer Lifecycle Management.

**Core Logic**:
- Manages the comprehensive "Status" of a customer (e.g., Active, Suspended, Dormant).
- **Credit Scoring**: Implements logic to calculate dynamic credit scores based on repayment history fetched from the Loan Service and external data points.
- **KYC Compliance**: Handles document verification and tier-based limits.

## 2. Account Service (The Balance Domain)
**Responsibility**: Wallet and Account Management.

**Core Logic**:
- Acts as the **State Machine** for funds, managing states such as Active, Frozen, Lien (holding funds), and Closed.
- **Balance Management**: Distinguishes between Ledger Balance (total funds) and Available Balance (funds withdrawable).

**Constraints**:
- Strictly handles state retrieval and validation ("Does this person have enough money?").
- Does not execute transactions directly; it updates state based on commands from the Transaction Engine.

## 3. Loan Service (The Credit Domain)
**Responsibility**: Loan Lifecycle Management (Origination, Disbursement, Repayment, and Recovery).

**Core Logic**:
- **Amortization Engine**: Generates schedules and calculates Principal vs. Interest splits using methods like Declining Balance or Straight Line.
- **Lifecycle States**: Manages transitions from Application -> Approved -> Disbursed -> Active -> Closed (or Written Off).
- **Delinquency Tracking**: Monitors missed payments and triggers penalty logic.

## 4. Ledger Service (The Source of Truth)
**Responsibility**: The Double-Entry General Ledger (GL).

**Core Logic**:
- **Immutability**: The most critical service where transactions are append-only. No updates or deletes allowed.
- **Double-Entry Principle**: Ensures that for every financial event, Total Debits = Total Credits.
- **Chart of Accounts**: Manages the hierarchy of asset, liability, equity, income, and expense accounts.

## 5. Transaction Engine (The Orchestrator)
**Responsibility**: Coordinating multi-service atomic operations.

**Core Logic**:
- Implements the **SAGA Pattern** to manage distributed transactions.
- **Example Workflow (Loan Disbursement)**:
    1.  **AccountService**: Create/Verify Loan Account.
    2.  **LedgerService**: Debit "Cash at Bank" (Asset) / Credit "Loan Receivable" (Asset).
    3.  **CustomerService**: Update "Active Loan" count and exposure limits.
    4.  **LoanService**: Update status to "Active".

## 6. Interest Accrual Engine
**Responsibility**: Automated interest calculation and capitalization.

**Core Logic**:
- **Daily Accrual**: Calculates interest daily based on the outstanding principal balance.
- **Posting**: Periodically posts accrued interest to the General Ledger and Customer Accounts (e.g., month-end).
- **Non-Performing Loans (NPL)**: Suspends accrual on loans marked as NPL to prevent inflating assets.

## 7. Audit Log Service
**Responsibility**: Security, Compliance, and Traceability.

**Core Logic**:
- **Immutable Trail**: Records *Who*, *What*, *When*, and *Why* for every API call and state change.
- **Metadata**: Captures IP addresses, device IDs, and previous/new values for data changes.
- **Retention**: Manages data retention policies required by financial regulations.

## 8. End of Day (EOD) Processing
**Responsibility**: System Reconciliation and Date Rollover.

**Core Logic**:
- **Reconciliation**: Verifies that the sum of all individual account balances matches the General Ledger control accounts.
- **Batch Jobs**: Triggers statement generation, interest posting, and fee assessments.
- **System Date Management**: Formally closes the current business day and opens the next, ensuring distinct accounting periods.
