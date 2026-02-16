# Execute this script from same folder
docker build -f .\Dockerfile -t jmstoledo/message-real-time-service:v1 ..
docker push jmstoledo/message-real-time-service:v1