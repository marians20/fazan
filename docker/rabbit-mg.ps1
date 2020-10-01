$dockerName = 'some-rabbit'
docker stop $dockerName
docker rm $dockerName
docker run `
    -d `
    --hostname my-rabbit `
    --name $dockerName `
    -p 5671-5672:5671-5672 `
    -p 15671:15671 `
    -p 15672:15672 `
    -p 25672:25672 `
    -e RABBITMQ_DEFAULT_USER=user `
    -e RABBITMQ_DEFAULT_PASS=password `
    rabbitmq:3-management