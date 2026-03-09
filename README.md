# Selenium BDD Test Automation Framework (Endava SoA Training)


[![CI Status](https://github.com/IskraKrasimirova/SeleniumFramework/actions/workflows/ci.yml/badge.svg)](https://github.com/IskraKrasimirova/SeleniumFramework/actions/workflows/ci.yml)


This repository contains a **UI + API automation framework** built in C# using Selenium WebDriver and ReqNroll. The framework tests the **SoA SUT Application** — a microservices-based web platform for user management.

--- 

# 🏗️ System Under Test (SUT): SoA Application

This automation framework tests the **SoA Application** — a microservices-based user management system used in the Endava SoA Training.

The application consists of three Dockerized services:

- **PHP Frontend (Apache)** – http://localhost:8080  
- **Python REST API (Flask)** – http://localhost:5000  
- **MySQL Database** – localhost:3306  

It provides functionality for:
- User authentication  
- User CRUD operations  
- Country & city lookup  
- Skills management  

### 🔗 Full SUT Documentation  
The complete documentation for the SoA Application (architecture, API endpoints, Docker setup, database details, cleanup instructions, etc.) is available in the main application repository:

👉 **https://github.com/Endava-Sofia/SoA-SUT**  

### 🚀 Start the SUT Locally  
To run the application locally:

```bash
docker compose -f docker-compose-v2.yml up -d
```
### The application will be available at:
 - UI: http://localhost:8080
 - API: http://localhost:5000

### Default credentials:
 - Admin: admin@automation.com
 - Password: pass123

## Overview
This repository hosts a **Selenium-based BDD test automation framework** that is being **iteratively developed during the Endava SoA Training**.  
The primary goal is to demonstrate **production-grade automation architecture**, tooling, and engineering practices using modern .NET ecosystem components.

The framework is intentionally designed as an **educational yet enterprise-ready reference implementation**.

---

## Key Objectives
- Build a **maintainable and scalable** UI automation framework
- Apply **Behavior-Driven Development (BDD)** principles end-to-end
- Demonstrate **clean architecture**, **SOLID**, and **design patterns**
- Showcase **Dependency Injection (DI)** and configuration best practices
- Integrate UI automation with **API-level validation** where applicable
- Provide a foundation suitable for **CI/CD execution**

---

## Technology Stack
| Area | Technology |
|----|----|
| Language | C# (.NET) |
| UI Automation | Selenium WebDriver |
| BDD | ReqNRoll |
| Dependency Injection | .NET DI / IoC containers |
| Configuration | appsettings / environment-based |
| Assertions | Build-in in nunit |
| Patterns | Page Object, Factory, Builder |

---

## Framework Characteristics
- **BDD-first approach**
  - Feature files as the primary source of truth
  - Clear separation between *what* is tested and *how* it is implemented
- **Layered architecture**
  - Features → Steps → Pages → Infrastructure
- **Strong DI usage**
  - No static state
  - Fully injectable WebDriver, contexts, and services
- **Extensible by design**
  - Easy to add new browsers, environments, and test layers
- **Training-oriented evolution**
  - Codebase grows alongside the SoA training modules

---

## Repository Structure (High-Level)
```text
/Features        -> Gherkin feature files
/Steps           -> Step definitions
/Pages           -> Page Object abstractions
/Infrastructure  -> WebDriver, hooks, configuration, DI
/Tests           -> Test execution entry points
