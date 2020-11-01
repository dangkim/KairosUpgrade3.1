#!/bin/sh

declare name=$1
declare tag=$2
declare port=$3
declare env="Production"

while [ "$name" == "" ]
do
	echo -e $"Please provide name. e.g.gameservice"
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

docker pull kairos.azurecr.io/gameservice:$tag

matchingStarted=$(docker ps --filter="name=$name" -q | xargs)

if [[ -n $matchingStarted ]] 
then
    echo "there is a docker container is running with the same name > $name"
else
	docker run -d --restart unless-stopped \
			   --log-opt max-size=50m \
			   --add-host=proxy.slotwallet.com:192.168.77.53 \
			   --add-host=logserver:192.168.77.33 \
			   -p $port:80 \
			   -e ASPNETCORE_ENVIRONMENT=$env \
			   --name $name kairos.azurecr.io/gameservice:$tag
	docker container ls --filter="name=$p4"
fi
