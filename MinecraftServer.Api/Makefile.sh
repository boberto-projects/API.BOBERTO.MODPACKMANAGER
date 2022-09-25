#!/usr/bin/env bash
echo "Baixando dependencias"
echo "testeeee" >> "teste.txt"
apt update -y
apt install jq -y
tmp=$(mktemp)
echo "Criando appsettings... para " . $CONFIG_FILE
jq '.ConnectionStrings.Redis = "'$REDIS_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
jq '.MongoConnections.CollectionSettings[0].ConnectionString = "'$MONGO_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
jq '.MongoConnections.CollectionSettings[1].ConnectionString = "'$MONGO_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
jq '.MongoConnections.CollectionSettings[2].ConnectionString = "'$MONGO_URL'"' $CONFIG_FILE > "$tmp" && mv "$tmp" $CONFIG_FILE
apt remove jq -y
echo "appsettings editado com sucesso"