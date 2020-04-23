# DuplicateFinder


## Installation

Requires [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) to run.

### Run
- Clone repo
- Save folder of files to be traversed somewhere locally and note the path as the program will ask for it. Alternatively, place the files in C:\Temp as the program will use that as default.

```sh
cd DuplicateFinder
dotnet run
```

### Test
```sh
cd DuplicateFinder
dotnet test
```

## Assumptions
- All business logic assumptions are documented in unit test names
- No need to restrict the search to just image file types
- Speed is not a major issue as this will not be run very frequently

### If speed was an issue
Could change the solution to run some of the loops in parallel. Currently the way we are modifying the dictionary within the loop would not be safe to do in parallel. 
Changes to allow it to be run in parallel would complicate the solution and the added complexity would need to be weighed up with the savings in speed.

## Notes
- Timeboxed the work on this
- In the interest of time I implemented some unit tests to demonstrate my skill and style but left some not implemented but detailed all of the test cases in the test names

## What I could have done with more time
- Implemented unfinished unit tests
- To increase reliability and processing power for large file sets, could implement batching
- Could extend to actually output the split files into separate folders either making assumptions on file name to use or prompt user to choose which one to keep
