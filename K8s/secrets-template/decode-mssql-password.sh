#!/bin/bash

kubectl get secret mssql -o jsonpath='{.data.MSSQL_SA_PASSWORD}' | base64 --decode