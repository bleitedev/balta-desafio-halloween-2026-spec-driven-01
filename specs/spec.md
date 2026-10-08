# Gerador de Senhas Fortes

## Problema
Usuários e sistemas frequentemente criam senhas previsíveis, curtas ou fracas por conveniência, expondo contas e acessos a vulnerabilidades. Além disso, quando geram senhas complexas, muitas vezes não possuem um mecanismo prático para armazená-las e recuperá-las sob demanda por meio de um identificador exclusivo.

## Objetivo
Disponibilizar uma interface programática simples para gerar senhas aleatórias de alta complexidade seguindo um padrão estrito, armazenar cada senha gerada e permitir sua consulta posterior por meio de um identificador único universal (GUID).

## Usuários
- **Aplicações e Serviços Clientes:** Sistemas que necessitam provisionar credenciais seguras para novos usuários ou processos automatizados.
- **Desenvolvedores e Integradores:** Profissionais que utilizam a API para obter e validar credenciais temporárias ou de teste durante o desenvolvimento.

## Histórias
- **Como** sistema integrador,  
  **quero** solicitar a geração de uma nova senha forte,  
  **para que** eu receba uma credencial que atenda aos padrões de segurança e um identificador para consultá-la depois.
- **Como** sistema integrador,  
  **quero** consultar uma senha existente informando seu identificador exclusivo,  
  **para que** eu possa recuperar a credencial exata gerada anteriormente.

## Requisitos funcionais
- **RF01 - Geração de Senha:** O sistema deve gerar uma senha aleatória que atenda a todas as regras de complexidade estipuladas.
- **RF02 - Registro de Senha:** O sistema deve persistir a senha gerada associando-a a um identificador exclusivo (GUID) e registrar a data/hora de criação.
- **RF03 - Retorno da Geração:** Ao gerar com sucesso, o sistema deve responder com o identificador exclusivo gerado e a respectiva senha.
- **RF04 - Consulta por Identificador:** O sistema deve permitir a busca de uma senha a partir de um identificador informado.
- **RF05 - Retorno da Consulta:** Se o identificador for encontrado, o sistema deve exibir os dados da credencial (identificador, senha e data/hora de criação).
- **RF06 - Identificador Inexistente:** Se o identificador não for localizado, o sistema deve retornar uma indicação explícita de recurso não encontrado.
- **RF07 - Identificador Inválido:** Se o formato do identificador fornecido for inválido, o sistema deve rejeitar a requisição com detalhes do erro.

## Regras de negócio
- **RN01 - Comprimento Mínimo:** Toda senha gerada deve possuir no mínimo 16 caracteres.
- **RN02 - Caracteres Especiais Obrigatórios:** Toda senha gerada deve conter obrigatoriamente um ou mais caracteres especiais (ex.: `!`, `@`, `#`, `$`, `%`, `&`, `*`, `-`, `_`, `+`, `=`).
- **RN03 - Ausência de Espaços:** Nenhuma senha gerada pode conter espaços em branco (espaço simples, tabulação ou quebras de linha).
- **RN04 - Diversidade de Caracteres:** A senha deve combinar letras maiúsculas, minúsculas, números e caracteres especiais.
- **RN05 - Unicidade do Identificador:** Cada registro deve possuir um GUID único e imutável.
- **RN06 - Imutabilidade da Senha:** Uma vez gerada e registrada, a senha não pode ser alterada.

## Casos de borda
- **Identificador em formato não reconhecido:** O cliente envia uma string arbitrária que não corresponde a um padrão GUID válido (deve resultar em erro de validação/requisição inválida).
- **Identificador válido, porém inexistente:** O cliente envia um GUID formatado corretamente, mas que não consta na base de dados (deve resultar em recurso não encontrado).
- **Geração concorrente:** Múltiplas requisições simultâneas de geração devem produzir senhas e identificadores distintos e independentes sem colisões.

## Fora de escopo
- Criptografia ou hashing de senhas em repouso (sistema exclusivamente didático/lúdico).
- Atualização, revogação ou exclusão de senhas geradas.
- Autenticação, autorização ou controle de permissões de acesso aos dados.
- Configuração personalizada de tamanho ou conjunto de caracteres pelo cliente (a geração segue estritamente o padrão fixo do sistema).
- Interface gráfica (front-end web, mobile ou desktop).

## Critérios de aceite
- **Cenário 1: Geração com sucesso**
  - **Dado** uma solicitação de geração de senha,
  - **Quando** a operação for processada,
  - **Então** o sistema deve retornar confirmação de criação com um identificador válido e uma senha que possua ao menos 16 caracteres, contenha caracteres especiais e não possua espaços.

- **Cenário 2: Consulta com sucesso**
  - **Dado** um identificador previamente gerado e existente,
  - **Quando** for realizada a consulta por este identificador,
  - **Então** o sistema deve retornar a senha correspondente e a data/hora em que foi registrada.

- **Cenário 3: Consulta de identificador inexistente**
  - **Dado** um identificador no formato correto que não existe na base,
  - **Quando** for realizada a consulta,
  - **Então** o sistema deve informar que o recurso não foi localizado.

- **Cenário 4: Consulta com formato inválido**
  - **Dado** um valor que não obedece à estrutura de um identificador válido,
  - **Quando** for submetido para consulta,
  - **Então** o sistema deve rejeitar a requisição com mensagem padronizada de validação.