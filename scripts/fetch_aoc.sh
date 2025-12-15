#!/usr/bin/env bash
set -euo pipefail

# Fetch Advent of Code puzzle input (and optionally the puzzle page) using session cookie
# Usage:
#   export AOC_SESSION="<cookie-value>"
#   # Recommended: only fetch input and ensure test file exists
#   ./scripts/fetch_aoc.sh 2025 11 \
#     DotNet/AOC2025/Resources/Day11_Test.txt \
#     DotNet/AOC2025/Resources/Day11_Input.txt
#
#   # Optional: also fetch the HTML page (NOT recommended to store inside repo)
#   ./scripts/fetch_aoc.sh 2025 11 <test-out> <input-out> /tmp/Day11_Instructions.html
#
# Notes:
# - Don't save the HTML into the repository (Resources). Keep it outside the project if needed.
# - Test file usually needs to be copied manually from the instructions page.

YEAR=${1:-}
DAY=${2:-}
TEST_PATH=${3:-}
INPUT_PATH=${4:-}
PAGE_OUT=${5:-}

if [[ -z "${YEAR}" || -z "${DAY}" ]]; then
  echo "Usage: ./scripts/fetch_aoc.sh <year> <day> <test-out> <input-out> <page-html-out>" >&2
  exit 2
fi

if [[ -z "${AOC_SESSION:-}" ]]; then
  echo "Error: AOC_SESSION environment variable is not set."
  echo "Find your session cookie from adventofcode.com and run:"
  echo "  export AOC_SESSION=\"<cookie>\""
  exit 2
fi

BASE_URL="https://adventofcode.com/${YEAR}/day/${DAY}"
UA="github.com/richardpineo/AOC2025 (script)"

# Fetch puzzle page (HTML) only if PAGE_OUT provided and not inside repo Resources
if [[ -n "${PAGE_OUT}" ]]; then
  if [[ "${PAGE_OUT}" == *"/Resources/"* ]]; then
    echo "Skipping HTML fetch: refusing to write puzzle page inside repository Resources." >&2
    echo "Provide a path outside the repo (e.g., /tmp/Day${DAY}_Instructions.html) if needed." >&2
  else
    echo "Fetching puzzle page to ${PAGE_OUT}"
    curl -sS --fail \
      -H "Cookie: session=${AOC_SESSION}" \
      -H "User-Agent: ${UA}" \
      "${BASE_URL}" -o "${PAGE_OUT}"
  fi
fi

# Fetch input (personalized)
if [[ -n "${INPUT_PATH}" ]]; then
  echo "Fetching input to ${INPUT_PATH}"
  curl -sS --fail \
    -H "Cookie: session=${AOC_SESSION}" \
    -H "User-Agent: ${UA}" \
    "${BASE_URL}/input" -o "${INPUT_PATH}"
fi

# Ensure test file exists (empty if not provided earlier)
if [[ -n "${TEST_PATH}" && ! -f "${TEST_PATH}" ]]; then
  echo "Creating empty test file at ${TEST_PATH}"
  mkdir -p "$(dirname "${TEST_PATH}")"
  : > "${TEST_PATH}"
fi

echo "Done. Open the HTML to copy the sample into the test file."
