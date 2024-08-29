# ListSky

## Build your own

* Fork this repository
* You may need to enable github actions through GitHub
* Update your configuration

### Configuration

To configure the runtime environment, provide the following as GithHub Actions Secrets:

* `Server_AT` (the AT server, eg. `bsky.social`)
* `AccountName_AT` (the account that publishes the lists, eg. `instantiator.bsky.social`)
* `AppPassword_AT` (create an app password for this use case)
* `Path_AllListsMetadataJson` (relative path to your config, probably best left as: `Data/lists.json`)

To test locally, create a file called `test.env` file in the root of this repository, and provide values for the same properties:

```env
Server_AT=
AccountName_AT=
AppPassword_AT=
Path_AllListsMetadataJson=
```

### Build and test

```bash
dotnet build
dotnet test
```

**⚠️ Warning.** The tests clean up after themselves by deleting all lists on the test profile starting with the words: `Unit test`

