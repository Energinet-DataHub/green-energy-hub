#!/bin/sh -l

cd ./src/streaming
python setup.py install
pytest tests
