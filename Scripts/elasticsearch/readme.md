### Installation

- vm https://www.elastic.co/guide/en/elasticsearch/reference/current/vm-max-map-count.html
- elastisearch https://www.elastic.co/guide/en/elasticsearch/reference/6.4/docker.html#docker-cli-run-prod-mode
- kibana https://www.elastic.co/guide/en/kibana/6.4/production.html

--------

#### run elastisearch cluster

```sh
docker-compose up -d
```


#### curator
https://www.elastic.co/guide/en/elasticsearch/client/curator/current/yum-repository.html


curator delete_indices.yml --config curator.yml


#### cron
https://www.rosehosting.com/blog/automate-system-tasks-using-cron-on-centos-7/

#### metricbeat
https://www.elastic.co/guide/en/beats/metricbeat/current/setup-repositories.html#_yum

Copy one of `Scripts/elasticsearch/metricbeat/metricbeat.*` to `/etc/metricbeat/metricbeat.yml`

restart service `sudo service metricbeat restart`

#### run on production
- https://www.elastic.co/guide/en/elasticsearch/reference/current/system-config.html
