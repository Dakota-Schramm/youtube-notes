# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ASP.NET Core MVC web application targeting .NET 10. Root namespace is `youtube_notes`.

## Commands

- **Run**: `dotnet run`
- **Build**: `dotnet build`
- **Watch (hot reload)**: `dotnet watch run`
- **Run tests**: `dotnet test` (no test project yet)

Dev server runs at http://localhost:5247 (HTTP) or https://localhost:7003 (HTTPS).

## Architecture

Standard ASP.NET Core MVC with conventional routing (`{controller=Home}/{action=Index}/{id?}`). Entry point is `Program.cs` using minimal hosting. Views use Razor with Bootstrap and jQuery from `wwwroot/lib/`.
