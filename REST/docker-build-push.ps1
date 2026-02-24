# Execute this script from same folder
docker build -f .\Dockerfile -t jmstoledo/rest:v2 ..
docker push jmstoledo/rest:v2