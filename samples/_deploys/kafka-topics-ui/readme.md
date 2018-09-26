### Setup Kafka & its ecosystem

```bash
> git clone https://github.com/confluentinc/cp-helm-charts.git
> kubectl ns create kafka
> helm install . --name kafka --namespace kafka
> # helm install cp-helm-charts --name kafka --namespace kafka
```

### Setup kafka-topics-ui

```bash
> kubectl create -f kafka-topics-ui.yaml
```

### Dashbard

```bash
> http://localhost:8000
```

Notes: https://github.com/Landoop/kafka-topics-ui/issues/91