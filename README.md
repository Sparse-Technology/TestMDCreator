
# Test Markdown Converter

## Description

**tmdc** is a command-line application designed to process input files and generate output based on specified parameters. This README provides information on how to use the application and its available options.

## Usage

```bash
tmdc [options]
```

### Options

- `--debug` (Default: Warning): Set output to debug messages.

- `-f, --folder` (Default: docs): Input folder to be processed.

- `-n, --name` (Default: test.md): Output file name.

- `--force` (Default: false): Force overwrite of the output file.

- `--test-xlsx-file`: Test xlsx file reader. Set to the file name.

- `--help`: Display the help screen.

- `--version`: Display version information.

## Examples

```bash
# Run with debug output
tmdc --debug

# Process files from a specific folder
tmdc -f input_folder

# Specify the output file name
tmdc -n output_file.md

# Force overwrite of the output file
tmdc --force

# Test xlsx file reader with a specific file
tmdc --test-xlsx-file test.xlsx
```

## License

This application is licensed under the [Apache License, Version 2.0](https://www.apache.org/licenses/LICENSE-2.0). See the [LICENSE](LICENSE) file for details.

## Contact

For any questions or support, please contact [iegursoy28@gmail.com](mailto:iegursoy28@gmail.com).