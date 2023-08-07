# Canary Gates App

The Canary Gates App is a webhook-enabled application designed to provide manual control over deployment gates in a continuous integration and deployment (CI/CD) pipeline. It allows users to manually review and approve or reject deployments at specific stages of the pipeline.

![image](https://github.com/iamsourabh-in/canary-gates/assets/22702292/2e81d81f-7e32-435d-9093-acc61e0e517f)


## Version Info

#### Latest 0.0.1

- First Beta Release

rest here in changelog

## Features

- **Webhook Integration**: Seamlessly integrates with your existing CI/CD pipeline via webhooks, following the same webhook specifications described in the [Flagger documentation](https://docs.flagger.app/usage/webhooks). This ensures easy compatibility and interoperability with your Flagger-based deployment automation.

- **Manual Approval**: Enables manual review and approval or rejection of deployments triggered by Flagger. You can configure Flagger to send webhook events to the Manual Gates App, which will pause the deployment process until manual approval is received.

- **Customizable Gates**: Define custom gates based on your specific deployment stages and criteria. Each gate can be configured with unique rules and requirements, allowing you to tailor the approval process to match your organization's policies and procedures. (TODO)

- **Intuitive Dashboard**: Provides a user-friendly dashboard to manage and monitor the deployment gates. Users can easily view pending deployments, review details, and take appropriate actions. The dashboard is designed to align with the Flagger workflow and enhance the overall user experience. (TODO)

- **Event Trail**: Keeps you updated with all the flagger Webhooks Events, taking place in the flagger controll plane, for the respective deployment.

## Getting Started

To use the Manual Gates App in conjunction with Flagger in your CI/CD pipeline, follow these steps: 

1. **Installation**: Clone the repository and configure the required environment variables as specified in the project documentation. OR You could you the docker image avilalbe here : `rohitrustagi007/canary-gates:1.0.7`

2. **Webhook Integration**: Follow the instructions provided in the [Flagger documentation](https://docs.flagger.app/usage/webhooks) to configure Flagger to send webhook events to the Manual Gates App. This integration enables the app to receive deployment events and pause the deployment process for manual approval.

3. **Configuration**: Customize the deployment gates based on your requirements, specifying the necessary rules, criteria, and approvers for each gate. You can adapt the configuration to match your organization's specific workflows and deployment stages.

4. **Usage**: Start your CI/CD pipeline with Flagger. When a gate is reached, Flagger will trigger a webhook event, and the Manual Gates App will receive it, pausing the deployment. Relevant users will be notified to review and approve or reject the deployment.

5. **Monitoring**: Utilize the provided dashboard to monitor and manage the deployments at each gate. Track approvals, rejections, and comments for auditing and compliance purposes. The dashboard provides visibility into the status of ongoing deployments and facilitates seamless collaboration among team members.

## Contributions

Contributions to the Manual Gates App are welcome! If you encounter any issues, have suggestions, or would like to contribute new features or improvements, please feel free to submit a pull request.

## License

The Manual Gates App is licensed under the [MIT License](https://opensource.org/licenses/MIT).

## Contact

For any questions or inquiries, please contact our support team at support@example.com.


## Prepare Docker Image
```sh

npm run build

docker build -t rohitrustagi007/canary-gates:latest .
docker tag rohitrustagi007/canary-gates:latest rohitrustagi007/canary-gates:latest
docker run -p 3000:3000 rohitrustagi007/canary-gates:latest

docker build -t rohitrustagi007/canary-gates:1.0.2 .
docker tag rohitrustagi007/canary-gates:1.0.1 rohitrustagi007/canary-gates:1.0.2
docker run -p 3000:3000 rohitrustagi007/canary-gates:1.0.2
docker push rohitrustagi007/canary-gates:1.0.2
```

## Deploy to Kubernetes

```sh
kubectl apply -f ./deploy/deployment.yaml
```
## Configure Flagger Webhooks

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
        url: http://canary-gates-service.default/confirm-rollout/gate/check
        metadata:
          type: webhookType
          cmd: "confirm-rollout"
      - name: "load test"
        type: rollout
        url: http://canary-gates-service.default/confirm-rollout/gate/check
        timeout: 15s
        metadata:
          cmd: "hey -z 1m -q 5 -c 2 http://podinfo-canary.test:9898/"
      - name: "traffic increase gate"
        type: confirm-traffic-increase
        url: http://canary-gates-service.default/gate/check
      - name: "promotion gate"
        type: confirm-promotion
        url: http://canary-gates-service.default/confirm-promotion/gate/check
      - name: smoke-test
        type: pre-rollout
        url: http://canary-gates-service.default/pre-rollout/gate/check
        timeout: 120s
        metadata:
          type: bash
          cmd: "curl -sd 'anon' http://podinfo-canary.test:9898/token | grep token"
      - name: load-test
        url: http://canary-gates-service.default/confirm-rollout/gate/check
        timeout: 5s
        metadata:
          type: cmd
          cmd: "hey -z 1m -q 10 -c 2 http://podinfo-canary.test:9898/"

```