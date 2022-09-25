#!/bin/bash
sudo apt-get install jq
CONFIG_FILE="appsettings.Production.json"
tmp=$(mktemp)
jq '.ConnectionStrings.Redis = "'$REDIS_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
jq '.MongoConnections.CollectionSettings[0].ConnectionString = "'$MONGO_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
jq '.MongoConnections.CollectionSettings[1].ConnectionString = "'$MONGO_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
jq '.MongoConnections.CollectionSettings[2].ConnectionString = "'$MONGO_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE