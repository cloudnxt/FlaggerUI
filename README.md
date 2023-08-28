### NOT A PRODUCTION READY APP !!
### Can be used in dev and stage.

<p align="center" style="float:left;" >
  <img src="https://github.com/cloudnxt/FlaggerUI/blob/main/docs/images/logo.png" style="width:100px;" alt="Flagger UI"/> 
</p>


![uihd](https://github.com/cloudnxt/FlaggerUI/assets/22702292/e1883e24-bf18-4ca0-830f-60515b6d2de5)



<h1>Flagger UI</h1>



The Flagger UI App is a webhook-enabled application designed to provide manual control over deployment gates in a continuous integration and deployment (CI/CD) pipeline. 

It allows users to manually review and approve or reject deployments at specific stages of the pipeline.


---
## Summary

- [**Why Flagger UI?**](#why-flagger-ui)
- [**Features**](#features)
- [**Getting Started**](#getting-started)
- [**Development**](#development-guides)
- [**License**](#license)


## Why flagger UI?

Seamlessly integrates with your existing app setting with just an annotation to the deployment. Currently approval initially in the dev, stage env when using flagger , the team might be dependent on flagger-loadtester, which is a cli base, and requires a overhead, 

When using Flagger UI, you just need to add anotations to the App, and configure webhook URL, Rest can be taken care at Flagger UI, the App, also shows the state each deployment is and you can keep track of the events. Te app allows to filter events based on deployments.

## Features

- **Plug and play**: Seamlessly integrates with your existing app setting with just an annotation to the deployment. The annotation will add the app tot the dashboard automatically. 


- **Webhook Integration**: Following the same webhook specifications described in the [Flagger documentation](https://docs.flagger.app/usage/webhooks). This ensures easy compatibility and interoperability with your Flagger-based deployment automation.

- **Manual Approval**: Enables manual review and approval or rejection of deployments triggered by Flagger. You can configure Flagger to send webhook events to the Manual Gates App, which will pause the deployment process until manual approval is received.

- **Intuitive Dashboard**: Provides a user-friendly dashboard to manage and monitor the deployment gates. Users can easily view pending deployments, review details, and take appropriate actions. The dashboard is designed to align with the Flagger workflow and enhance the overall user experience. (TODO)

- **Event Trail**: Keeps you updated with all the flagger Webhooks Events, taking place in the flagger controll plane, for the respective deployment.


# Getting Started
To use the Manual Gates App in conjunction with Flagger in your CI/CD pipeline, follow these steps: 

## Prerequisites

- **Flagger** - should be installed and running along with its CRD in the cluster, version 1.26 and above. learn more [here](https://docs.flagger.app)
- **Kubernetes** - version 1.24 


## Understanding Hooks

The canary analysis can be extended with webhooks. Flagger will call each webhook URL and determine from the response status code (HTTP 2xx) if the canary is failing or not.

Below are the hooks that can be configured for flagger UI. 

* `confirm-rollout` hooks are executed before scaling up the canary deployment and can be used for manual approval. The rollout is paused until the hook returns a successful HTTP status code.

* `rollout` hooks are executed during the analysis on each iteration before the metric checks. If a rollout hook call fails the canary advancement is paused and eventfully rolled back.

* `confirm-traffic-increase` hooks are executed right before the weight on the canary is increased. The canary advancement is paused until this hook returns HTTP 200.

* `confirm-promotion` hooks are executed before the promotion step. The canary promotion is paused until the hooks return HTTP 200. While the promotion is paused, Flagger will continue to run the metrics checks and rollout hooks.

* `rollback` hooks are executed while a canary deployment is in either Progressing or Waiting status. This provides the ability to rollback during analysis or while waiting for a confirmation. If a rollback hook returns a successful HTTP status code, Flagger will stop the analysis and mark the canary release as failed.

* `event` hooks are executed every time Flagger emits a Kubernetes event. When configured, every action that Flagger takes during a canary deployment will be sent as JSON via an HTTP POST request.

![image](https://github.com/cloudnxt/FlaggerUI/blob/main/docs/asdasd.png)

## Installation

Clone the repository and configure the required environment variables as specified in the project documentation. OR You could you the docker image available here : `cloudnxt/flagger-ui:0.0.1`. 

Flagger UI can currently be installed using helm charts. This can be done either getting the repo locally or from the helm repo


```sh

#direct Install

helm install flagger-ui https://cloudnxt.github.io/FlaggerUI/flagger-ui-0.0.1.tgz

# Add Flagger UI Repo
helm repo add flagger-ui  https://cloudnxt.github.io/FlaggerUI/

#update Repo
helm repo update


# uninstall flagger-ui
helm uninstall flagger-ui
```

Forward the port to to expose the flagger ui locally.

```sh
kubectl port-forward svc/flagger-ui 80:5443
```

## Configuration

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

The thumbs up sign shows all good, the waiting icon will change from `thumbs up` to `radioactive` to show the webhook is currently waiting.

The gate will look like below.

![image](https://github.com/cloudnxt/FlaggerUI/blob/main/docs/images/confirmrollout.png)


## Webhook Integration

Flagger UI, exposes some endpoint which flagger, canary can be configured to invoke in each state. We will be taking a look at the full configuration when we discuss about configuring the canary resource.

* **api/Gate/check**- The endpoint is used for maintaining the state of the deployment when a new version is out. The payload have a metadata object which will contain the field to specify the webhook state 

```
- name: "start gate"
  type: confirm-rollout
  url: http://flagger-ui.default.svc/api/Gate/check
  metadata:
    Action: "pass"
    WebhookState: "ConfirmRollout"
```
* **api/Event** - This Endpoint if configured to keep track of all the Event happening in Flagger for each and all deployments.

```
- name: "start gate"
  type: confirm-rollout
  url: http://flagger-ui.default.svc/api/Gate/check
  metadata:
    Action: "pass"
    WebhookState: "ConfirmRollout"
```

* **api/Loadtest** - This endpoints allows you to configure a load for a particulat URL, it lets you specify the payload and request which needs to be send, and also the number of request.

```
- name: "load test"
  type: rollout
  url: http://flagger-ui.default.svc/api/Loadtest
  metadata:
    Action: "pass"
    WebhookState: "rollout"
    Method: GET
    Url: http://podinfo-canary.default.svc:9898/healthz
    NoOfRequests: "100"
    Payload: ""
```

## Configuring the Full Canary Resource


Follow the instructions provided in the [Flagger documentation](https://docs.flagger.app/usage/webhooks) to configure Flagger to send webhook events to the Manual Gates App. 
This integration enables the app to receive deployment events and pause the deployment process for manual approval.


Configuring the CRD Canary of flagger.

```sh
apiVersion: flagger.app/v1beta1
kind: Canary
metadata:
  name: podinfo
  namespace: default
spec:
  skipAnalysis: false
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
  # # HPA reference (optional)
  # autoscalerRef:
  #   apiVersion: autoscaling/v2beta2
  #   kind: HorizontalPodAutoscaler
  #   name: podinfo
  service:
    port: 9898
    portDiscovery: true
  analysis:
    # schedule interval (default 60s)
    interval: 30s
    # max number of failed checks before rollback
    threshold: 5
    # number of checks to run before rollback
    iterations: 5
    # Prometheus checks based on 
    # http_request_duration_seconds histogram
    metrics:
      - name: "request-success-rate-custom"
        templateRef:
          name: request-success-rate-custom
        thresholdRange:
          min: 99
        interval: 1m
    # acceptance/load testing hooks
    webhooks:
      - name: "start gate"
        type: confirm-rollout
        url: http://flagger-ui.default.svc/api/Gate/check
        metadata:
          Action: "pass"
          WebhookState: "ConfirmRollout"

      - name: "confirm-traffic-increase"
        type: confirm-traffic-increase
        url: http://flagger-ui.default.svc/api/Gate/check
        metadata:
          Action: "pass"
          WebhookState: "ConfirmTrafficIncrease"
          
          
      - name: "load test"
        type: rollout
        url: http://flagger-ui.default.svc/api/Loadtest
        metadata:
          Action: "pass"
          WebhookState: "rollout"
          Method: GET
          Url: http://podinfo-canary.default.svc:9898/healthz
          NoOfRequests: "100"
          Payload: ""
      
      - name: "promotion gate"
        type: confirm-promotion
        url: http://flagger-ui.default.svc/api/Gate/check
        metadata:
          Action: "pass"
          WebhookState: "ConfirmPromotion"

      - name: "Send to Events"
        type: event
        url: http://flagger-ui.default.svc/api/Event
        metadata:
          Action: "pass"
          WebhookState: "Event"

      - name: "rollback gate"
        type: rollback
        url: http://flagger-ui.default.svc/api/Gate/Check
        metadata:
          Action: "pass"
          WebhookState: "Rollback"


```

## Usage

Start your CI/CD pipeline with Flagger. When a gate is reached, Flagger will trigger a webhook event, and the Manual Gates App will receive it, pausing the deployment. Relevant users will be notified to review and approve or reject the deployment.

## Monitoring

Utilize the provided dashboard to monitor and manage the deployments at each gate. Track approvals, rejections, and comments for auditing and compliance purposes. The dashboard provides visibility into the status of ongoing deployments.


# Development Guides

If trying to get this repo locally and use it or to build you own image.

See [Developer Guide](DEVELOPER.md) for further details.

# Contributions

Contributions to the Manual Gates App are welcome! If you encounter any issues, have suggestions, or would like to contribute new features or improvements, please feel free to submit a pull request.

# License

The Manual Gates App is licensed under the [MIT License](https://opensource.org/licenses/MIT).

# Contact

For any questions or inquiries, please contact us at sourabh.rustagi@hotmail.com.

