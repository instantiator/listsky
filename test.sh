#!/bin/bash

set -a && source test.env && set +a
dotnet test
