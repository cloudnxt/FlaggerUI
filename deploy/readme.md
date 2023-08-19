helm install flagger-ui ./chart

helm upgrade -i flagger flagger/flagger --namespace flagger --set prometheus.install=true --set meshProvider=kubernetes