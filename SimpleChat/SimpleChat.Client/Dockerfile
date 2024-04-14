# Stage 1: Build the .NET Core application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY SimpleChat.Client/SimpleChat.Client.csproj ./SimpleChat.Client/
COPY SimpleChat.Common/SimpleChat.Common.csproj ./SimpleChat.Common/
RUN dotnet restore SimpleChat.Client/SimpleChat.Client.csproj

COPY SimpleChat.Client/. ./SimpleChat.Client/
COPY SimpleChat.Common/. ./SimpleChat.Common/
WORKDIR /app/SimpleChat.Client
RUN dotnet publish -c Release -o out

# Stage 2: Create the runtime image with SSH and Ubuntu
FROM ubuntu:20.04 AS runtime

# Install necessary packages for SSH and .NET Runtime
RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
       openssh-server \
       wget \
       ca-certificates \
       curl \
       procps \
       libicu66 \
       libssl1.1 \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir /var/run/sshd

RUN echo 'PermitRootLogin yes' > /etc/ssh/sshd_config.d/root.conf
RUN service ssh restart

# Set root password (change 'password' to a secure password)
RUN echo 'root:root' | chpasswd

COPY SimpleChat.Client/chat-script.sh /usr/local/bin/chat
RUN chmod +x /usr/local/bin/chat

# Install .NET Runtime
RUN wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-runtime-6.0

# Copy the published output from the build stage
WORKDIR /app
COPY --from=build /app/SimpleChat.Client/out ./

# Expose SSH port and start SSH service
EXPOSE 22
CMD ["/usr/sbin/sshd", "-D"]
