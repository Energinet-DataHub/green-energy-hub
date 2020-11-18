FROM jupyter/pyspark-notebook:45bfe5a474fa

RUN conda install --quiet --yes --satisfied-skip-solve \
    'rope=0.18.*' 'pytest=6.1.*' 'pylint=2.6.*' 'autopep8=1.5.*' 'configargparse=1.2.3' 'applicationinsights=0.11.9' && \
    conda clean --all -f -y && \
    fix-permissions "${CONDA_DIR}" && \
    fix-permissions "/home/${NB_USER}"

COPY spark-defaults.conf /spark-defaults.conf

USER root

RUN sudo ln -s `(realpath /spark-defaults.conf)` $SPARK_HOME/conf/spark-defaults.conf

ENTRYPOINT ["/github/workspace/utilities/databricks-unit-test/entrypoint.sh"]
