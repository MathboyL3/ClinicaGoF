# ClinicaGoF - Sistema de Agendamento de Consultas

🩺 Visão Geral

## ClinicaGoF é uma aplicação ASP.NET Core 9 (Web API) desenvolvida com arquitetura limpa (Clean Architecture) e princípios SOLID, voltada para o ensino de Design Patterns (GoF), testes automatizados e boas práticas em APIs REST.

## 🚀 Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- xUnit para testes automatizados
- Swagger para documentação interativa

## 📂 Estrutura de Projeto

`ClinicaGoF/
├── src/
│   ├── ClinicaGoF.API             # Web API com controllers REST
│   ├── ClinicaGoF.Application     # Casos de uso, serviços e DTOs
│   ├── ClinicaGoF.Domain          # Entidades e interfaces de domínio
│   ├── ClinicaGoF.Infrastructure  # Persistência com EF Core InMemory
├── tests/
│   ├── ClinicaGoF.UnitTests       # Testes de unidade
│   └── ClinicaGoF.IntegrationTests # Testes de integração
`
## 🧠 Funcionalidades Atuais

### 📋 Pacientes

> GET /api/paciente — Listar todos os pacientes
> GET /api/paciente/{documento} — Buscar paciente por documento
> POST /api/paciente — Cadastrar novo paciente

### 👨‍⚕️ Médicos

> GET /api/medico — Listar todos os médicos
> GET /api/medico/{crm} — Buscar médico por CRM
> POST /api/medico — Cadastrar novo médico

### 📆 Consultas

> GET /api/consulta — Listar todas as consultas
> GET /api/consulta/paciente/{pacienteId} — Por ID de paciente
> GET /api/consulta/paciente/documento/{documento} — Por documento de paciente
> GET /api/consulta/medico/crm/{crm} — Por CRM de médico
> GET /api/consulta/intervalo?inicio=2025-01-01&fim=2025-01-31 — Por intervalo de datas
> POST /api/consulta — Agendar nova consulta

## 🧪 Testes

Implementados com xUnit
Injeção de dependência simulada com repositórios InMemory

## 🔧 Padrões e Princípios Aplicados

- Clean Architecture (camadas bem separadas)
- SOLID (com ênfase em SRP e DIP)
- DTOs para entrada e saída de dados (InputModel/ViewModel)

## 📚 Próxima Etapa

1 - Refatoração com Design Patterns Criacionais:
 - Builder: criação de consultas com fluxo fluente
 - Factory Method: envio de notificações
 - Singleton: gerenciador de notificações

2 - Refatoração com Design Patterns Estruturais
3 - Refatoração com Design Patterns Comportamentais
4 - Criação de testes Unitários e Testes de Integração

## 📦 Como Executar

# Restore e build
`cd src/ClinicaGoF.API
dotnet restore`

# Executar API
`dotnet run`

Acesse: https://localhost:7150/swagger para explorar a documentação interativa.
