#!/bin/sh

declare name="merchantfaker"
declare tag=$1
declare port=9083
declare env="UAT"


matchingStarted=$(docker ps --filter="name=$name" -q | xargs)
[[ -n $matchingStarted ]] && docker stop $matchingStarted

matching=$(docker ps -a --filter="name=$name" -q | xargs)
[[ -n $matching ]] && docker rm $matching


docker pull kairos.azurecr.io/merchantfaker:$tag

docker run -d --restart unless-stopped -p $port:80 -e ASPNETCORE_ENVIRONMENT=$env --name $name kairos.azurecr.io/merchantfaker:$tag

docker container ls
