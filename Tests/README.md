# Test Suite

## Overview
Comprehensive test suite for the Blog Application with unit and integration tests.

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Unit Tests Only
```bash
dotnet test Tests/DeveloperAssessment.Tests.Unit
```

### Run Integration Tests Only
```bash
dotnet test Tests/DeveloperAssessment.Tests.Integration
```

## Test Structure

### Unit Tests (27 tests)
- **BlogController Tests** - Blog post display, comment and reply functionality
- **HomeController Tests** - Home page, privacy page, and error handling
- **ViewModel Validation Tests** - Form validation and data annotations

### Integration Tests (19 tests)
- **BlogWorkflow Tests** - End-to-end testing of blog post display and navigation
- **DataPersistence Tests** - JSON file operations and data integrity

## Frameworks Used
- xUnit 2.6.1
- FluentAssertions 6.12.0
- Moq 4.20.69
- Microsoft.AspNetCore.Mvc.Testing 8.0.0
