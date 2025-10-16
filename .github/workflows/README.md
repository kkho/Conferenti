```mermaid
graph TD
    A[Push to main branch] --> B[Test Suite Workflow]
    A --> C[Playwright Tests Workflow]

    B -->|Success| D{Both workflows<br/>completed?}
    C -->|Success| D

    B -->|Failure| E[❌ Stop - Don't Deploy]
    C -->|Failure| E

    D -->|Yes, both passed| F[check-prerequisites job]
    D -->|No, waiting...| G[Wait for other workflow]

    F -->|Validation passed| H[deploy job]
    F -->|Validation failed| E

    H --> I[Build Docker Image]
    I --> J[Push to Registry]
    J --> K[Deploy to Kubernetes]

    style B fill:#90EE90
    style C fill:#90EE90
    style H fill:#87CEEB
    style E fill:#FFB6C1
    style K fill:#FFD700
```

# Workflow Dependency Flow

## Current Setup

```
┌─────────────────────────────────────────────────────────────┐
│                    Push to main branch                      │
└─────────────────┬───────────────────────────────────────────┘
                  │
         ┌────────┴────────┐
         │                 │
         ▼                 ▼
  ┌─────────────┐   ┌─────────────┐
  │ Test Suite  │   │ Playwright  │
  │  (test.yaml)│   │ Tests       │
  │             │   │ (e2e.yaml)  │
  └──────┬──────┘   └──────┬──────┘
         │                 │
         │                 │
    ✅ Success         ✅ Success
         │                 │
         └────────┬────────┘
                  │
                  ▼
         ┌────────────────────┐
         │  workflow_run      │◄─── Triggers when EITHER completes
         │  triggers          │
         └────────┬───────────┘
                  │
                  ▼
         ┌────────────────────┐
         │ check-prerequisites│
         │ job                │
         │                    │
         │ • Query GitHub API │
         │ • Verify BOTH      │
         │   workflows passed │
         │ • Set output flag  │
         └────────┬───────────┘
                  │
         all-passed=true?
                  │
                  ▼
         ┌────────────────────┐
         │   deploy job       │
         │                    │
         │ if: all-passed &&  │
         │     ref == main    │
         └────────┬───────────┘
                  │
                  ▼
         ┌────────────────────┐
         │ 🚀 Deploy to K8s   │
         └────────────────────┘
```
