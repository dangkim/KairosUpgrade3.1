#!/bin/sh

declare servicename=$1
declare servicepath=$2
declare sites='/etc/nginx/default.d/'

echo $""
echo $"The script is used for create reverse proxy on nginx"
echo $""
echo $"> sudo ./create_proxy.sh \$servicename \$servicepath"
echo $""
echo $"e.g."
echo $"> sudo ./create_proxy.sh kairos01 http://localhost:8100"
echo ""

if [ "$(whoami)" != 'root' ]; then
    echo $"You have no permission to run $0 as non-root user. Use sudo"
        exit 1;
fi

while [ "$servicename" == "" ]
do
	echo -e $"Please provide service name. e.g.dev,staging"
	read servicename
done

### check if service name already exists
if [ -e $sites$servicename ]; then
    echo -e $"This servicename already exists.\nPlease Try Another one"
    exit;
fi

while [ "$servicepath" == "" ]
do
	echo -e $"Please provide servicepath"
	read servicepath
done

if ! echo "
location /$servicename/ {
    proxy_pass         $servicepath;
    proxy_http_version 1.1;
    proxy_set_header   Upgrade \$http_upgrade;
    proxy_set_header   Connection keep-alive;
    proxy_set_header   Host \$host;
    proxy_cache_bypass \$http_upgrade;
    proxy_set_header   X-Real-IP $remote_addr;
    proxy_set_header   X-Forwarded-For \$proxy_add_x_forwarded_for;
    proxy_set_header   X-Forwarded-Proto \$scheme;
}
" > $sites$servicename.conf
then
    echo -e $"There is an ERROR create $servicename file"
    exit;
else
    echo -e $"\nNew Reverse Proxy Created\n"
fi

ls -lh $sites

nginx -t && service nginx reload

echo -e $"Nginx is reloaded"
