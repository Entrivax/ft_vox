# ft_vox

ft_vox is a 42 introduction project to voxel engine, inspired by Minecraft, in which we have to create a randomly generated world.

## Build

Tested with Visual Studio 2017 and Mono 4.x.

### Visual Studio 2017

```bash
# Clone the project
git clone https://github.com/Entrivax/ft_vox
```
Open the .sln file with Visual Studio 2017.

You should restore the NuGet packages before building solution.

You can now build the solution in Visual Studio 2017 (default shortcut: Ctrl+Shift+B)

### Mono 4.x

```bash
# Clone the project
git clone https://github.com/Entrivax/ft_vox
# Navigate into the project directory
cd ft_vox
# Restore NuGet packages
nuget restore
# Build the solution
make all
```

You can even directly start the program with:
```bash
make start
```
