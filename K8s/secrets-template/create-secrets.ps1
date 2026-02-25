kubectl create namespace messageapp
kubectl apply -f mssql-secret.yaml -n messageapp
kubectl create secret generic jwt-settings --from-file=jwt-settings.json -n messageapp
kubectl create secret generic db-settings --from-file=db-settings.json -n messageapp