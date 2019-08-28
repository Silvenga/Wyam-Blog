FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS wyam
WORKDIR /source
RUN dotnet tool install -g Wyam.Tool --version 2.2.4 \
    && apt-get update \
    && apt-get install -y libc6-dev
ADD recipe/Wyam.SlightBlog.csproj /source/recipe/
RUN dotnet restore recipe/
ADD recipe/ /source/recipe/
RUN dotnet publish recipe/

FROM node:current AS webpack
WORKDIR /source
ADD ["src/yarn.lock", "src/package.json", "/source/src/"]
RUN cd /source/src/ \
    && yarn install

FROM wyam AS publish-stage1
ADD ["input", "config.wyam", "/source/"]
RUN LD_DEBUG=libs /root/.dotnet/tools/wyam

FROM webpack AS publish-stage2
COPY --from=stage /source/output /source/output
ADD src/ /source/src/
RUN yarn build