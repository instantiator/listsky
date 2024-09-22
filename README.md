# ListSky

[![on-push-to-main](https://github.com/instantiator/listsky/actions/workflows/on-push-to-main.yaml/badge.svg)](https://github.com/instantiator/listsky/actions/workflows/on-push-to-main.yaml)

ListSky is a repository app that can manage any number of BlueSky lists.

When a pull request is merged to `main`, the contents of the `Data` directory are used to update each list on BlueSky.

The application also publishes the contents of all lists through GitHub Pages. See: [ListSky](https://instantiator.dev/listsky)

## How to add people to these lists

There are 2 ways to submit an update to the lists:

1. Create a pull request (preferred)
2. Create an issue

Each is explained below.

### 1. Create a pull request (fast option, bit technical)

This tool will update lists on BlueSky based on edits made to CSV files in the `Data` directory.

1. Fork this repository
1. Make your edits to a list CSV in the `Data` directory
   
   (eg. edit `Data/list-ci.csv` to add a new Collective Intelligence person)

1. Commit and push your change to your fork
1. Create a pull request for your change to the source repository
1. Tests will run against your pull request to make sure there are no mistakes
1. Your pull request will be reviewed, and if it passes it will be merged into the main repository

Here's an [example pull request](https://github.com/instantiator/listsky/pull/9). If in doubt, ask a developer friend to help!

### 2. Create an issue (bit slower, bit less technical)

You could also [create a new issue](https://github.com/instantiator/listsky/issues/new/choose). Make sure you provide enough info about the people you want to add. (As a minimum, provide a name, description, BlueSky account, and state which list you'd like to add them to.)

* Name
* Description
* Record type (individual / organisation / community / bot)
* BlueSky account
* List(s) to add them to

You can also add details of a number of other accounts:

* Other accounts (optional)
  * Mastodon
  * Twitter
  * LinkedIn
  * YouTube
  * GitHub
  * RSS feed url
  * Website url
  * Blog url

This is a bit less technical - someone will review this and manually modify the list. It'll also go a bit slower as they have a little more work to do this way.

### Required fields

Please ensure you provide a value for:

* `Name` - name of the person/organisation/community/bot to include in the list
* `Type` - type of record (see below)
* `Description` - a description of the record you're adding that links it to the list
* `AccountName_BlueSky` - the AT account name - for BlueSky these are often suffixed with `.bsky.social`, eg. `instantiator.bsky.social`

All other columns are optional, but very helpful!

### `Type` of record

When editing a CSV record, note that it must have a value in the `Type` column. This can be:

* `Individual` - a person who should be included in the list
* `Organisation` - a charity, company, government department, etc. (ie. a formal organisation)
* `Community` - an account representing an association of people that's not a formal organisation
* `Bot` - a helpful bot that ought to be included in the list

**NB. Match the spelling and capitalisation above - in particular the UK English spelling of Organisation!**

# Build your own ListSky

1. Fork this repository
1. You may need to enable github actions through GitHub
1. Create secrets to configure your instance (described in the following section)
1. Create all the lists you're going to use, and get their ids
1. Create new CSV files in the `Data` directory for each list, copy headings from existing CSV files
1. Modify `Data/lists.jsonc` to point to your new lists and new CSV files
1. Set up branch protections to prevent accidental pushes to `main`
1. Require that the tests pass before pull requests can be merged (use workflow `on-pull-request-run-tests`)

## Configuration

To configure the runtime environment, provide values for the following GithHub Actions repository secrets:

* `Server_AT` (the AT server, eg. `bsky.social`)
* `AccountName_AT` (the account that publishes the lists, eg. `instantiator.bsky.social`)
* `AppPassword_AT` (create an app password for this use case)
* `Path_AllListsMetadataJson` (relative path to your config, probably best left as: `Data/lists.jsonc`)

To test locally, create a file called `test.env` file in the root of this repository, and provide values for these properties:

```env
Server_AT=
AccountName_AT=
AppPassword_AT=
Path_AllListsMetadataJson=
GITHUB_REPO=
GITHUB_USER=
```

NB. `GITHUB_REPO` and `GITHUB_USER` are provided by GitHub Actions, so do not need to be provided as repository secrets.

# Developer notes

## Build and run tests locally

Use the `run-tests.sh` script.

```bash
./run-tests.sh
```

If no filter is provided, this will run all the tests - including BlueSky and data tests.

To do so, the tests will source environment variables from your `test.env` file. so that you can test from 

**⚠️ Warning.** The tests clean up after themselves by deleting all lists on the test profile starting with the words: `Unit test`

### Run a subset of the tests

You can provide a `TestCategory` to filter tests, eg.

```bash
./run-tests.sh --filter TestCategory=Unit
```

| TestCategory | Tests | Description |
|-|-|-|
| `Unit` | **Unit tests** | These test individual pieces code in isolation, and do not rely on configuration, data, or external services. |
| `BlueSky` | **BlueSky/AT tests** | These test the specific code that uses the BlueSky API and confirms that it can connect and perform operations. |
| `Config` | **Configuration and data tests** | These test the list configuration and data for validity - against required fields, and social networks for valid accounts. |

## Build and run the app locally

Use the `listsky.sh` script with the command you wish to test.

```bash
./test-app.sh apply
```

Environment variables will be sourced from `test.env`.

## Testing workflows

Install [act](https://nektosact.com/installation/index.html) and the GitHub CLI...


```bash
brew install act
brew install gh
```

You'll need to provide a github token to `act` to be able to interact with you repository. `gh auth token` can provide this, and we pass that in to `act` as a secret called `GITHUB_TOKEN`

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
