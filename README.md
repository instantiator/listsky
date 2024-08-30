# ListSky

## How to add people to these lists

This tool will update lists on BlueSky based on edits made to CSV files in the `Data` directory.

1. Fork this repository
1. Make your edits to a list CSV (eg. edit `list-ci.csv` to add a new Collective Intelligence person)
1. Commit and push your change to your fork
1. Create a pull request for your change to the source repository
1. Tests will run against your pull request to make sure there are no mistakes
1. Your pull request will be reviewed, and if it passes it will be merged into the main repository

If in doubt, ask a developer friend to help!

### Types of record

Each CSV record must have a value in the `Type` column. This can be:

* `Individual` - a person who should be included in the list
* `Organisation` - a charity, company, government department, etc. (ie. a formal organisation)
* `Community` - an account representing an association of people that's not a formal organisation
* `Bot` - a helpful bot that ought to be included in the list

**NB. Match the spelling and capitalisation above - in particular the UK English spelling of Organisation!**

### Required fields

Please ensure you provide a value for:

* `Name` - name of the person/organisation/community/bot to include in the list
* `Type` - type of record (see above)
* `Description` - a description of the record you're adding that links it to the list
* `AccountName_BlueSky` - the AT account name - for BlueSky these are often suffixed with `.bsky.social`, eg. `instantiator.bsky.social`

All other columns are optional, but very helpful!

## Build your own

1. Fork this repository
1. You may need to enable github actions through GitHub
1. Create all the lists you're going to use, and get their ids
1. Create new CSV files in the `Data` directory
1. Update `Data/lists.json` to point to your new lists and new CSV files
1. Set up branch protections to prevent accidental pushes to `main`
1. Require that the tests pass before pull requests can be merged (use workflow `on-pull-request-run-tests`)

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

## Developer notes

### Build and test locally

Use the `run-tests.sh` script. This will first source environment variables from your `test.env` file - so that you can test from 

```bash
./run-tests.sh
```

**⚠️ Warning.** The tests clean up after themselves by deleting all lists on the test profile starting with the words: `Unit test`

Use the `test-app.sh` script with the `apply` command to run the application with environment variables from `test.env`.

```bash
./test-app.sh apply
```

### Testing workflows

Install [act](https://nektosact.com/installation/index.html) and the GitHub CLI...


```bash
brew install act
brew install gh
```

You'll need to provide a github token to ACT to be able to interact with you repository. `gh auth token` can provide this, and we pass that in to `act` as a secret called `GITHUB_TOKEN`

```bash
gh auth login
```

Use act to simulate running tests on a pull request:

```bash
act -W .github/workflows/on-pull-request-run-tests.yaml --secret-file test.env -s GITHUB_TOKEN="$(gh auth token)" -j "on-pull-request-run-tests"
```

Use `act` to invoke the tests action directly:

```bash
act -W .github/workflows/actions/run-tests.yaml --secret-file test.env -s GITHUB_TOKEN="$(gh auth token)" -j "run-listsky-tests"
```

Use `act` to simulate a push to main:

```bash
act -W .github/workflows/on-push-to-main-apply.yaml --secret-file test.env -s GITHUB_TOKEN="$(gh auth token)" -j "on-push-to-main-apply"
```
