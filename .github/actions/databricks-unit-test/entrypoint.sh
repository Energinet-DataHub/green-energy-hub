#!/bin/sh -l

cd ./src/streaming
python setup.py install
# python coverage-threshold install
pip install coverage-threshold
coverage run --branch -m pytest tests/
# Create data for threshold evaluation
coverage json
# Create human reader friendly HTML report
coverage html
coverage-threshold