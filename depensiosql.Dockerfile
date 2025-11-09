FROM postgres:15

WORKDIR /app

COPY migrations.sql .

CMD ["sh", "-c", "psql -h $DB_HOST -U $DB_USER -d $DB_NAME -f migrations.sql"]
