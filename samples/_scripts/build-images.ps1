TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}
Write-Host "${NAMESPACE} and ${TAG}"

Write-Host "Build TODO API..."
docker build -f samples/TodoApi/Dockerfile -t vndg/todoapi:$(git rev-parse --short HEAD) -t vndg/todoapi:latest .

Write-Host "Build SignalR..."
docker build -f samples/SignalRNotifier/Dockerfile -t vndg/signalrnotifier:$(git rev-parse --short HEAD) -t vndg/signalrnotifier:latest .

Write-Host "Build Web Notifier..."
docker build -f samples/WebNotifier/Dockerfile -t vndg/webnotifier:$(git rev-parse --short HEAD) -t vndg/webnotifier:latest .
