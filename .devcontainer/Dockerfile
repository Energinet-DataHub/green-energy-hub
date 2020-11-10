FROM jupyter/pyspark-notebook:45bfe5a474fa

# [Optional] Uncomment to install a different version of Python than the default
# RUN conda install -y python=3.5 \
#     && pip install --no-cache-dir pipx \
#     && pipx reinstall-all

RUN conda install --quiet --yes --satisfied-skip-solve \
    'rope=0.18.*' 'pytest=6.1.*' 'pylint=2.6.*' 'autopep8=1.5.*' 'configargparse=1.2.3' && \
    conda clean --all -f -y && \
    fix-permissions "${CONDA_DIR}" && \
    fix-permissions "/home/${NB_USER}"

USER root

RUN apt-get update
RUN apt-get install aspell aspell-en
RUN pip install pyspelling