# TUnit

<a href="https://trendshift.io/repositories/11781" target="_blank"><img src="https://trendshift.io/api/badge/repositories/11781" alt="thomhurst%2FTUnit | Trendshift" style="width: 250px; height: 55px;" width="250" height="55"/></a>

A modern, flexible and fast testing framework for .NET 8 and up. With Native AOT and Trimmed Single File application support included! 

TUnit is designed to aid with all testing types:
- Unit
- Integration
- Acceptance
- and more!


![GitHub Repo stars](https://img.shields.io/github/stars/thomhurst/TUnit) [![GitHub Sponsors](https://img.shields.io/github/sponsors/thomhurst)](https://github.com/sponsors/thomhurst)
 [![nuget](https://img.shields.io/nuget/v/TUnit.svg)](https://www.nuget.org/packages/TUnit/) [![NuGet Downloads](https://img.shields.io/nuget/dt/TUnit)](https://www.nuget.org/packages/TUnit/)
 ![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/thomhurst/TUnit/dotnet.yml) ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/thomhurst/TUnit/main) ![License](https://img.shields.io/github/license/thomhurst/TUnit) 

## Documentation

See here: <https://thomhurst.github.io/TUnit/>

## Modern and Fast
TUnit leverages source generators to locate and register your tests as opposed to reflection. You'll have a slight bump in build time, but a speedier runtime.

TUnit also builds upon the newer Microsoft.Testing.Platform, whereas most other frameworks you'll have used will use VSTest. The new platform was reconstructed from the ground up to address pain points, be more extensible, and be faster.

## Hooks, Events and Lifecycles
One of the most powerful parts of TUnit is the information you have available to you because of the source generation and the events you can subscribe to.
Because tests are constructed at the point of discovery, and not at runtime, you know all your arguments, properties, etc. upfront.

You can then register to be notified about various events such as test registered (scheduled to run in this test session at some point in the future), test started, test finished, etc.

Say we injected an external object into our tests:
By knowing how many tests are registered, we could count them up, and then on a test end event, we could decrease the count. When hitting 0, we know our object isn't going to be used by any other tests, so we can dispose of it. We know when we can handle the lifecycle, and this prevents it from living till the end of the test session where it could be hanging on to precious resources.

## Built in Analyzers
TUnit tries to help you write your tests correctly with analyzers. If something isn't quite right, an analyzer should tell you what's wrong.

## IDE

TUnit is built on top of the newer Microsoft.Testing.Platform, as opposed to the older VSTest platform. Because the infrastructure behind the scenes is new and different, you may need to enable some settings. This should just be a one time thing.

### Visual Studio

Visual Studio is supported. The "Use testing platform server mode" option must be selected in Tools > Manage Preview Features.

<img src="/docs/static/img/visual-studio.png" height="300px">

### Rider

Rider is supported. The [Enable Testing Platform support](https://www.jetbrains.com/help/rider/Reference__Options__Tools__Unit_Testing__VSTest.html) option must be selected in Settings > Build, Execution, Deployment > Unit Testing > VSTest.

<img src="/docs/static/img/rider.png" height="300px">

## VS Code
Visual Studio Code is supported.

- Install the extension Name: [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
- Go to the C# Dev Kit extension's settings
- Enable Dotnet > Test Window > Use Testing Platform Protocol

<img src="/docs/static/img/visual-studio-code.png" height="300px">

### CLI
`dotnet` CLI - Fully supported. Tests should be runnable with `dotnet test`, `dotnet run`, `dotnet exec` or executing an executable directly. See the docs for more information!

## Packages

### TUnit.Core
To be used when you want to define re-useable components, such as a test library, but it wouldn't be run as its own test suite.

### TUnit.Engine
For test suites. This contains the test execution logic and test adapter. Only install this on actual test projects you intend to run, not class libraries.

### TUnit.Assertions
This is independent from the framework and can be used wherever - Even in other test frameworks. It is just an assertion library used to assert data is as you expect. It uses an asychronous syntax which may be different to other assertion libraries you may have used.

### TUnit
This is a helper package to combine the above 3 packages. If you just want a standard test app where you can write, run and assert tests, just install this!

### TUnit.Playwright
This provides you base classes, similarly to Microsoft.Playwright.NUnit or Microsoft.Playwright.MSTest, to automatically create and dispose of Playwright objects in tests, to make it easier for you to write tests without worrying about lifecycles or disposing. The base classes are named the same as the other libraries: `PageTest`,  `ContextTest`, `BrowserTest`, and `PlaywrightTest`.

## Features

- Native AOT / Trimmed Single File application support
- Source generated tests
- Property injection
- Full async support
- Parallel by default, with mechanisms to:
    - Run specific tests completely on their own
    - Run specific tests not in parallel with other specific tests
    - Limit the parallel limit on a per-test, class or assembly level
- Tests can depend on other tests to form chains, useful for if one test depends on state from another action. While not recommended for unit tests, this can be useful in integration testing where state matters
- Easy to read assertions - though you're also free to use whichever assertion library you like
- Injectable test data via classes, methods, compile-time args, or matrices
- Hooks before and after: 
    - TestDiscover
    - TestSession
    - Assembly
    - Class
    - Test
- Designed to avoid common pitfalls such as leaky test states
- Dependency injection support ([See here](https://thomhurst.github.io/TUnit/docs/tutorial-extras/class-constructors))
- Ability to view and interrogate metadata and results from various assembly/class/test context objects

## Installation

`dotnet add package TUnit --prerelease`

## Example test

```csharp
    private static readonly TimeOnly Midnight = TimeOnly.FromTimeSpan(TimeSpan.Zero);
    private static readonly TimeOnly Noon = TimeOnly.FromTimeSpan(TimeSpan.FromHours(12));
    
    [Test]
    public async Task IsMorning()
    {
        var time = GetTime();

        await Assert.That(time).IsAfterOrEqualTo(Midnight)
            .And.IsBefore(Noon);
    }
```

or with more complex test orchestration needs

```csharp
    [Before(Class)]
    public static async Task ClearDatabase(ClassHookContext context) { ... }

    [After(Class)]
    public static async Task AssertDatabaseIsAsExpected(ClassHookContext context) { ... }

    [Before(Test)]
    public async Task CreatePlaywrightBrowser(TestContext context) { ... }

    [After(Test)]
    public async Task DisposePlaywrightBrowser(TestContext context) { ... }

    [Retry(3)]
    [Test, DisplayName("Register an account")]
    [MethodData(nameof(GetAuthDetails))]
    public async Task Register(string username, string password) { ... }

    [Repeat(5)]
    [Test, DependsOn(nameof(Register))]
    [MethodData(nameof(GetAuthDetails))]
    public async Task Login(string username, string password) { ... }

    [Test, DependsOn(nameof(Login), [typeof(string), typeof(string)])]
    [MethodData(nameof(GetAuthDetails))]
    public async Task DeleteAccount(string username, string password) { ... }

    [Category("Downloads")]
    [Timeout(300_000)]
    [Test, NotInParallel(Order = 1)]
    public async Task DownloadFile1() { ... }

    [Category("Downloads")]
    [Timeout(300_000)]
    [Test, NotInParallel(Order = 2)]
    public async Task DownloadFile2() { ... }

    [Repeat(10)]
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [DisplayName("Go to the page numbered $page")]
    public async Task GoToPage(int page) { ... }

    [Category("Cookies")]
    [Test, Skip("Not yet built!")]
    public async Task CheckCookies() { ... }

    [Test, Explicit, WindowsOnlyTest, RetryHttpServiceUnavailable(5)]
    [Property("Some Key", "Some Value")]
    public async Task Ping() { ... }

    [Test]
    [ParallelLimit<LoadTestParallelLimit>]
    [Repeat(1000)]
    public async Task LoadHomepage() { ... }

    public static IEnumerable<(string Username, string Password)> GetAuthDetails()
    {
        yield return ("user1", "password1");
        yield return ("user2", "password2");
        yield return ("user3", "password3");
    }

    public class WindowsOnlyTestAttribute : SkipAttribute
    {
        public WindowsOnlyTestAttribute() : base("Windows only test")
        {
        }

        public override Task<bool> ShouldSkip(TestContext testContext)
        {
            return Task.FromResult(!OperatingSystem.IsWindows());
        }
    }

    public class RetryHttpServiceUnavailableAttribute : RetryAttribute
    {
        public RetryHttpServiceUnavailableAttribute(int times) : base(times)
        {
        }

        public override Task<bool> ShouldRetry(TestInformation testInformation, Exception exception, int currentRetryCount)
        {
            return Task.FromResult(exception is HttpRequestException { StatusCode: HttpStatusCode.ServiceUnavailable });
        }
    }

    public class LoadTestParallelLimit : IParallelLimit
    {
        public int Limit => 50;
    }
```

## Motivations

TUnit is inspired by NUnit and xUnit - two of the most popular testing frameworks for .NET.

It aims to build upon the useful features of both while trying to address any pain points that they may have.

[Read more here](https://thomhurst.github.io/TUnit/docs/comparison/framework-differences)

## Prerelease

You'll notice that version 1.0 isn't out yet. While this framework is mostly feature complete, I'm waiting for a few things:

- Full Rider support for all features
- Full VS support for all features
- Open to feedback on existing features
- Open to ideas on new features

As such, the API may change. I'll try to limit this but it's a possibility.

## Benchmark

### Scenario: Building the test project

#### macos-latest

```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7.1 (23H222) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method       | Mean     | Error    | StdDev    | Median   |
|------------- |---------:|---------:|----------:|---------:|
| Build_TUnit  | 957.9 ms | 19.11 ms |  49.68 ms | 942.0 ms |
| Build_NUnit  | 949.3 ms | 38.30 ms | 109.27 ms | 910.4 ms |
| Build_xUnit  | 916.5 ms | 18.38 ms |  53.32 ms | 916.4 ms |
| Build_MSTest | 915.3 ms | 18.24 ms |  49.94 ms | 907.1 ms |



#### ubuntu-latest

```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method       | Mean    | Error    | StdDev   |
|------------- |--------:|---------:|---------:|
| Build_TUnit  | 1.796 s | 0.0345 s | 0.0354 s |
| Build_NUnit  | 1.474 s | 0.0255 s | 0.0226 s |
| Build_xUnit  | 1.483 s | 0.0234 s | 0.0195 s |
| Build_MSTest | 1.543 s | 0.0259 s | 0.0230 s |



#### windows-latest

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2849) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method       | Mean    | Error    | StdDev   |
|------------- |--------:|---------:|---------:|
| Build_TUnit  | 1.841 s | 0.0312 s | 0.0276 s |
| Build_NUnit  | 1.525 s | 0.0204 s | 0.0191 s |
| Build_xUnit  | 1.535 s | 0.0208 s | 0.0195 s |
| Build_MSTest | 1.583 s | 0.0175 s | 0.0146 s |


### Scenario: A single test that completes instantly (including spawning a new process and initialising the test framework)

#### macos-latest

```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7 (23H124) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean      | Error     | StdDev    |
|---------- |----------:|----------:|----------:|
| TUnit_AOT |  77.27 ms |  0.222 ms |  0.185 ms |
| TUnit     | 460.58 ms |  4.037 ms |  3.776 ms |
| NUnit     | 697.60 ms |  4.334 ms |  5.785 ms |
| xUnit     | 685.56 ms |  4.719 ms |  3.684 ms |
| MSTest    | 635.80 ms | 12.393 ms | 16.544 ms |



#### ubuntu-latest

```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    30.46 ms |  0.608 ms |  0.981 ms |
| TUnit     |   836.53 ms | 16.008 ms | 18.435 ms |
| NUnit     | 1,310.56 ms | 12.780 ms | 11.955 ms |
| xUnit     | 1,290.16 ms | 13.392 ms | 12.526 ms |
| MSTest    | 1,168.66 ms | 13.034 ms | 11.554 ms |



#### windows-latest

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2849) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    77.92 ms |  0.177 ms |  0.148 ms |
| TUnit     |   823.19 ms | 16.281 ms | 22.824 ms |
| NUnit     | 1,288.16 ms | 10.296 ms |  9.127 ms |
| xUnit     | 1,261.28 ms |  6.821 ms |  5.695 ms |
| MSTest    | 1,150.37 ms | 10.706 ms | 10.015 ms |


### Scenario: A test that takes 50ms to execute, repeated 100 times (including spawning a new process and initialising the test framework)

#### macos-latest

```

BenchmarkDotNet v0.14.0, macOS Sonoma 14.7 (23H124) [Darwin 23.6.0]
Apple M1 (Virtual), 1 CPU, 3 logical and 3 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), Arm64 RyuJIT AdvSIMD

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean        | Error     | StdDev    |
|---------- |------------:|----------:|----------:|
| TUnit_AOT |    256.7 ms |   9.91 ms |  29.07 ms |
| TUnit     |    700.7 ms |  31.40 ms |  92.57 ms |
| NUnit     | 14,283.6 ms | 284.04 ms | 540.42 ms |
| xUnit     | 14,495.3 ms | 288.48 ms | 614.79 ms |
| MSTest    | 14,330.9 ms | 285.91 ms | 537.02 ms |



#### ubuntu-latest

```

BenchmarkDotNet v0.14.0, Ubuntu 22.04.5 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean       | Error    | StdDev   |
|---------- |-----------:|---------:|---------:|
| TUnit_AOT |   100.6 ms |  1.93 ms |  2.90 ms |
| TUnit     |   900.7 ms | 17.97 ms | 29.01 ms |
| NUnit     | 6,474.1 ms | 20.49 ms | 19.17 ms |
| xUnit     | 6,458.3 ms | 15.11 ms | 14.13 ms |
| MSTest    | 6,451.9 ms | 23.17 ms | 21.67 ms |



#### windows-latest

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2849) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  .NET 9.0 : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=.NET 9.0  Runtime=.NET 9.0  

```
| Method    | Mean       | Error     | StdDev    |
|---------- |-----------:|----------:|----------:|
| TUnit_AOT |   126.5 ms |   3.21 ms |   9.46 ms |
| TUnit     |   950.1 ms |  18.91 ms |  33.61 ms |
| NUnit     | 8,869.8 ms | 174.45 ms | 238.79 ms |
| xUnit     | 8,830.6 ms | 168.81 ms | 173.36 ms |
| MSTest    | 8,718.4 ms | 171.77 ms | 287.00 ms |



