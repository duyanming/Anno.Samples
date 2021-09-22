set serviceName=AnnoWorkerService
set serviceFilePath=%cd%\AnnoWorkerService.exe
set serviceDescription=Anno Windows ·þÎñ AnnoWorkerService

sc create %serviceName%  BinPath=%serviceFilePath%
sc config %serviceName%    start=auto  
sc description %serviceName%  %serviceDescription%
sc start  %serviceName%
pause