#!/bin/sh

declare name=$1
declare tag=$2
declare port=$3
declare env="UAT"

while [ "$name" == "" ]
do
    echo -e $"Please provide name. e.g.gameservice_moneymonkey"
    read name
done

while [ "$tag" == "" ]
do
    echo -e $"Please provide tag. e.g.v1"
    read tag
done

while [ "$port" == "" ]
do
    echo -e $"Please provide port. e.g.1234"
    read port
done

matchingStarted=$(docker ps --filter="name=$name" -q | xargs)
[[ -n $matchingStarted ]] && docker stop $matchingStarted

matching=$(docker ps -a --filter="name=$name" -q | xargs)
[[ -n $matching ]] && docker rm $matching

docker pull kairos.azurecr.io/gameservice:$tag

docker run -d --restart unless-stopped --log-opt max-size=100m --network=host -e ASPNETCORE_URLS=http://+:$port -e ASPNETCORE_ENVIRONMENT=$env --name $name kairos.azurecr.io/gameservice:$tag

docker container ls
