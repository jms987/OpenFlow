# OpenFlow

**Distributed ML Experiment Manager**

**Status:** Active — deployment in progress (Jul 2025 – Present)

**Last updated:** October 5, 2025

---

## Project overview

OpenFlow is a fault-tolerant, scalable platform to orchestrate and monitor machine-learning experiments across multiple edge and cloud devices. It was built to simplify multi-device training, enable reproducible research, and accelerate debugging by centralizing logs, metrics, and artifacts.

Key goals:

- Orchestrate synchronized experiments across heterogeneous devices (edge and cloud).
- Provide resource-aware scheduling that adapts to differing device capabilities.
- Support checkpoint / resume semantics so long-running, interrupted experiments can continue without losing progress.
- Centralize experiment artifacts, logs, and metrics with strong versioning to support reproducibility.
- Offer a compact, real-time dashboard for experiment observability.


## Tech stack

- **Backend / API:** ASP.NET Core (.NET)
- **Training clients:** Python (PyTorch)
- **Database:** Azure Cosmos DB
- **Artifact storage:** Azure Blob Storage
- **Packaging / deployment:** Docker, GitHub Actions
- **CI/CD:** GitHub Actions


## Planned Features and Enhancements

> This section summarizes the functionality implemented so far and what you can use immediately.

- **RESTful API and client** A complete REST API and a client library to manage the experiment lifecycle (start / stop / schedule / checkpoint / status / logs). The API exposes endpoints that let you programmatically control experiments and query system state.

- **Parallel training & resource-aware scheduling:** Concepts from the author’s master’s thesis have been extended to support parallel training across heterogeneous devices. A resource-aware scheduler can place jobs taking into account device capabilities and current utilization (prototype / production-ready depending on deployment profile).

- **Checkpoint & resume:** Checkpointing mechanisms are implemented so experiments can be paused and resumed. Checkpoints are stored as versioned artifacts in Azure Blob Storage and indexed in Cosmos DB for quick lookup.

- **Lightweight real-time dashboard:** A compact UI provides live logs and metrics streaming and a quick view of active experiments, devices, and artifact versions. The dashboard is optimized to be lightweight so it can be hosted alongside the backend or served independently.

- **Artifact storage & versioning:** Integration with Azure Blob Storage to store models, logs, and other artifacts. Versioning and metadata are tracked in Cosmos DB to enable reproducible runs and easy retrieval.

- **Containerization & CI/CD:** Docker images for the backend and client components and GitHub Actions workflows are in place to build and push images and run tests/linters. This enables reproducible, containerized deployments.

- **Fault-tolerance primitives:** Basic mechanisms are implemented to recover from failed client nodes (restarts, checkpoint-based resume). Work is ongoing to harden resilience in high-churn environments.

- **Documentation & examples:** Foundational documentation, basic usage examples, and a minimal quickstart guide are included in the repository. More comprehensive tutorials and sample experiments are in progress.


## Architecture (high level)

```
+-------------+       +----------------+       +--------------------+
| Edge Device | <-->  | Experiment API | <-->  | Azure Cosmos DB     |
| (Python)    |       | (ASP.NET Core) |       | (Metadata / state)  |
+-------------+       +----------------+       +--------------------+
       |                     |                         |
       v                     |                         v
  Checkpoint              Dashboard                 Azure Blob Storage
  / Logs (Blob)            (WebSocket/REST)          (Artifacts / models)
```

- **Control plane:** ASP.NET Core REST API that orchestrates experiments and handles scheduling decisions.
- **Data plane:** Python training clients that run the actual ML workloads (PyTorch), stream logs and metrics, and upload checkpoints/artifacts.
- **Persistence:** Cosmos DB stores experiment metadata, device state and run history; Blob Storage stores heavy artifacts and checkpoint binaries.
- **Observability:** Dashboard + logs + basic metric streams for real-time feedback.

## Deployment notes

- **Cloud:** The system is designed to run on Azure using Cosmos DB and Blob Storage as first-class integrations. Typical production deployment options: Azure App Service, Azure Container Instances or Azure Kubernetes Service (AKS).

- **Containers & registry:** Dockerfiles are included for backend and client. Use your preferred container registry (Docker Hub, Azure Container Registry) and build/push images via the included GitHub Actions workflows.

- **CI/CD:** GitHub Actions workflows build and test the code, and (optionally) push images to a container registry. Secrets for cloud credentials should be stored in GitHub Secrets or your environment manager.

- **Configuration:** Use environment variables for all connection strings and secrets (e.g., `AZURE_COSMOS_CONNECTION_STRING`, `AZURE_BLOB_CONNECTION_STRING`). Consider using a secrets manager for production.


## Security & privacy (current)

- Basic authentication/authorization is available for API access. Production deployments should integrate stronger identity and access management (e.g., Azure AD, OAuth2) and move secrets to a managed secrets store.
- Network-level protections (VNETs, private endpoints, firewall rules) are recommended for Azure-hosted deployments.


## Roadmap & future development

The project is functional and useful today for many distributed experiment workflows, but several important improvements are planned to increase reliability, usability, and scale. The roadmap below represents planned enhancements — prioritize and break these into issues/pull requests as needed.

**Short term (next 1–3 months)**

- Harden the scheduler for production workloads: improved device reliability signalling, smarter task preemption, and eviction policies.
- Add comprehensive e2e tests and integration tests for common failure modes (network partition, client churn).
- Improve documentation: step-by-step deployment guides for Azure (App Service, ACR, AKS) and a complete quickstart with examples for PyTorch distributed jobs.
- Add TLS everywhere by default (backend and dashboard) and provide automated cert management examples.

**Mid term (3–9 months)**

- Autoscaling & multi-tenant support: allow experiments to elastically scale across cloud/edge resources while isolating tenant data/artifacts.
- Advanced resource-awareness: incorporate GPU/TPU metrics, bandwidth-awareness, energy-aware scheduling for constrained edge devices.
- Web UI enhancements: richer visualization of training curves, comparative experiment views, artifact diffing and model-preview support.
- Role-based access control (RBAC) and audit logging for enterprise use.

**Long term (9–18+ months)**

- Full orchestration integration with Kubernetes (native operators) for managing large clusters and resource pools.
- Cross-framework support: first-class adapters for TensorFlow, JAX and model-serving plugins for online evaluation.
- Formal resilience guarantees: persistent leader election, quorum/consensus for critical control plane decisions (if required by use cases).
- Plug-in ecosystem: allow community plugins for custom schedulers, storage backends, and monitoring/alerting integrations.


## How to contribute

Contributions are welcome! Suggested ways to help:

1. Open issues for bugs or feature requests and tag them with an appropriate label (e.g., `bug`, `enhancement`).
2. Pick an open issue and submit a PR. Prefer small, focused changes with tests and documentation updates.
3. Help expand the examples and add real-world experiment recipes for common ML tasks (image classification, NLP fine-tuning, RL training).

Please follow the repository coding style and add tests where appropriate. CI will run on PRs to validate builds and basic test suites.


## Troubleshooting & support

- Check logs streamed to the dashboard for live debugging.
- Ensure environment variables for Azure credentials are set and that the configured Cosmos DB and Blob Storage accounts are reachable.
- For checkpoint/restore issues, verify that blob uploads completed and that metadata in Cosmos DB matches expected checkpoint versions.


## Acknowledgements

Inspired by work from the author’s master’s thesis and shaped by real-world needs for reproducible, multi-device ML experimentation. Special thanks to early testers and contributors.
