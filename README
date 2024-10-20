# UserAuthAPI

## Descrição
O UserAuthAPI é uma API para autenticação de usuários, que suporta funcionalidades como cadastro, login e recuperação de senha. Este projeto é construído em ASP.NET Core e utiliza o MongoDB como banco de dados.

## Requisitos
### Requisitos de Sistema
- **.NET SDK 8.0**: Necessário para compilar e executar a aplicação.
- **MongoDB**: Um banco de dados NoSQL para armazenar informações de usuários.
- **Docker**: Para facilitar a containerização da aplicação.

## Funcionalidades
- **Cadastro de Usuário**: Permite que novos usuários se cadastrem fornecendo um e-mail e uma senha.
- **Login de Usuário**: Usuários existentes podem fazer login com suas credenciais.
- **Recuperação de Senha**: Usuários podem solicitar um e-mail para recuperar suas senhas.

### Funcionalidades em Detalhe

#### 1. Recuperação de Senha
- **Cenário**: Solicitar recuperação de senha
  - Dado que o usuário fornece um e-mail
  - Quando o usuário solicita a recuperação de senha
  - Então o sistema deve enviar um e-mail com instruções de recuperação

#### 2. Login de Usuário
- **Cenário**: Login com credenciais válidas
  - Dado que existe um usuário com e-mail e senha
  - Quando o usuário tenta fazer login com essas credenciais
  - Então o login deve ser bem-sucedido

#### 3. Cadastro de Usuário
- **Cenário**: Cadastro com sucesso
  - Dado que o usuário fornece um cadastro com e-mail e senha
  - Quando o usuário se cadastra
  - Então o cadastro deve ser bem-sucedido

## Estrutura do Projeto

### Dockerfile
O projeto inclui um Dockerfile para facilitar a containerização. O Dockerfile realiza as seguintes etapas:

1. **Base**: Utiliza a imagem `aspnet:8.0` para a aplicação.
2. **Build**: Usa a imagem `sdk:8.0` para restaurar as dependências e compilar a aplicação.
3. **Publish**: Publica a aplicação em um diretório específico.
4. **Final**: Copia os arquivos publicados para a imagem base e define o ponto de entrada.

```dockerfile
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserAuthAPI.csproj", "."]
RUN dotnet restore "./UserAuthAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./UserAuthAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserAuthAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserAuthAPI.dll"]


#Integração Contínua (CI)
O projeto inclui um arquivo de configuração de CI para automatizar os testes sempre que há uma alteração no código. 
Durante o processo de testes, um arquivo de resultados no formato .trx é gerado. 
Este arquivo contém informações detalhadas sobre a execução dos testes, incluindo quais testes foram bem-sucedidos, 
quais falharam e quaisquer mensagens de erro associadas. Abaixo está o arquivo de configuração:

#- name archive - ci.yml 

name: CI - Run Tests

on:
  push:
    branches:
      - master
  pull_request:
    branches:
      - master

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET SDK
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'

      - name: Restore dependencies
        run: dotnet restore UserAuthAPI.sln

      - name: Build
        run: dotnet build UserAuthAPI.sln --configuration Debug

      - name: Run Tests
        run: dotnet test UserAuthAPI.sln --logger "trx;LogFileName=test_results.trx"

      - name: Publish Test Results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: Test Results
          path: "**/test_results.trx"
