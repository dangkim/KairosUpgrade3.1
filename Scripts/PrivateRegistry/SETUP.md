### Set up

- Import `./Scripts/certs/ca.crt` to `Trusted Root Certification Authorities` by `certlm`, it's CN is `kairos`
- Copy `./Scripts/certs/registry.uat.crt` and `./Scripts/certs/registry.uat.key` to `C:\ProgramData\Docker\pki`
- Install OpenSSH for windows https://github.com/PowerShell/Win32-OpenSSH/wiki/Install-Win32-OpenSSH
- Add `192.168.86.50 registry.uat` to Hosts file

### Verify

- Access https://registry.uat/v2/_catalog see if there is any warnning for the certificate
- Or tag to `registry.uat/<imageName>` and push on command line

### Adding a user to docker group
- Login to the server and execute `sudo usermod -aG docker $USER` to add current user to "docker" group.
- Verify using `grep /etc/group -e "docker"`. The current user should be included in the output.

### How to use

**IMPORT:** Run this script on your local not on the server, it will build the docker image based on your local source code and push and run ont UAT.

```
./Scripts/deploy_uat.ps1 -user <YOUR_USERNAME> -name <container name> -tag <version tag> -port <listen port> -env UAT
```

- `-user` : default `slt.support03`, you can change it on your local
- `-name` : default `gameservice`
- `-tag`  : default `latest`
- `-port` : default `9123`
- `-env`  : default `UAT`
