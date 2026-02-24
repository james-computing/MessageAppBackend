# Execute this script from same folder
docker build -f .\Dockerfile -t jmstoledo/auth:v2 ..
docker push jmstoledo/auth:v2