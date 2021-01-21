# README
# ====================================================================
# Script for generating HTML report of code coverage of the streaming
# Python code base.
#
# Install coverage by executing:
#
#     pip install coverage
#
# Not sure, but if necessary also install this module:
#
#     pip install pytest-cov
# ====================================================================


# Run pytest while collecting coverage data
# This requires:
# - An __init__.py file in tests/ folder
# - The accompanying .coveragerc file
coverage run -m pytest

# Create HTML report
coverage html

# Open report
echo "Opening created HTML report."
echo "TIP: This index.html can be viewed as HTML by adding VS Code extension like e.g. 'HTML Preview'."
code htmlcov/index.html
