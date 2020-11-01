Kairos 
===========
opportunity, luck and favorable moments.

- All game modules must be under `GameModules` folder
- All game modules must have `ModuleInfo` attribute
- All game modules must be pure function, don't do any operation to database.

Api
==========
- https://localhost:5001/api/User/Authenticate?op=faker&token=valor
- https://localhost:5001/api/User/GetFunPlayKey?op=faker
- https://localhost:5001/api/Game/getbets?key=valor&game=moneymonkey
- https://localhost:5001/api/Game/spin?key=valor&game=moneymonkey&bet=10

Build && Publish
==========
```sh
Slot.WebApiCore> dotnet publish -c Release -r win8-x64
```

Benchmark
==========
https://github.com/aliostad/SuperBenchmarker

usages:
- call `http://localhost:5432/api/User/Authenticate?op=faker&token=valor`
- `sb -u "http://localhost:5432/api/Game/spin?operator=faker&key=valor&game=monkeysmash&bet=10" -n 1000 -c 100`

Docker
======

```
docker build -t kairos/reelgems-gameservice:v2.0 -f .\Slot.WebApiCore\Dockerfile .
```

- run on docker
```sh
docker run -e "ASPNETCORE_ENVIRONMENT=Development" -it -p 8100:80 --name kairos kairos
docker run -d -p 9001:80 --name gameservice kairos.azurecr.io/gameservice:v2_14
```

- build && running all services
```sh
docker-compose build
docker-compose up
```

- run with 3 instances of gameservice
```sh
docker-compose -f .\docker-compose.yml -f .\docker-compose.prod.yml up --scale gameservice=3 -d
```

- deploy to UAT
```sh
./deploy_uat.sh ${container_name} ${tag} ${port}
```
e.g. `./deploy_uat.sh gameservice_moneymonkey v13 9100`

- create a reverse proxy for the container
```sh
sudo ./create_proxy.sh ${servicename} ${port}
```
e.g. `sudo ./create_proxy.sh kairos03 9100`

access `http://192.168.86.50/kairos03`

Releases Branch (https://semver.org/)
===============

- `releases/v{version}`

- version: major.minor.patch
- example: 
- v3.2.0-uat
- v4.0.0-prod

SSH on windows
==============
https://github.com/PowerShell/Win32-OpenSSH/wiki/Install-Win32-OpenSSH
https://docs.microsoft.com/en-us/powershell/scripting/core-powershell/ssh-remoting-in-powershell-core?view=powershell-6
