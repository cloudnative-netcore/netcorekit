### Installation

- MySQL

```bash
> helm install --name mysql --namespace netcorekit stable/mysql --set mysqlRootPassword=P@ssw0rd --set mysqlPassword=P@ssw0rd --set mysqlDatabase=maindb
```

- Redis

```bash
> helm install --name redis --namespace redis stable/redis --set usePassword=true --set password=letmein
```

- Kafka

```bash
> git clone https://github.com/confluentinc/cp-helm-charts.git
> kubectl ns create kafka
> helm install cp-helm-charts --name kafka --namespace kafka
```

- Build all solutions
- Run multiple projects `NetCoreKit.Samples.TodoAPI`, `NetCoreKit.Samples.SignalRNotifier`, and `NetCoreKit.Samples.WebNotifier`


**Notes**: Change configuration in appsettings.Development.json with your configuration on your local machine.
