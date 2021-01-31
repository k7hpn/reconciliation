# Get build image
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app

# Copy source
COPY . ./

# Bring in metadata via --build-arg for build
ARG IMAGE_VERSION=unknown

# Restore packages
RUN dotnet restore

# Publish release project
RUN dotnet publish -c Release -o "/app/publish/"

# Get runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0 AS publish
WORKDIR /app

# Bring in metadata via --build-arg to publish
ARG BRANCH=unknown
ARG IMAGE_CREATED=unknown
ARG IMAGE_REVISION=unknown
ARG IMAGE_VERSION=unknown

# Configure image labels
LABEL branch=$branch \
    maintainer="Harald Nagel <dev@hpn.is>" \
    org.opencontainers.image.authors="Harald Nagel <dev@hpn.is>" \
    org.opencontainers.image.created=$IMAGE_CREATED \
    org.opencontainers.image.description="Reconciliation coverts an Instagram backup to a Hugo blog" \
    org.opencontainers.image.documentation="https://github.com/k7hpn/reconciliation" \
    org.opencontainers.image.licenses="MIT" \
    org.opencontainers.image.revision=$IMAGE_REVISION \
    org.opencontainers.image.source="https://github.com/k7hpn/reconciliation" \
    org.opencontainers.image.title="Reconciliation" \
    org.opencontainers.image.url="https://github.com/k7hpn/reconciliation" \
    org.opencontainers.image.version=$IMAGE_VERSION

# Default image environment variable settings
ENV org.opencontainers.image.created=$IMAGE_CREATED \
    org.opencontainers.image.revision=$IMAGE_REVISION \
    org.opencontainers.image.version=$IMAGE_VERSION

# Copy source
COPY --from=build "/app/publish/" .

# Set entrypoint
ENTRYPOINT ["dotnet", "Reconciliation.dll"]
