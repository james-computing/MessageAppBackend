# Execute this script from same folder
docker build -f .\Dockerfile -t jmstoledo/kafka-consumer:v2 ..
docker push jmstoledo/kafka-consumer:v2