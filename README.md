# acc104ua

**acc104ua** is a tool that mass-exports Gas and Gas Delivery [104.ua](104.ua) bills and statements.

## Prerequisites

- .NET 5 SDK
- Visual Studio / Visual Studio Code

## Usage

Export since `start-date` to last month inclusive:
```
acc104ua.exe --export <username> <password> <start-date>
```

Export since `start-date` to `end-date` inclusive:
```
acc104ua.exe --export <username> <password> <start-date> <end-date>
```

Export last month:
```
acc104ua.exe --export <username> <password>
```

Show current statement:
```
acc104ua.exe <username> <password>
```


NOTES:
- 2FA not supported
- export is performed into `bin` folder
- dates are inclusive
- dates format preferably is `yyyy-MM-dd`, but parsing depends on your local settings
- dates should be in the following order: `start-date`, then `end-date`

## Output

1. Raw data. TXT files of what APIs give back.
2. JSON. JSON files of cleaned-up data.
3. CSVs. Data grouped by account ID.