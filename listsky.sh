#!/bin/bash

set -a && source test.env && set +a
export GITHUB_TOKEN=$(gh auth token)
dotnet run --project ListSky.App/ListSky.App.csproj $@

