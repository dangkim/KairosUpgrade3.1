param (
    [string]$name = "gameservice",
    [string]$tag = "latest",
    [string]$user = "slt.support03",
    [int]$port = 9123,
    [string]$env = "UAT"
)

$Registry = "registry.uat"
$UatServer = "192.168.86.50"
$Repository = "kairos"
$DockerImage = "${Repository}/${name}:${tag}"


Write-Host "Start to build docker image '${name}:${tag}'" -ForegroundColor Green
docker build -t $DockerImage -f .\Slot.WebApiCore\Dockerfile .
docker tag $DockerImage $Registry/$DockerImage

Write-Host "Pushing '${Registry}/${DockerImage}'" -ForegroundColor Green
docker push $Registry/$DockerImage

$session = New-PSSession -HostName ${UatServer} -UserName $user -SSHTransport
Enter-PSSession -Session $session

Invoke-Command $session -ScriptBlock { 
    param($p1, $p2, $p3, $p4, $p5)

    # pull the latest image
    docker pull $p1

    # stop and remove conflicts container
    $containers = docker ps --filter="name=^/$p4$" -q
    foreach ($x in $containers) {
        docker stop $x
        docker rm $x
    }

    # run new instance
    docker run -d --restart unless-stopped --network=host --log-opt max-size=50m -e ASPNETCORE_URLS=$p2 -e ASPNETCORE_ENVIRONMENT=$p3 --name $p4 $p1

    # show if the container is running
    docker container ls --filter="name=$p4"

} -ArgumentList ${Registry}/${DockerImage}, "http://+:${port}", $env, ${name}

Exit-PSSession

Write-Host "Done" -ForegroundColor Green

