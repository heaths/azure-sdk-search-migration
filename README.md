# Azure Cognitive Search Migration Sample

This is a simple sample designed to create indexes, ingest documents, and query documents in Microsoft.Azure.Search,
and query those same documents in Azure.Search.Documents using the following support packages:

* [Microsoft.Azure.Core.NewtonsoftJson](https://www.nuget.org/packages/Microsoft.Azure.Core.NewtonsoftJson)
* [Microsoft.Azure.Core.Spatial](https://www.nuget.org/packages/Microsoft.Azure.Core.Spatial)
* [Microsoft.Azure.Core.Spatial.NewtonsoftJson](https://www.nuget.org/packages/Microsoft.Azure.Core.Spatial.NewtonsoftJson)

## Getting started

To begin, create a new resource group and deployment:

```bash
# Assumes you're already connected
az group create -g my-resourceGroup-name -l westus2
az group deployment create -g my-resourceGroup-name --template-file deployment.json --parameters serviceName=mySearchService
```

That will dump the Search endpoint, admin key, and query key you'll need below (you can also use the `<admin key>` in both invocations):

```bash
dotnet run -p src/Track1/Track1.csproj -- --endpoint https://mySearchService.search.windows.net --key <admin key>
dotnet run -p src/Track2/Track2.csproj -- --endpoint https://mySearchService.search.windows.net --key <query key>
```

In both invocations you should see output like the following:

```text
https://www.bing.com/maps?cp=46.85287~-121.76044&sp=point.46.85287_-121.76044_Mount%20Rainier
```

It's possible in the first invocation you will not if indexing takes longer than the `--wait` specified (default of 2 seconds), but if you run it again the index won't be created again and you should see the output.

When you're finished, you can simply delete the whole resource group:

```bash
az group delete -g my-resourceGroup-name
# You'll be prompted for confirmation
```
