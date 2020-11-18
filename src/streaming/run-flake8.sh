# These parameters should match those used in CI pipeline
# in order to be able to validate Python linting rules
# before pushing.
#
# Please update if you realize that CI pipeline configuration
# has changed.
flake8 --ignore=E501,F401,E402,W503 --max-line-length=200 .