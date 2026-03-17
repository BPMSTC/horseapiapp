# horseapiapp

This repository contains two .NET solutions used together for a horse management sample application:

## Solutions

### 1. horseapispring26 (ASP.NET Core Web API)
- Folder: `horseapispring26`
- Purpose: Backend API for horse data and authentication/authorization flows.
- Entry solution: `horseapispring26/horseapispring26.slnx`

### 2. horseappspring26 (.NET MAUI MVVM Mobile App)
- Folder: `horseappspring26`
- Purpose: Cross-platform mobile client that consumes the API.
- Entry solution: `horseappspring26/horseappspring26.slnx`

## Repository Intent

This repo is intended to hold both backend and mobile app code in one place so they can evolve together for the Spring 2026 version of the project.

## Quick Start

### API
1. Open `horseapispring26/horseapispring26.slnx` in Visual Studio.
2. Restore and run the API project.
3. Verify endpoints with Swagger or the included `.http` file.

### Mobile App
1. Open `horseappspring26/horseappspring26.slnx` in Visual Studio.
2. Select a target (Windows/Android/iOS as available).
3. Run the MAUI app and point it to the running API as needed.

## Source Control

The repository was reset and renamed for Spring 2026, and now uses:
- API name: `horseapispring26`
- GitHub repository: `BPMSTC/horseapiapp`
