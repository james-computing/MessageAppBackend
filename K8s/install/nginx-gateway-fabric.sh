#!/bin/bash

# This script installs NGINX Gateway Fabric.
# Helm is needed for NGINX Gateway Fabric.
# Install Helm before executing this script.
# Helm can be installed with the helm.sh script.

# From https://docs.nginx.com/nginx-gateway-fabric/install/secure-certificates/
# Install Gateway API CRDs
kubectl kustomize "https://github.com/nginx/nginx-gateway-fabric/config/crd/gateway-api/standard?ref=v2.4.0" | kubectl apply -f -

# Install cert-manager
helm repo add jetstack https://charts.jetstack.io
helm repo update

helm install \
  cert-manager jetstack/cert-manager \
  --namespace cert-manager \
  --create-namespace \
  --set config.apiVersion="controller.config.cert-manager.io/v1alpha1" \
  --set config.kind="ControllerConfiguration" \
  --set config.enableGatewayAPI=true \
  --set crds.enabled=true

# Create the CA issuer.
# This example uses a self-signed Issuer,
# which should not be used in production environments.
# For production environments, you should use a real CA issuer.
kubectl create namespace nginx-gateway

kubectl apply -f ./yaml/selfsigned-issuer.yaml

# Create server and client certificates

kubectl apply -f ./yaml/nginx-gateway-certificate.yaml

kubectl apply -f ./yaml/nginx-certificate.yaml

# Confirm the Secrets have been created

kubectl -n nginx-gateway get secrets

# From https://docs.nginx.com/nginx-gateway-fabric/install/helm/
# Install NGINX Gateway Fabric with Helm

# Installing the Gateway API resources
kubectl kustomize "https://github.com/nginx/nginx-gateway-fabric/config/crd/gateway-api/standard?ref=v2.4.0" | kubectl apply -f -

# Install from the OCI registry
helm install ngf oci://ghcr.io/nginx/charts/nginx-gateway-fabric --create-namespace -n nginx-gateway