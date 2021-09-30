# Test principals

**“Main is always production graded”** this sentence is the essence of our principals. 
The thought here is that any code merged into a main branch **must** be **thoroughly tested** so the team feel it’s production ready and have **confidence** in the code.

**Automated test** is always preferred over manual tests, as they provide **fast feedback**, which helps more quickly isolate issues with the software.

**All** automated tests must **succeed** before change are **merged** into the main branch, preferably in a PR to document the results. 

**Any manual tests** that the business or team regards as **necessary, must** be automated **as soon as possible** to ensure quality and fast feedback. Any manual tests are considered **technical debt** and must be resolved.

Small independent tests are **preferred** over full system tests, as to ensure **quick feedback** to the developers. We test our **data contracts and behavior** at a domain level as to ensure that our implementation fulfills our **business requirements** and **domain integrations**.

The **team** is **responsible** to use the **necessary** combination of test-types that they feel is **sufficiently** to ensure the **quality** of the software so that it is **production graded**.

**Common language:**
-	Unit test: Testing individual methods or classes within a component.
-	Component test: Tests of a single component within a domain.
-	Integration test: Test integration between multiple components (Azure Function, running process, database, etc.) within a single domain.
-	Domain test:  Blackbox test of a single domain.
-	System test: Automated test of the whole system (will not be done currently) 
