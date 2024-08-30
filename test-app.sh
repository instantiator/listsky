#!/bin/bash

set -a && source test.env && set +a
dotnet run --project ListSky.App/ListSky.App.csproj $@

