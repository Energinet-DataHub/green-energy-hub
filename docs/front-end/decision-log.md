# Decision log

## 1. Hosting of front-ends (FEs), backend-for-frontends (BFFs) and Design System in Green Force ART

Date: 2021-09-20

Decision: **Option A** has been chosen as **option B** can be applied later on, if necessary  

Responsible: SSG, MEA, XALEH, XDZUS

### Option A - Use the same repository (monorepo) to host all front-ends (FEs), backend-for-frontends (BFFs) and the Design System

Pros

- A single code repository encourages team collaboration
- Easier code reuse across projects
- Single CI/CD pipeline
- Better code visibility across teams
- Mature tools to support the work
- Single version of Angular and other third party libraries across FEs and Design System
- Single version of DOT.NET across BFFs
- Consistent code style across projects
- No versioning necessary for Design System
- A feature can be implemented across the whole stack (vertically)
- Easier integration between layers in the stack

Cons

- CI/CD pipeline can grow in complexity
- Unclear boundaries for code ownership (shared code especially)
- Higher chance of introducing merge conflicts

### Option B - Use multiple repositories (multi-repo) to host front-ends (FEs), backend-for-frontends (BFFs) and the Design System

Pros

- Clear boundaries for code ownership
- Independent releases
- Simpler CI/CD pipeline
- Design System is published as a package that can be used outside of Energinet

Cons

- Own CI/CD pipelines means need for versioning and syncing across projects
- Versioning necessary for Design System
- Own Angular version for each FE
- Own DOT.NET version for each BFF
- Higher chance for code duplication
- More effort required for team collaboration
