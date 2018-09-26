TAG=${TAG:=$(git rev-parse --short HEAD)}
NAMESPACE=${NAMESPACE:="vndg"}
echo "${NAMESPACE} and ${TAG}"

echo "Build TODO API..."
docker build -f samples/TodoApi/Dockerfile -t vndg/todoapi:$(git rev-parse --short HEAD) -t vndg/todoapi:latest .

echo "Build SignalR..."
docker build -f samples/SignalRNotifier/Dockerfile -t vndg/signalrnotifier:$(git rev-parse --short HEAD) -t vndg/signalrnotifier:latest .