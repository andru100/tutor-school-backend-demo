# Tutoring-App README

## Overview
Tutoring-App is an online learning/tutoring platform built using C# and ASP.NET. It provides a backend infrastructure for managing user accounts, roles, and endpoints.

## Authentication
The app utilizes Identity framework with SignInManager for user authentication and management.

## User Management
Users in the system inherit from IdentityUser and have additional properties to cater to the online learning environment.

## Endpoints Structure
Endpoints are organized into Mutation.cs and Query.cs files to handle different types of operations efficiently.

## Account Management
Login and user management functionalities are centralized in the AccountController.cs file for easy access and maintenance.

## User Roles and Policies
Users can have roles such as Teacher, Student, or Admin, managed through policies with corresponding names. Different endpoints are protected by these policies to control access based on user roles.

## Database
The app uses PostgreSQL as the database.

## Deployment
There is a GitHub workflow set up to deploy the app to Azure Container Instances.

## Environment Variables
Environment variables are required for configuration and are provided in the .env.example file.

## Database Seeding
You can seed the database with mock data using seed.sh, which will wipe and seed the database.