REM ========================================================
REM generate a certificate
REM and 
REM generate an .env-file
REM 
REM .env file has thumbprints that are read by docker-compose
REM ========================================================
dotnet dev-certs https --clean
dotnet dev-certs https -ep .\certificate\aspnetapp.pfx -p  password
dotnet dev-certs https --trust
dotnet dev-certs https -c
dotnet dev-certs https -c | findstr "CN" > tmp.a
@echo off
FOR /F "tokens=6" %%G IN (tmp.a) DO (
   @echo LEAF=%%G >  .env
   @echo INTERMEDIATE=%%G >> .env
)
erase tmp.a