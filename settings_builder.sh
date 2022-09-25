sudo apt-get install jq
tmp=$(mktemp)
jq '.ConnectionStrings.Redis = "'$REDIS_URL'"' appsettings.json > "$tmp" && mv "$tmp" appsettings.json
jq '.MongoConnections.CollectionSettings[0].ConnectionString = "'$MONGO_URL'"' appsettings.json > "$tmp" && mv "$tmp" appsettings.json
jq '.MongoConnections.CollectionSettings[1].ConnectionString = "'$MONGO_URL'"' appsettings.json > "$tmp" && mv "$tmp" appsettings.json
jq '.MongoConnections.CollectionSettings[2].ConnectionString = "'$MONGO_URL'"' appsettings.json > "$tmp" && mv "$tmp" appsettings.json
echo $MONGO_URL