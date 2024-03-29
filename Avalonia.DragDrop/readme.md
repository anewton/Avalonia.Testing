AvaloniaTest.DragDrop
---
Sample applications to demonstrate custom Drag+Drop controls 
- DropEnabledItemsControl
- DraggableControl

### WorkingVersion.csproj
- Sample using Avalonia Version 0.10.22
- Working for mousepad, stylus, or touch as pointer for drag/drop on Surface Pro 8.

### ReleaseVersion.csproj
- Sample using Avalonia Version 11.0.4
- BUG: Unable to use touch or stylus as pointer for drag/drop in latest version on Surface Pro 8. Mousepad does allow drag/drop.

### Build/Run
- Set either of the projects named "*.Desktop" as the startup project in Visual Studio. Then Debug.
- Otherwise use the one of the following in Visual Studio Code from the root folder:
    ```
    > dotnet run --project ./WorkingVersion/WorkingVersion.Desktop/WorkingVersion.Desktop.csproj
    ```
    OR

    ```
    > dotnet run --project ./ReleaseVersion/ReleaseVersion.Desktop/ReleaseVersion.Desktop.csproj
    ```

### Publishing
- To publish both projects, execute the publish.sh script using Git Bash in the root folder
- Creates a releases folder with the separate builds for each project
