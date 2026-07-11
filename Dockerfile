FROM python:3.11-slim

WORKDIR /app

COPY Panel/requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY Panel/ .
COPY Main/ /app/Main/

EXPOSE 5000

CMD gunicorn app:app --bind 0.0.0.0:${PORT:-5000}
