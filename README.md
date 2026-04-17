# LiteTools :gear:

O **LiteTools** é a plataforma base (Nave-mãe) de código aberto projetada para unificar, orquestrar e potencializar ferramentas de Quality Assurance (QA) em ambientes corporativos. Desenvolvido em C# (.NET 10) e Windows Forms, ele atua como um motor central que carrega plugins dinamicamente (como o LiteShot e o LiteFlow), gerenciando a comunicação entre eles, configurações unificadas e execução em segundo plano com altíssima performance.

<br>

## 🔒 Segurança e Privacidade

> [!IMPORTANT]
> **O LiteTools é uma plataforma 100% offline.**
>
> * Não realiza chamadas de rede.
> * Não possui telemetria.
> * Não coleta métricas de uso.
> * Não envia dados para a nuvem.
> * Não possui mecanismo de auto-update.
>
> Todo o processamento e tráfego de dados entre os plugins ocorrem localmente na memória RAM (com descarte imediato). O código é open source (MIT) para permitir auditoria e total transparência em ambientes corporativos.

<br>

## 🎯 Non-Goals (O que NÃO fazemos)

> [!WARNING]
> **O LiteTools não tem como objetivo:**
>
> * Substituir ferramentas corporativas oficiais.
> * Sincronizar ou fazer upload de dados na nuvem.
> * Consumir recursos excessivos do sistema (CPU/RAM).
> * Coletar informações de usuários (telemetria zero).
>
> O foco é estritamente fornecer um ambiente de execução local ultrarrápido, seguro e extensível para ferramentas de produtividade de QA.

<br> 

## 📥 Download

Você não precisa baixar o código-fonte para usar! Baixe a versão portátil e pronta para uso diretamente na página de Releases do GitHub:

[**Baixar LiteTools (Versão Mais Recente)**](https://github.com/eugenio122/LiteTools/releases/latest)

<br>

## ✨ Funcionalidades

* **Ecossistema Dinâmico de Plugins:** Arquitetura baseada no contrato ILitePlugin. Importe, ative, desative e recarregue plugins (DLLs) diretamente pela interface, sem precisar reiniciar o aplicativo.

* **Performance Corporativa (Zero-Latency):** Motor nativo de Publish/Subscribe otimizado rigorosamente para testes de estresse extremos. Suporta o tráfego de dezenas de capturas de tela e dados por segundo sem engasgar o sistema operacional ou estourar a memória.

* **Internacionalização Global (i18n):** O Host dita o idioma da frota. Suporte nativo para 6 idiomas (PT-BR, EN-US, ES-ES, FR-FR, DE-DE, IT-IT). Ao alterar o idioma no LiteTools, a interface de todos os plugins conectados é traduzida instantaneamente.

* **Interface Unificada e Silenciosa:** Um painel de configurações elegante que agrupa as opções de todos os plugins instalados. Execução discreta na Bandeja do Sistema (System Tray) com gerenciamento inteligente de notificações (Anti-Spam do Windows).

* **100% Portátil (Self-Contained):** Distribuído como um executável único. Não exige a instalação prévia do .NET Runtime na máquina do usuário. Basta extrair e usar.

<br>

## 🚀 Como usar

1. Extraia o conteúdo do .zip baixado para uma pasta de sua preferência.
2. Coloque os seus plugins (ex: LiteShot.dll, LiteFlow.dll) dentro da pasta plugins que fica ao lado do executável.
3. Nota: Bibliotecas de terceiros necessárias aos plugins (como o OpenXML) podem ficar diretamente na raiz, junto ao LiteTools.exe.
4. Execute o LiteTools.exe. A plataforma abrirá e, ao fechar a janela, continuará executando silenciosamente na sua Bandeja do Sistema (ícone perto do relógio).
5. Dê um duplo clique no ícone da bandeja para gerenciar seus plugins, idiomas e configurações globais a qualquer momento.

<br>

## 🛠️ Como compilar (Para Desenvolvedores)

Este projeto foi construído usando .NET 10. Para gerar o executável portátil (contendo o Host e a interface de contrato), utilize o comando abaixo na raiz da solução:

**Build Portátil (Arquivo único, roda em qualquer PC sem depender do .NET instalado):**

```bash
dotnet publish LiteTools\LiteTools.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

<br>

## 🧩 Desenvolvendo seus próprios Plugins (SDK)

Para criar uma ferramenta compatível com a Nave-mãe, crie um projeto de Biblioteca de Classes (.NET), adicione a referência ao arquivo LiteTools.Interfaces.dll e implemente a interface ILitePlugin na sua classe principal.
