# MHDataParser

MHDataParser is a parser for various data files used by Marvel Heroes. It loads data files used by the client and can output some or all of them as human-readable TSV/JSON files (depending on the format).

This tool has been tested mainly with version `1.52.0.1700`.

## Usage

1. Make sure you have [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed.

2. Build the project with Visual Studio or any other tool you prefer.

3. Copy all data files from `%MarvelHeroesGameDirectory%\Data\Game` to `%MHDataParserDirectory%\Data`.

4. Run the parser and wait for it to process the data.

5. Enter one of the available commands to export the data you need, or `help` to see a list of available commands.

## Commands

| Command             | Description                                                                                     |
| ------------------- | ----------------------------------------------------------------------------------------------- |
| help                | Shows a list of available commands.                                                             |
| extract-pak         | Extracts all entries and data from loaded pak files.                                            |
| extract-pak-entries | Extracts all entries from loaded pak files.                                                     |
| extract-pak-data    | Extracts all data from loaded pak files.                                                        |
| export-all-data     | Exports all parsed data.                                                                        |
| export-calligraphy  | Exports parsed Calligraphy data (directories, curves, asset types, blueprints, and prototypes). |
| export-directories  | Exports parsed directories as TSV.                                                              |
| export-curves       | Exports parsed curves as TSV.                                                                   |
| export-asset-types  | Exports parsed asset types as JSON.                                                             |
| export-blueprints   | Exports parsed blueprints as JSON.                                                              |
| export-prototypes   | Exports parsed prototypes as JSON.                                                              |
| export-resources    | Exports parsed resource data (cells, districts, encounters, props, prop sets, and UIs.          |
| export-cells        | Exports parsed cells as JSON.                                                                   |
| export-districts    | Exports parsed districts as JSON.                                                               |
| export-encounters   | Exports parsed encounters as JSON.                                                              |
| export-props        | Exports parsed props as JSON.                                                                   |
| export-prop-sets    | Exports parsed prop sets as JSON.                                                               |
| export-uis          | Exports parsed UIs as JSON.                                                                     |
| export-locales      | Exports parsed locales with all of their strings as JSON.                                       |
