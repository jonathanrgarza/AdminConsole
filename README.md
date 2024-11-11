# Admin Console

Admin Console is a Windows-only .NET 8.0 console application designed to launch a new administrator console host. The application can take optional switch arguments to specify the type of console host to launch or detect the running console host and launch the appropriate console host.

## Features

- Detects the current console host and determines if it is within Windows Terminal.
- Launches a new Windows Terminal as an administrator.
- Launches a new console host (Command Prompt, PowerShell, or Windows PowerShell) as an administrator.
- Detects if user denies User Account Control (UAC) prompt for admin rights and outputs a console message

## Requirements

- Windows operating system
- .NET 8.0 SDK

## Usage

Run the application from the command line with optional switch arguments to specify the console host to launch. Ideally the release built of the application is added to the PATH environment variable for convenience.

### Command-Line Options

- `--help`: Displays usage information.
- `--version`: Displays the application version.
- `--wt`: Launches a new Windows Terminal as an administrator.
- `--cmd`: Launches a new Command Prompt (cmd) as an administrator.
- `--powershell`: Launches a new PowerShell Core (pwsh) as an administrator.
- `--winpowershell`: Launches a new Windows PowerShell as an administrator.

The `--wt` option caused be used with `--cmd`, `--powershell`, or `--winpowershell` to open one of these console host in Windows Terminal. Otherwise, `--powershell` is the default option if PowerShell 7.0+ is installed, otherwise `--winpowershell` is the default.

### Examples

Launch a new console host using the detected console host as an administrator:
```sh
admin
```

Launch a new Windows Terminal using the Command Prompt profile as an administrator:
```sh
admin --wt --cmd
```

Launch a new Command Prompt (cmd) as an administrator:
```sh
admin --cmd
```

Launch a new PowerShell Core (pwsh) as an administrator:
```sh
admin --powershell
```

Launch a new Windows PowerShell as an administrator:
```sh
admin --winpowershell
```

## Build

To build the Admin Console application, follow these steps:

1. Ensure you have the .NET 8.0 SDK installed on your machine.
2. Open a command prompt or terminal window.
3. Navigate to the project directory.
5. Build the project:
    ```sh
    dotnet build
    ```

The build output will be located in the `bin` directory within your project folder.

## License

This project is privately owned. Users are permitted to clone, build and use the built project for personal purposes. Redistribution, modification, or commercial use is not allowed without explicit permission from the owner.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

For any inquiries, please contact the owner.