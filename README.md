# BoneLab Level Utils
Utilities for bonelab levels. Player health options and waypoints!<br/>
<br/>
<br/>
Level Utils loads customized level properties from "\<Game directory\>\\UserData\\LevelUtils\\\<level pallet\>\\\<level barcode\>.json".<br/>
Options can be set using the in-game BoneMenu as well.<br/>
Comes with JSON for [LabWorks](https://mod.io/g/bonelab/m/boneworks) that disables level reloading on death for the currently released levels (01 through 08). No more having to replay the entire level after dying!<br/>
<br/>
<br/>
# Setting up custom level properties
In-game, load the level you wish to add properties for.<br/>
Open up Menu->Preferences/Options->BoneMenu->Level Utils<br/>
Set the values as you prefer.<br/>
For waypoints, physically position yourself at the desired location, go to Waypoints->New Waypoint, and then select a color.<br/>
Once created, a waypoint's name, color, and position can be modified inside the level's JSON file (see above for location).<br/>

## Compilation

To compile this mod, create a file named `LevelUtils.csproj.user` in the
project. Copy this text into the file, replacing the path in `BONELAB_PATH` with
the path to your BONELAB installation.
```
<?xml version="1.0" encoding="utf-8"?>
<Project>
    <PropertyGroup>
        <BONELAB_PATH>C:\Program Files (x86)\Steam\steamapps\common\BONELAB</BONELAB_PATH>
    </PropertyGroup>
</Project>
```
<br/>
<br/>

