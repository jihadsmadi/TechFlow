\# TechFlow - Task Management System



A modern task management platform built with:

\- \*\*Backend\*\*: ASP.NET Core 8 (Clean Architecture + CQRS)

\- \*\*Frontend\*\*: Angular 17 (SPA) + MVC Dashboard

\- \*\*Database\*\*: SQL Server + Redis (caching)

\- \*\*Real-time\*\*: SignalR



\## Project Structure

\- `TechFlow.Domain` - Entities, Value Objects, Interfaces

\- `TechFlow.Application` - CQRS, DTOs, Use Cases

\- `TechFlow.Infrastructure` - EF Core, Repositories, Services

\- `TechFlow.API` - Web API for Angular

\- `TechFlow.Web` - MVC Dashboard for Admin

\- `client/` - Angular SPA



\## Getting Started

1\. Run `docker-compose up -d` to start SQL Server + Redis

2\. Update connection strings in `appsettings.json`

3\. Run `dotnet run --project TechFlow.API`

4\. Run `cd client \&\& ng serve` for Angular

5\. Run `dotnet run --project TechFlow.Web` for Dashboard



\## Features

\- ✅ Clean Architecture

\- ✅ CQRS with MediatR

\- ✅ JWT + Refresh Tokens

\- ✅ Dynamic Custom Fields (Showcase)

\- ✅ Drag-drop Tasks

\- ✅ Real-time Updates (Future)

\- ✅ Saved Filters (Future)

