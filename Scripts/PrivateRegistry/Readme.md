Set up a docker private registry with basic HTTP authentication support
------

- https://docs.docker.com/registry/insecure/#use-self-signed-certificates
- https://yeasy.gitbooks.io/docker_practice/repository/registry_auth.html
- http://www.larrycaiyu.com/2014/12/01/private-docker-registry-with-nginx.html


### Create self-signed certificate

- CA

```sh
openssl genrsa -out ca.key 4096
openssl req -new -key ca.key -out ca.csr -sha256  -subj '/C=US/ST=US/L=Kairos/O=Kairos/CN=kairos'
openssl x509 -req -days 3650 -in ca.csr -signkey ca.key -sha256 -out ca.crt -extfile ca.cnf -extensions root_ca
```

- Domain (example: `registry.uat`)

```sh
openssl genrsa -out registry.uat.key 4096
openssl req -new -key registry.uat.key -out registry.uat.csr -sha256  -subj '/C=US/ST=US/L=Kairos/O=Kairos/CN=registry.uat'
openssl x509 -req -days 3650 -in registry.uat.csr -sha256 -CA ca.crt -CAkey ca.key -CAcreateserial -out registry.uat.crt -extfile registry.uat.cnf -extensions server
```

### Trust the cert on your client computer (Windows)
- Import `./Scripts/certs/ca.crt` to `Trusted Root Certification Authorities` by `certlm`, it's CN is `kairos`
- Copy `./Scripts/certs/registry.uat.crt` and `./Scripts/certs/registry.uat.key` to `C:\ProgramData\Docker\pki`
- Hosts file `192.168.86.50  registry.uat`

### Setup private registry

```sh
docker run -d -e REGISTRY_STORAGE_DELETE_ENABLED=true --restart=always --name registry -p 5000:5000 registry:2
```

- Use `docker-registry.conf` as the config on nginx

