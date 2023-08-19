
# Deploy flagger UI

```sh
cd /deploy

kubectl create ns flagger-system

kubectl apply -f https://raw.githubusercontent.com/fluxcd/flagger/main/artifacts/flagger/crd.yaml


helm upgrade -i flagger flagger/flagger --namespace flagger-system --set prometheus.install=true --set meshProvider=kubernetes

helm install admission-webhook ./chart

kubectl apply -f ingress_app.yaml

kubectl apply -f podinfo.yaml

kubectl apply -f podinfo_canary.yaml

kubectl set image deployment/podinfo podinfod=ghcr.io/stefanprodan/podinfo:6.0.2

kubectl apply -f nginx.yaml

kubectl apply -f nginx_canary.yaml

kubectl set image deployment/nginx nginx=nginx:1.14.2

kubectl delete -f podinfo.yaml

kubectl delete -f podinfo_canary.yaml

kubectl delete -f nginx.yaml

kubectl delete -f nginx_canary.yaml
```