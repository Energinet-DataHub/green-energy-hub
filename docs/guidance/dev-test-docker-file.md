# Build you Databricks Dev and Test environment with Docker

Databricks jobs or workbooks development requires a special tool set to be installed. To simplify development you can use online editor in Databricks itself.
However to have rich capabilities around debug and traces, as well as to build and run unit and integration tests as part of your CI\CD, you may want to prepare a local dev environment.
As a setup of spark and other tools, required to build and debug jobs, is quite complex and introduces a lot of dependencies required on local machine OS, a good approach would be to build a docker image, that you can use in [VS Code Remote](https://code.visualstudio.com/docs/remote/containers) or in your custom [GitHub Container Action](https://docs.github.com/en/free-pro-team@latest/actions/creating-actions/creating-a-docker-container-action).
There are two important rules you need to follow in order to achieve a smooth dev and test process:

1. Use same container definition for VS Code Remote and GitHub Docker Container Action

2. Docker container should be pre built, so it will take less time to run it in GitHub

As of now, Green Energy Hub repo contains three almost equal container definitions: [Dev Container Dockerfile](../../.devcontainer/Dockerfile), [Unit Test Dockerfile](../../.github/actions/databricks-unit-test/Dockerfile) and [Integration Test Dockerfile](../../.github/actions/databricks-integration-test/Dockerfile).

These Dockerfiles are quite complex and that is the reason, why spinning up the containers in Unit Tests and Integration Tests actions can take tens of minutes.

This is where optimization can be done. You can pre-build a base Spark image and publish it to Docker hub and then reference this base image in the Dockerfiles within the solution.

## Steps to publish your Docker Image

1. Create an account in [Docker Hub](https://hub.docker.com/) named e.g. "greenenergyhub".

2. Install Docker on your machine. Use [this](https://docs.docker.com/engine/install/) link for reference.

3. [Login](https://docs.docker.com/engine/reference/commandline/login/) to your docker account using docker CLI.

4. At this moment we will start building and publishing of our Docker image.
Let's pick Integration Tests image as base. From your local machine terminal navigate to [.github/actions/databricks-integration-test](../../.github/actions/databricks-integration-test) and copy it to a new folder, where a basic container definition will be stored (for example: .github/base-container).
Navigate to this new folder from command line terminal.

5. Run [build](https://docs.docker.com/engine/reference/commandline/build/) command:

    ```sh
    # in this case: 'greenenergyhub' is your docker account you created at step 1, 'sparkbase' is a name of your container, 'latest' is a tag (you should specify version here to be able to refer to concrete image version in other containers).
    docker build -t greenenergyhub/sparkbase:latest .
    ```

6. Run [push](https://docs.docker.com/engine/reference/commandline/push/) command to publish your image to the Docker Hub repo

    ```sh
    docker push greenenergyhub/sparkbase:latest
    ```

7. Now the Docker image is published in Docker Hub. We can reference it as a base image for other containers. So the [Dev Container Dockerfile](../../.devcontainer/Dockerfile) should be modified to (yes, only one line in definition):

    ```docker
    FROM greenenergyhub/sparkbase:latest
    ```

8. [Unit Test Dockerfile](../../.github/actions/databricks-unit-test/Dockerfile) should be:

    ```docker
    FROM greenenergyhub/sparkbase:latest

    ENTRYPOINT ["/github/workspace/.github/actions/databricks-unit-test/entrypoint.sh"]
    ```

9. [Integration Test Dockerfile](../../.github/actions/databricks-integration-test/Dockerfile) should be:

    ```docker
    FROM greenenergyhub/sparkbase:latest

    ENTRYPOINT ["/github/workspace/.github/actions/databricks-integration-test/entrypoint.sh"]
    ```

10. Now you can manage container definition centrally for Dev and Test environments. You just need to push a newly created base image and change Dockerfiles in the repo to reference new image version.

11. Define a Docker image build and publish workflows to be able to automate changing of a base image. Read [this](https://docs.github.com/en/free-pro-team@latest/actions/guides/publishing-docker-images) for reference.
