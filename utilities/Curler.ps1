while($true){
  curl -Uri http://arch.homework/otusapp/health | Select-Object -ExpandProperty Content
  Start-Sleep -s 1
}