# WEATHER API

## Docker

Instalación dotnet (entorno WSL2)

```bash
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
```

Creación de aplicación ejemplo:

```bash
dotnet new webapi -controllers -n weather-api
```

Ejecución:

```bash
dotnet run watch
```

Modificaremos el controller para mostrar un par de variables de entorno
```c++
[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
    string? valueHost = Environment.GetEnvironmentVariable(varHost);
    string? valueEnv = Environment.GetEnvironmentVariable(varEnv);
    if (varHost != null && varEnv != null)
    {
        whereiam = string.Format("Environment:{0} - Host: {1}", valueEnv, valueHost);
    }
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        LocalInfo = whereiam,
    })
    .ToArray();
```

Validación
```
curl http://localhost:5095/weatherforecast
  [{"date":"2025-02-27","temperatureC":26,"temperatureF":78,"summary":"Balmy","localInfo":"Environment:local - Host: docker"},{"date":"2025-02-28","temperatureC":13,"temperatureF":55,"summary":"Warm","localInfo":"Environment:local - Host: docker"},{"date":"2025-03-01","temperatureC":37,"temperatureF":98,"summary":"Chilly","localInfo":"Environment:local - Host: docker"},{"date":"2025-03-02","temperatureC":4,"temperatureF":39,"summary":"Freezing","localInfo":"Environment:local - Host: docker"},{"date":"2025-03-03","temperatureC":-9,"temperatureF":16,"summary":"Bracing","localInfo":"Environment:local - Host: wsl"}]
```


Construcción de imagen:

```bash
docker build -t weather-api:1.0 .
```

Ejecución contenedor:

```bash
docker run -d -p 8080:8080 --env APPENVIRONMENT=local --env APPHOST=docker --name weatherapi weather-api:1.0
```

Validación
```
curl http://localhost:8080/weatherforecast
  [{"date":"2025-02-27","temperatureC":26,"temperatureF":78,"summary":"Balmy","localInfo":"Environment:local - Host: docker"},{"date":"2025-02-28","temperatureC":13,"temperatureF":55,"summary":"Warm","localInfo":"Environment:local - Host: docker"},{"date":"2025-03-01","temperatureC":37,"temperatureF":98,"summary":"Chilly","localInfo":"Environment:local - Host: docker"},{"date":"2025-03-02","temperatureC":4,"temperatureF":39,"summary":"Freezing","localInfo":"Environment:local - Host: docker"},{"date":"2025-03-03","temperatureC":-9,"temperatureF":16,"summary":"Bracing","localInfo":"Environment:local - Host: wsl"}]
```


Publicación imagen DockerHub

```bash
docker tag weather-api:1.0 anxony/weather-api:1.0
docker login
docker push anxony/weather-api:1.0
```

## Helm

Creamos namespace
```bash
kubectl create ns test-helm
```

Instalamos aplicación
```bash
helm upgrade --install weatherapi ./helm-chart/ -n test-helm
```

Desinstalamos aplicación
```bash
helm uninstall weatherapi -n test-helm
```

Creamos namespace
```bash
kubectl create ns charts
```

Hacemos login en el servicio Registry
```bash
helm registry login https://harbor.local
```

Creamos paquete helm
```
helm package .
  Successfully packaged chart and saved it to: /mnt/c/T480LCK/LIBRARY/KUBERNETES/src/agm-helm-repos/weather-api/helm-chart/weather-api-0.1.0.tgz
```

Subimos el artefacto al servicio Regsitry
```bash
helm push weather-api-0.1.0.tgz oci://harbor.local/library/ --insecure-skip-tls-verify 
  Pushed: harbor.local/library/weather-api:0.1.0
  Digest: sha256:779ba159ee40e6e870d07f7ff049c143d56a1bafe9b3b05455a9d7a3f3283836
```

Obtenemos el artefacto del servicio Regsitry
```bash
helm pull https://harbor.local/library/weather-api --version 1.0.0 --insecure-skip-tls-verify
```
Añadir repositorio a Helm
```bash
helm repo add myharbor https://harbor.local/library/ --insecure-skip-tls-verify 
```
> Este sería el comando, pero Harbor ya no incluye `Chartmuseum`,y parece que `helm repo add` no es compatible con registros `OCI`

