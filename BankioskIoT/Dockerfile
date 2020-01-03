FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim-arm32v7 AS base
RUN apt-get update  # && apt-get upgrade 
RUN apt-get install -y --no-install-recommends pcscd libpcsclite1 pcsc-tools
RUN apt-get install dos2unix
RUN rm -rf /var/lib/apt/lists/* 
RUN apt-get -y autoremove
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["BankioskIoT/BankioskIoT.csproj", "./BankioskIoT/"]
COPY ["UsbPcscReader/UsbPcscReader.csproj", "./UsbPcscReader/"]
COPY ["pcsc-sharp/src/PCSC/PCSC.csproj", "./pcsc-sharp/src/PCSC/"]
COPY ["PN532/Pn532/Pn532.csproj", "./PN532/Pn532/"]
COPY ["PN532/Card/CardRfid.csproj", "./PN532/Card/"]
COPY ["PN532/Card/CreditCard/CreditCardProcessing.csproj", "./PN532/Card/CreditCard/"]

RUN dotnet restore "./BankioskIoT/BankioskIoT.csproj"
COPY BankioskIoT ./BankioskIoT
COPY UsbPcscReader ./UsbPcscReader
COPY pcsc-sharp ./pcsc-sharp
COPY PN532/Pn532 ./PN532/Pn532
COPY PN532/Card ./PN532/Card

WORKDIR /src
RUN dotnet build "./BankioskIoT/BankioskIoT.csproj" -o /app

FROM build AS publish
RUN dotnet publish "./BankioskIoT/BankioskIoT.csproj" -r linux-arm -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY ["start.sh", "start.sh"]
RUN dos2unix start.sh
ENTRYPOINT ["/bin/bash", "start.sh"]