# Política de Privacidade - LiteTools

**Última atualização:** 28 de Abril de 2026

A sua privacidade é fundamental. Esta Política de Privacidade descreve como o **LiteTools** (e os seus plugins oficiais: LiteFlow, LiteJson, LiteShot e LiteAutomation) lida com as suas informações.

O princípio central do ecossistema LiteTools é o processamento local. **Nós não coletamos, não armazenamos em nuvem e não rastreamos os seus dados de uso.**

## 1. Coleta e Uso de Dados

O LiteTools é uma plataforma de hospedagem de ferramentas para Quality Assurance (QA) que opera estritamente de forma local no seu ambiente de trabalho (Desktop).

* **Nenhuma Telemetria:** O aplicativo não possui códigos de rastreamento, ferramentas de analytics ou envio de relatórios de falhas para servidores internos ou externos.
* **Processamento Local:** Todas as ações realizadas pelas ferramentas — como capturas de tela, edição de fluxos, extração de dados em JSON ou geração de scripts — são processadas na memória RAM e salvas exclusivamente no armazenamento local da sua máquina.

## 2. Permissões e Acesso ao Sistema

Para o funcionamento das ferramentas de QA, o LiteTools requer acesso a alguns recursos do sistema operacional:

* **Captura de Tela (LiteShot):** A aplicação acessa a tela para capturar imagens quando acionada pelo usuário (via atalho de teclado ou interface). As imagens permanecem locais até que o usuário decida exportá-las.
* **Gestão e Edição de Fluxos (LiteFlow):** Atua como um editor básico que recebe as capturas do LiteShot. O salvamento de projetos, a exportação de arquivos e a importação de templates são realizados apenas mediante ação manual do usuário, garantindo que nenhum dado seja processado sem consentimento.
* **Leitura de Tela/Navegador (LiteJson):** Inspeciona elementos (DOM) via porta de depuração (CDP) estritamente para extrair dados semânticos e gerar evidências estruturadas. Esses dados não são transmitidos para os criadores do LiteTools.
* **Geração de Scripts (LiteAutomation):** Opera de forma totalmente passiva e offline. Sua única função é ler os arquivos JSON gerados localmente pelo LiteJson para compilar e gerar scripts de automação de teste.
* **Armazenamento:** A aplicação lê e escreve arquivos de configuração (`litetools.json`) e carrega bibliotecas (`.dll`) a partir da sua própria pasta de instalação.

## 3. Serviços de Terceiros

O LiteTools não integra serviços de terceiros que rastreiam o seu comportamento. Ao utilizar navegadores de teste abertos pela plataforma para acessar sites da web, as políticas de privacidade desses respectivos sites se aplicarão à sua navegação normalmente.

## 4. Segurança

Como o LiteTools não armazena nem transmite os seus dados para servidores externos, a segurança das informações manipuladas (como evidências de testes, fluxos ou scripts) é garantida pelas políticas de segurança do seu próprio sistema operacional e infraestrutura corporativa.

## 5. Alterações a esta Política

Podemos atualizar esta Política de Privacidade periodicamente para refletir novos plugins ou recursos. Recomendamos que reveja esta página ao baixar novas versões.

## 6. Contato

Dúvidas ou sugestões podem ser enviadas através de uma *Issue* no repositório oficial:

🔗 **[https://github.com/eugenio122/LiteTools/issues]**
