Write-Host "Delete from TODO API..."
kubectl delete -f samples/TodoApi/k8s/

Write-Host "Delete from SignalR..."
kubectl delete -f samples/SignalRNotifier/k8s/

Write-Host "Delete from Web Notifier..."
kubectl delete -f samples/WebNotifier/k8s/
