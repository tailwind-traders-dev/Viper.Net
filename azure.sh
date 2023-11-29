VAULT="viper-vault"
RG="viperdotnet"

#az group create --name $RG --location westus2
#az keyvault create --name $VAULT --resource-group $RG --location westus2

az keyvault secret set --vault-name $VAULT --name thingy --value "buddy"
az keyvault secret show --name thingy --vault-name $VAULT --query "value"