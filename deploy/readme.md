
# Deploy flagger UI

```sh
cd /deploy

kubectl create ns flagger-system

kubectl apply -f https://raw.githubusercontent.com/fluxcd/flagger/main/artifacts/flagger/crd.yaml

helm repo update 

helm upgrade -i flagger flagger/flagger --namespace flagger-system --set prometheus.install=true --set meshProvider=kubernetes


helm install flagger-ui ./deploy/chart

helm uninstall flagger-ui



kubectl apply -f ingress_app.yaml

kubectl apply -f ./examples/k8/podinfo.yaml

kubectl apply -f ./examples/k8/podinfo_canary.yaml

kubectl set image deployment/podinfo podinfod=ghcr.io/stefanprodan/podinfo:6.0.2



kubectl apply -f ./examples/k8/nginx.yaml

kubectl apply -f ./examples/k8/nginx_canary.yaml

kubectl set image deployment/nginx nginx=nginx:1.14.2




kubectl delete -f ./examples/k8/podinfo.yaml

kubectl delete -f ./examples/k8/podinfo_canary.yaml

kubectl delete -f ./examples/k8/nginx.yaml

kubectl delete -f ./examples/k8/nginx_canary.yaml
```