
curl -X PUT "localhost:9200/_snapshot/kairos_gameservice_backup" -H 'Content-Type: application/json' -d'
{
  "type": "fs",
  "settings": {
    "location": "kairos_gameservice_backup"
  }
}
'


curl -s -XPUT localhost:9200/_snapshot/kairos_gameservice_backup/snapshot_1?wait_for_completion=true

curl -s -XDELETE localhost:9200/_snapshot/kairos_gameservice_backup/backup1


curl -XDELETE localhost:9200/kairos-gameservice-2018.10.16

curl -XDELETE localhost:9200/_all

### or

today=`date +%Y-%m-%d`
echo "today is ${today}"

daynum=51

if [ $# -gt 1 ] ;then
  echo "either 0 or 1 args"
  exit 101;
fi

if [ $# == 1 ] ;then
  daynum=$1
fi

esday=`date -d '-'"${daynum}"' day' +%Y.%m.%d`;
echo "${daynum} ago is ${esday}"

curl -XDELETE http://127.0.0.1:9200/kairos-gameservice-${esday}
echo "${esday} done"
