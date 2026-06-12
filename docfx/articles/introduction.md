# Introduction

## Overview

WorkoutTracker helps users record training sessions and meals. Authenticated users manage their own workouts and meals; administrators can manage cached nutrition products.

## Architecture

- **UI:** ASP.NET Core MVC with Razor views and Identity UI (Razor Pages)
- **Persistence:** SQL Server with Entity Framework Core (code first)
- **Services:** `MealService`, `NutritionSummaryService`, `ProductCacheService`, `FatSecretClient`
- **Auth:** ASP.NET Core Identity with `User` and `Admin` roles
- **External API:** FatSecret (OAuth 2.0, JSON)

## Main features

1. **Workouts** — CRUD for workouts and nested exercise entries
2. **Meals** — CRUD for meals and items, daily nutrition totals, and charts
3. **FatSecret** — search foods and prefill meal items from API data
4. **Admin products** — browse and edit cached products (`Admin` role)
5. **Docker** — local stack with SQL Server and automatic migrations

## Configuration

See the project `README.md` in the repository root for connection strings, FatSecret credentials, optional admin seed, and Docker setup.
