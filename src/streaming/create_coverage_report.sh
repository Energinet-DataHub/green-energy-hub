# README
# ====================================================================
# Script for generating HTML report of code coverage of the streaming
# Python code base.
#
# Exit codes:
#     On threshold success: 0
#     On threshold failure: 1
#
# Requires the following modules that are installed in the Docker image
#
#     pip install coverage
#     pip install coverage-threshold
# ====================================================================


# Run pytest while collecting coverage data
# This requires:
# - An __init__.py file in tests/ folder
# - The accompanying .coveragerc file
# - --branch option is required in order to generate required branch data for coverage-threshold
coverage run --branch -m pytest tests/

# Create data for threshold evaluation
coverage json

# Create human reader friendly HTML report
coverage html

# Open HTML report
#echo "Opening created HTML report."
#echo "TIP: This index.html can be viewed as HTML by adding VS Code extension like e.g. 'HTML Preview'."
#code htmlcov/index.html

# Test if threshold requirements are met
coverage-threshold
