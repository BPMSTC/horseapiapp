# SQL Server in a container for local dev
# Build context can be your solution root or a subfolder; this file does NOT build your API.

FROM mcr.microsoft.com/mssql/server:2022-latest

# Accept EULA and set a strong SA password
ENV ACCEPT_EULA=Y \
    MSSQL_SA_PASSWORD=Your_strong_password123!

# Expose the SQL Server port
EXPOSE 1433

# Persist data between container restarts
VOLUME ["/var/opt/mssql"]

# Optional healthcheck to ensure the DB is responsive
# Uses sqlcmd from the sqlcmd v18 tooling bundled in the image
HEALTHCHECK --interval=10s --timeout=3s --retries=10 \
  CMD /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -C || exit 1

# Use the default entrypoint provided by the image
# ENTRYPOINT ["/opt/mssql/bin/sqlservr"]
