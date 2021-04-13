This project is created using the WorkerService template and can be installed as a Windows Service.

Make sure AppSettings are specified in appsettings.json, namely:
URL: https://www.psx.com.pk/market-summary/
OutputDirectory: Where/You/Want/To/Store (can be relative or absolute)
IntervalBetween: 300 (In seconds)

First publish the project using a FolderProfile, then install the exe from the service control.

If using PowerShell, sc is reserved for set content, so sc.exe must be used.

Example:
sc.exe create k173669_Q1 binpath= Path/to/publish/k173669_Q1.exe

After starting the service, you can observe the running state by checking the logs at
Documents/LogFile.txt

You should see this: [SC] CreateService SUCCESS