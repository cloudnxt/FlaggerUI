# Flagger UI
<p align="center" style="width:200px;">
  <img src="https://github.com/cloudnxt/FlaggerUI/blob/main/docs/images/logo.png" alt="Flagger UI"/>
</p>


The Flagger UI App is a webhook-enabled application designed to provide manual control over deployment gates in a continuous integration and deployment (CI/CD) pipeline. 

It allows users to manually review and approve or reject deployments at specific stages of the pipeline.

![image](https://github.com/cloudnxt/FlaggerUI/blob/main/docs/images/apps.png)
---


## Prerequisites

- **Flagger** - should be installed and running along with its CRD in the cluster, version 1.26 and above.
- **Kubernetes** - version 1.24

---

## Version Info

### Latest 0.0.1.beta

- Idea Showcase
- UI to register App automatically using annotations.
- Gates are automatically configured for the registered app with `close` being the default status.
- Shows Load test done by the app.
- Able to handle rollback hook.


---

## Features

- **Plug and play**: Seamlessly integrates with your existing app setting with just an annotation to the deployment. The annotation will add the app tot the dashboard automatically. 


- **Webhook Integration**: Seamlessly integrates with your existing CI/CD pipeline via webhooks, following the same webhook specifications described in the [Flagger documentation](https://docs.flagger.app/usage/webhooks). This ensures easy compatibility and interoperability with your Flagger-based deployment automation.

- **Manual Approval**: Enables manual review and approval or rejection of deployments triggered by Flagger. You can configure Flagger to send webhook events to the Manual Gates App, which will pause the deployment process until manual approval is received.

- **Customizable Gates**: Define custom gates based on your specific deployment stages and criteria. Each gate can be configured with unique rules and requirements, allowing you to tailor the approval process to match your organization's policies and procedures. (TODO)

- **Intuitive Dashboard**: Provides a user-friendly dashboard to manage and monitor the deployment gates. Users can easily view pending deployments, review details, and take appropriate actions. The dashboard is designed to align with the Flagger workflow and enhance the overall user experience. (TODO)

- **Event Trail**: Keeps you updated with all the flagger Webhooks Events, taking place in the flagger controll plane, for the respective deployment.


## Flow
![image](https://github.com/cloudnxt/FlaggerUI/blob/main/docs/asdasd.png)
---
## Getting Started

To use the Manual Gates App in conjunction with Flagger in your CI/CD pipeline, follow these steps: 

**Installation**

Clone the repository and configure the required environment variables as specified in the project documentation. OR You could you the docker image avilalbe here : `rohitrustagi007/canary-gates:1.0.7`. Flagger UI can currently be installed using helm charts. This can be done either getting the repo locally or from the helm repo


```sh
helm install flagger-ui flaggerui
```

**Configuration** 

Customize the deployment gates based on your requirements, specifying the necessary rules, you can register the app, directly using api or you can use the below annotations.

Enable your app to be visible by flagger UI. To make you deployment visible to flagger ui, add the annotation to your deployment.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: podinfo
  labels:
    app: podinfo
  annotations:
    enable-canary-gates: "true" # add this line to your deployment.
spec:
  ...
```


Once the above deployment is applied to the cluster, the deployment will be visible in the flagger-ui app, along with its gates configured withthe default status `close`

The gate will look like below.

![image](https://github.com/cloudnxt/FlaggerUI/blob/main/docs/images/confirmrollout.png)




The thumbs up sign shows all good, the waiting icon will change from thumbs up to `radioactive` to show the webhook is currently waiting.



**Webhook Integration**

Follow the instructions provided in the [Flagger documentation](https://docs.flagger.app/usage/webhooks) to configure Flagger to send webhook events to the Manual Gates App. 
This integration enables the app to receive deployment events and pause the deployment process for manual approval.

 Forward the port to to expose the flagger ui locally.


```sh
kubectl port-forward svc/flagger-ui 80:5443
```
Configuring the CRD Canary of flagger.

```sh
apiVersion: flagger.app/v1beta1
kind: Canary
metadata:
  name: podinfo
  namespace: test
spec:
  # service mesh provider can be: kubernetes, istio, appmesh, nginx, gloo
  provider: kubernetes
  # deployment reference
  targetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: podinfo
  # the maximum time in seconds for the canary deployment
  # to make progress before rollback (default 600s)
  progressDeadlineSeconds: 60
  # HPA reference (optional)
  autoscalerRef:
    apiVersion: autoscaling/v2beta2
    kind: HorizontalPodAutoscaler
    name: podinfo
  service:
    port: 9898
    portDiscovery: true
  analysis:
    # schedule interval (default 60s)
    interval: 30s
    # max number of failed checks before rollback
    threshold: 2
    # number of checks to run before rollback
    iterations: 5
    # Prometheus checks based on 
    # http_request_duration_seconds histogram
    metrics:
      - name: request-success-rate
        thresholdRange:
          min: 99
        interval: 1m
      - name: request-duration
        thresholdRange:
          max: 500
        interval: 30s
    # acceptance/load testing hooks
    webhooks:
      - name: "start gate"
        type: confirm-rollout
        url: http://flagger-ui.default/confirm-rollout/gate/check
        metadata:
          type: webhookType
          cmd: "confirm-rollout"
      - name: "load test"
        type: rollout
        url: http://flagger-ui.default/confirm-rollout/gate/check
        timeout: 15s
        metadata:
          cmd: "hey -z 1m -q 5 -c 2 http://podinfo-canary.test:9898/"
      - name: "traffic increase gate"
        type: confirm-traffic-increase
        url: http://flagger-ui.default/gate/check
      - name: "promotion gate"
        type: confirm-promotion
        url: http://flagger-ui.default/confirm-promotion/gate/check
      - name: smoke-test
        type: pre-rollout
        url: http://flagger-ui.default/pre-rollout/gate/check
        timeout: 120s
        metadata:
          type: bash
          cmd: "curl -sd 'anon' http://podinfo-canary.test:9898/token | grep token"
      - name: load-test
        url: http://flagger-ui.default/confirm-rollout/gate/check
        timeout: 5s
        metadata:
          type: cmd
          cmd: "hey -z 1m -q 10 -c 2 http://podinfo-canary.test:9898/"

```

**Usage**

Start your CI/CD pipeline with Flagger. When a gate is reached, Flagger will trigger a webhook event, and the Manual Gates App will receive it, pausing the deployment. Relevant users will be notified to review and approve or reject the deployment.

**Monitoring**

Utilize the provided dashboard to monitor and manage the deployments at each gate. Track approvals, rejections, and comments for auditing and compliance purposes. The dashboard provides visibility into the status of ongoing deployments and facilitates seamless collaboration among team members.

---
## Development Guides
If trying to get this repo locally and use it or to build you own image.

```sh

# build image
docker build -t <username>/flagger-ui:latest .

# tag image
docker tag <username>/canary-gates:latest <username>/flagger-ui:latest

# push image
docker push rohitrustagi007/canary-gates:<tag>

# run image localy in docker.
docker run -p 3000:3000 rohitrustagi007/flagger-ui:latest

```


## Contributions

Contributions to the Manual Gates App are welcome! If you encounter any issues, have suggestions, or would like to contribute new features or improvements, please feel free to submit a pull request.

## License

The Manual Gates App is licensed under the [MIT License](https://opensource.org/licenses/MIT).

## Contact

For any questions or inquiries, please contact us at sourabh.rustagi@hotmail.com.

