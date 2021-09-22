set serviceName=AnnoWorkerService

sc stop   %serviceName% 
sc delete %serviceName% 

pause