
# Copilot Engineer Pessoal

AI Engineering Platform para desenvolvimento assistido por IA.

Este projeto implementa um **Copilot Engineer Pessoal**, um sistema que utiliza LLMs, agentes especialistas, memória contextual e ferramentas para auxiliar desenvolvedores no ciclo completo de engenharia de software.

O objetivo é transformar LLMs em **engenheiros assistentes**, capazes de:

- entender issues
- analisar arquitetura
- investigar bugs
- revisar código
- analisar SQL
- gerar testes
- gerar documentação
- propor refatorações

---

# Regras para agentes que trabalham neste repositório

Este arquivo define como **agentes automatizados (Codex, GPT, Claude, Copilot Agents)** devem interagir com o projeto.

Sempre siga estas regras antes de modificar o código.

---

# Objetivo do projeto

Implementar um **Copilot Engineer Pessoal em .NET utilizando Semantic Kernel**, com arquitetura baseada em:

- Engineer Core
- Specialist Agents
- Skills
- Tools
- MCP Servers
- Memory
- Workflow Engine

A arquitetura completa está documentada em:

/docs/copilot-engineer-pessoal.md

---

# Stack principal

Este projeto utiliza:

- .NET 8
- C#
- Semantic Kernel
- System.CommandLine
- SQLite (memória episódica no MVP)
- YAML (memória estrutural)
- MCP Servers (filesystem, git no MVP)
- Docker

---

# Estrutura do projeto

CopilotEngineer/

src/
  CopilotEngineer.Core/
  CopilotEngineer.Agents/
  CopilotEngineer.Memory/
  CopilotEngineer.Workflows/
  CopilotEngineer.McpClients/
  CopilotEngineer.Cli/

tests/
  CopilotEngineer.Tests/

docs/
  copilot-engineer-pessoal.md

memory/
  project-context.yaml
  conventions.yaml

prompts/
  engineer-core.md
  debug-specialist.md
  database-specialist.md
  code-specialist.md

workflows/
  issue-analysis.yaml
  bug-investigation.yaml
  sql-analysis.yaml

---

# Arquitetura do sistema

User
 ↓
CLI / IDE
 ↓
Engineer Core
 ↓
Intent Router
 ↓
Specialist Agents
 ↓
Skills
 ↓
Tools / MCP Servers
 ↓
Memory

O **Engineer Core** é o ponto central de orquestração.

---

# Engineer Core

Responsável por:

- interpretar requests do usuário
- identificar intenção
- selecionar especialistas
- combinar respostas
- retornar artefatos estruturados

Classe principal:

EngineerCore

Componentes:

IntentRouter
AgentRegistry
WorkflowExecutor

---

# Specialist Agents

## Debug Specialist

Responsável por:

- analisar stack traces
- correlacionar logs
- identificar possíveis causas raiz
- sugerir correções

Skills:

- analyze_stack_trace
- investigate_bug
- correlate_logs

---

## Database Specialist

Responsável por:

- analisar queries SQL
- sugerir índices
- detectar problemas de join
- identificar gargalos de performance

Skills:

- analyze_sql_query
- suggest_indexes
- explain_query_plan

---

## Code Specialist

Responsável por:

- revisar código
- identificar code smells
- sugerir refatorações
- gerar testes

Skills:

- review_code
- generate_tests
- suggest_refactor

---

# Skills

Skills são **capacidades reutilizáveis** utilizadas por agentes.

Exemplos no MVP:

- analyze_issue
- analyze_sql_query
- inspect_stack_trace
- generate_adr
- create_test_plan
- review_code

Cada skill deve ser implementada de forma modular.

---

# Tools

Tools executam ações determinísticas.

Exemplos:

- read_file
- search_code
- git_diff
- run_dotnet_build
- run_dotnet_tests
- sql_explain

Tools **não devem conter lógica de IA**.

---

# Memory

## Memória estrutural

Arquivos YAML:

memory/project-context.yaml
memory/conventions.yaml

Contém:

- arquitetura do projeto
- convenções de código
- decisões técnicas

## Memória episódica

Armazenada em SQLite.

Contém:

- bugs investigados
- queries analisadas
- soluções anteriores

---

# CLI

Interface principal do sistema.

Comandos previstos:

- copilot debug
- copilot sql analyze
- copilot review
- copilot test generate
- copilot ask

Todos os comandos devem chamar o **EngineerCore**.

---

# Processo obrigatório de alteração

Antes de qualquer modificação significativa:

1. apresentar plano curto
2. implementar mudanças
3. rodar build
4. rodar testes
5. apresentar resumo das alterações

---

# Comandos obrigatórios de validação

dotnet restore
dotnet build
dotnet test

O projeto **não deve ser deixado quebrado**.

---

# Guardrails

Agentes **não devem**:

- alterar arquitetura sem justificativa
- adicionar dependências desnecessárias
- modificar estrutura de pastas
- executar comandos destrutivos
- remover testes existentes

---

# Fluxo de desenvolvimento recomendado

### Fase 1 — Bootstrap

Criar:

- solution
- projetos
- interfaces base
- estrutura de pastas

### Fase 2 — Engineer Core

Implementar:

- EngineerCore
- IntentRouter
- AgentRegistry

### Fase 3 — Specialist Agents

Implementar:

- Debug Specialist
- Database Specialist
- Code Specialist

### Fase 4 — CLI

Implementar CLI com `System.CommandLine`.

### Fase 5 — Memory

Implementar:

- loaders YAML
- memória episódica SQLite

### Fase 6 — Integração com Semantic Kernel

Substituir stubs por chamadas reais de LLM.

---

# Critérios de aceite do MVP

O MVP é considerado funcional quando:

- o projeto compila
- existe CLI funcional
- agentes podem ser chamados
- memória básica funciona
- pelo menos uma skill por agente está ativa

---

# Convenções de código

Usar:

- PascalCase para classes
- camelCase para variáveis
- interfaces com prefixo `I`
- injeção de dependência

---

# Referências

/docs/copilot-engineer-pessoal.md
