FROM squidfunk/mkdocs-material:5.2.0

COPY requirements-local.txt ./

RUN pip install -r requirements-local.txt