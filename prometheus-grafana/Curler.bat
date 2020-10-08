
CD c:\Work\OTUS\Labs\Helm\

:LOOP
 
ab.exe -n 100 -c 10 http://arch.homework/products > nul

TIMEOUT 1 /nobreak > null

ab.exe -n 100 -c 10 http://arch.homework/products/health > nul

TIMEOUT 1 /nobreak > null

GOTO :LOOP


