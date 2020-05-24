FROM squidfunk/mkdocs-material:5.2.1

COPY requirements-local.txt ./

RUN pip install -r requirements-local.txt