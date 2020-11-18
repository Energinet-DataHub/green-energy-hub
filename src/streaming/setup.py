from setuptools import setup, find_packages

__version__ = ""
with open('VERSION') as version_file:
    __version__ = version_file.read().strip()

setup(name='geh_stream',
      version=__version__,
      description='Tools for streaming and aggregation of meter data of Green Energy Hub',
      long_description="",
      long_description_content_type='text/markdown',
      license='MIT',
      packages=find_packages()
      )
