FROM jupyter/scipy-notebook

SHELL ["/bin/bash", "-o", "pipefail", "-c"]

USER root

ARG openjdk_version="8"

ARG hadoop_version="3.3.0"
ARG hadoop_checksum="9ac5a5a8d29de4d2edfb5e554c178b04863375c5644d6fea1f6464ab4a7e22a50a6c43253ea348edbd114fc534dcde5bdd2826007e24b2a6b0ce0d704c5b4f5b"

ARG spark_version="3.0.1"
ARG spark_checksum="07FC6DF224F303EF53189E6082004400D51CD6C2244D1851D90EABBE58404A69FF980BFFE147A5259A34190E1E1C974C72D02470D10A783D2D87F43A8DA0850B"

ARG py4j_version="0.10.9"

ENV APACHE_SPARK_VERSION="${spark_version}" \
    HADOOP_VERSION="${hadoop_version}"

RUN apt-get -y update && \
    apt-get install --no-install-recommends -y \
    "openjdk-${openjdk_version}-jre-headless" \
    "openssh-client" "git" "bash-completion" \
    "aspell" "aspell-en" \
    "flake8" \
    "ca-certificates-java" && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

ENV JAVA_HOME=/usr/lib/jvm/java-${openjdk_version}-openjdk-amd64 \
    PATH=$JAVA_HOME/bin:$PATH

# Spark and Hadoop installation
WORKDIR /tmp

RUN wget -q $(wget -qO- https://www.apache.org/dyn/closer.cgi/hadoop/common/hadoop-${HADOOP_VERSION}/hadoop-${HADOOP_VERSION}.tar.gz\?as_json | \
    python -c "import sys, json; content=json.load(sys.stdin); print(content['preferred']+content['path_info'])") && \
    echo "${hadoop_checksum} *hadoop-${HADOOP_VERSION}.tar.gz" | sha512sum -c - && \
    tar xzf "hadoop-${HADOOP_VERSION}.tar.gz" -C /usr/local --owner root --group root --no-same-owner && \
    rm "hadoop-${HADOOP_VERSION}.tar.gz"

RUN wget -q $(wget -qO- https://www.apache.org/dyn/closer.lua/spark/spark-${APACHE_SPARK_VERSION}/spark-${APACHE_SPARK_VERSION}-bin-without-hadoop.tgz\?as_json | \
    python -c "import sys, json; content=json.load(sys.stdin); print(content['preferred']+content['path_info'])") && \
    echo "${spark_checksum} *spark-${APACHE_SPARK_VERSION}-bin-without-hadoop.tgz" | sha512sum -c - && \
    tar xzf "spark-${APACHE_SPARK_VERSION}-bin-without-hadoop.tgz" -C /usr/local --owner root --group root --no-same-owner && \
    rm "spark-${APACHE_SPARK_VERSION}-bin-without-hadoop.tgz"

WORKDIR /usr/local
RUN ln -s "spark-${APACHE_SPARK_VERSION}-bin-without-hadoop" spark && \
    ln -s "hadoop-${HADOOP_VERSION}" hadoop

# Configure Spark
ENV SPARK_HOME=/usr/local/spark \
    HADOOP_HOME=/usr/local/hadoop

ENV PATH=$SPARK_HOME/bin:$HADOOP_HOME/bin:$PATH \
    PYTHONPATH="${SPARK_HOME}/python:${SPARK_HOME}/python/lib/py4j-${py4j_version}-src.zip" \
    SPARK_OPTS="--driver-java-options=-Xms1024M --driver-java-options=-Xmx4096M --driver-java-options=-Dlog4j.logLevel=info"

USER $NB_UID

ENV PATH=$JAVA_HOME/bin:$SPARK_HOME/bin:$HADOOP_HOME/bin:$PATH
RUN echo "export SPARK_DIST_CLASSPATH=$(hadoop classpath)" >> $HOME/.bashrc

WORKDIR $HOME

# [Optional] Uncomment to install a different version of Python than the default
# RUN conda install -y python=3.5 \
#     && pip install --no-cache-dir pipx \
#     && pipx reinstall-all

RUN conda install --quiet --yes --satisfied-skip-solve \
    'pyarrow=2.0.*' 'rope=0.18.*' 'pytest=6.1.*' 'pylint=2.6.*' 'autopep8=1.5.*' 'configargparse=1.2.3' 'applicationinsights=0.11.9' \
    'coverage=5.3.*' 'coverage-threshold=0.4.*' \
    && \
    pip --no-cache-dir install pyspelling azure-eventhub && \
    conda clean --all -f -y && \
    fix-permissions "${CONDA_DIR}" && \
    fix-permissions "/home/${NB_USER}"