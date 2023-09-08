# About the Project.

The project was built for a demo of flagger, and being used in dev for handling releases, when a service in dev might not have webhook ready, or configured to be used straight forward.

## Technology Stack of Flagger UI:

Backend Powered by .NET Core:
Flagger UI relies on the robust foundation of .NET Core for its backend infrastructure. .NET Core is a cross-platform, high-performance framework that enables the development of scalable and efficient applications. It's known for its speed, flexibility, and extensive library support, making it an ideal choice for building the backend of Flagger UI.

Sleek User Interface with Blazor:
Flagger UI's user interface is crafted using Blazor, a modern web framework developed by Microsoft. Blazor combines C# and HTML to create interactive and dynamic web applications directly in the browser. This means that users can enjoy a responsive and feature-rich UI experience without relying on heavy JavaScript libraries. Blazor's component-based architecture simplifies UI development and ensures a smooth user experience.

By combining the power of .NET Core on the backend with the flexibility and elegance of Blazor on the frontend, Flagger UI delivers a seamless and efficient platform for managing manual gating canary releases in Kubernetes. Users can expect a user-friendly interface backed by a robust backend that streamlines the entire canary release process.

## Working with Flagger UI

If trying to get this repo locally and use it or to build you own image.

See [Guide](readme.md) for Details on Flagger UI

## Building Docker Image
```sh

# build image
docker build -t <username>/flagger-ui:latest .

# tag image
docker tag cloudnxt/flagger-ui:latest <username>/flagger-ui:latest

# push image
docker push cloudnxt/flagger-ui:<tag>

# run image localy in docker.
docker run -p 3000:3000 cloudnxt/flagger-ui:latest

```


# Deploying resources to kubernetes (Locally)

```sh

# Create a namespace and Install Flagger in the cluster
kubectl create ns flagger-system

#Apply CRD of Flagger
kubectl apply -f https://raw.githubusercontent.com/fluxcd/flagger/main/artifacts/flagger/crd.yaml

helm repo update 

#Install or Upgrade Flagger
helm upgrade -i flagger flagger/flagger --namespace flagger-system --set prometheus.install=true --set meshProvider=kubernetes

#Install Flagger UI
helm install flagger-ui ./stable/flagger-ui

helm uninstall flagger-ui

# Add Deployents .
kubectl apply -f ./examples/k8/podinfo.yaml

# Apply Canary for Deployment
kubectl apply -f ./examples/k8/podinfo_canary.yaml

# Change Image for Flagger to kick in
kubectl set image deployment/podinfo podinfod=ghcr.io/stefanprodan/podinfo:6.0.2


# Add Deployents .
kubectl apply -f ./examples/k8/nginx.yaml

# Apply Canary for Deployment
kubectl apply -f ./examples/k8/nginx_canary.yaml

# Change Image for Flagger to kick in
kubectl set image deployment/nginx nginx=nginx:1.14.2


# Delete All Resources.

kubectl delete -f ./examples/k8/podinfo.yaml

kubectl delete -f ./examples/k8/podinfo_canary.yaml

kubectl delete -f ./examples/k8/nginx.yaml

kubectl delete -f ./examples/k8/nginx_canary.yaml
```