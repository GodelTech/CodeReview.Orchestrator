# CodeReview.Orchestrator

#### new
Generating new skeleton manifest file
<pre>
> dotnet CodeReview.Orchestrator.dll new -o manifest.yml
</pre>
| Agruments     | Key       | Required  | Type      | Description agrument      |
| ------------- | --------- | --------- | --------- | ------------------------- |
| --output      | -o        | true      | string    | Path to output file       |

#### new-engine-collection
Creates draft of Docker engine collection manifest.
<pre>
> dotnet CodeReview.Orchestrator.dll new-engine-collection -o docker-engines.yml
</pre>
| Agruments     | Key       | Required  | Type      | Description agrument      |
| ------------- | --------- | --------- | --------- | ------------------------- |
| --output      | -o        | true      | string    | Path to output file       |


#### run
Creating container and import files into
<pre>
> dotnet CodeReview.Orchestrator.dll run -f manifest.yml -b LoadLatest -e true
</pre>
| Agruments     | Key       | Required  | Type      | Description agrument      |
| ------------- | --------- | --------- | --------- | ------------------------- |
| --file        | -f        | true      | string    | Path to workflow file     |
| --docker-engines | -d        | false      | string    | Path to Docker Engine collection manifest |
| --behavior    | -b        | false     | enum: None, LoadIfMissing, LoadLatest | Defines behavior of Docker Image loading. LoadIfMissing by default |
| --exit        | -e        | false     | bool      |Terminates execution of program after Docker Images are downloaded. False by defalut |

#### eval
Validating manifest file
<pre>
> dotnet CodeReview.Orchestrator.dll eval -f manifest.yml -o output.txt
</pre>
| Agruments     | Key       | Required  | Type      | Description agrument      |
| ------------- | --------- | --------- | --------- | ------------------------- |
| --file        | -f        | true      | string    | Path to workflow file     |
| --output      | -o        | true      | string    | Path to output file       |

#### extract-metadata
Generate a json with all commands and parameters.
<pre>
> dotnet CodeReview.Orchestrator.dll extract-metadata -o output.txt
</pre>
| Agruments     | Key       | Required  | Type      | Description agrument      |
| ------------- | --------- | --------- | --------- | ------------------------- |
| --output      | -o        | true      | string    | Path to output file       |