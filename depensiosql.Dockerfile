FROM postgres:15

WORKDIR /app

COPY migrations/migrations.sql /migrations/migrations.sql

# Variables d'environnement par défaut (peut être remplacé par docker-compose)
ENV DB_HOST=db \
    DB_USER=pa \
    DB_PASSWORD=as \
    DB_NAME=d

CMD ["sh", "-c", "psql -h $DB_HOST -U $DB_USER -d $DB_NAME -f migrations.sql"]
