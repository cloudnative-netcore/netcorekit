Write-Host "Apply for TODO API..."
kubectl apply -f samples/TodoApi/k8s/

Write-Host "Apply for SignalR..."
kubectl apply -f samples/SignalRNotifier/k8s/

Write-Host "Apply for Web Notifier..."
kubectl apply -f samples/WebNotifier/k8s/
