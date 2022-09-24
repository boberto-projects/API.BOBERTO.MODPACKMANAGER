#!/bin/bash
APP="api-mine-modpack"
PORT=5555
dokku apps:destroy $APP --force && dokku apps:create $APP || dokku apps:create $APP
dokku nginx:validate-config $APP --clean
dokku proxy:ports-add $APP http:80:$PORT
dokku proxy:ports-add $APP https:443:$PORT
dokku proxy:ports-remove $APP http:$PORT:$PORT
dokku builder-dockerfile:set $APP dockerfile-path MinecraftServer.Api/Dockerfile
dokku redis:create redis-service
dokku redis:link redis-service $APP
dokku mongo:create mongo-service
dokku mongo:link mongo-service $APP
