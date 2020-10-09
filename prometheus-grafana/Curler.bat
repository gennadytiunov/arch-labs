
CD C:\Repositories\GitHub-ArchLabs\prometheus-grafana

:LOOP
 
ab.exe -n 5000 -c 100 http://arch.homework/products > nul

TIMEOUT 1 /nobreak > nul

ab.exe -n 5000 -c 1000 http://arch.homework/products/health > nul

TIMEOUT 1 /nobreak > nul

GOTO :LOOP


