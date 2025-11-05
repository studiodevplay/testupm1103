
#!/usr/bin/env bash

# Extract current version from package.json
get_current_version() {
  local PACKAGE_JSON=$1
  grep '"version"' "$PACKAGE_JSON" | sed -E 's/.*"([0-9]+\.[0-9]+\.[0-9]+)".*/\1/'
}

# Calculate next version
bump_version() {
  local VERSION=$1
  local PART=$2 # major / minor / patch
  IFS='.' read -r MAJOR MINOR PATCH <<< "$VERSION"
  
  case $PART in
    major)
      MAJOR=$((MAJOR + 1)); MINOR=0; PATCH=0 ;;
    minor)
      MINOR=$((MINOR + 1)); PATCH=0 ;;
    patch|*)
      PATCH=$((PATCH + 1)) ;;
  esac

  echo "$MAJOR.$MINOR.$PATCH"
}
