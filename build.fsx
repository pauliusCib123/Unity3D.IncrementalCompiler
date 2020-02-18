﻿#I @"packages/FAKE/tools"
#I @"packages/FAKE.BuildLib/lib/net451"
#r "FakeLib.dll"
#r "BuildLib.dll"

open Fake
open BuildLib

setBuildParam "MSBuild" @"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin"

let solution = initSolution "./IncrementalCompiler.sln" "Release" [ ]

Target "Clean" <| fun _ -> cleanBin

Target "Restore" <| fun _ -> restoreNugetPackages solution

Target "Build" <| fun _ -> buildSolution solution

Target "Export" (fun _ -> 
    for (target, target2, useHarmony) in [("2018", "Unity5", true); ("2019.3", "2019.3", false)] do
        let targetDir = binDir @@ target
        let compilerDir = targetDir @@ "Compiler"
        let editorDir = targetDir @@ "Assets" @@ "CSharp vNext Support" @@ "Editor"
        let pluginsDir = targetDir @@ "Assets" @@ "Plugins" @@ "Incremental Compiler"
        CreateDir targetDir
        CreateDir compilerDir
        CreateDir editorDir
        CreateDir pluginsDir

        "./GenerationAttributes/bin/Release/GenerationAttributes.dll" |> CopyFile pluginsDir
        "./GenerationAttributes/bin/Release/GenerationAttributes.xml" |> CopyFile pluginsDir
        "./Macros/bin/Release/Macros.dll" |> CopyFile pluginsDir
        "./Macros/bin/Release/Macros.xml" |> CopyFile pluginsDir
        "./core/UnityPackage/Assets/Editor/CompilerSettings.cs" |> CopyFile editorDir
        if useHarmony then "./tools/0Harmony.dll" |> CopyFile editorDir
        "./core/IncrementalCompiler/IncrementalCompiler.xml" |> CopyFile compilerDir
        "./extra/CompilerPlugin." + target2 + "/bin/Release/Unity.PureCSharpTests.dll" |> CopyFile (editorDir @@ "CSharpVNextSupport.dll")
        "./extra/UniversalCompiler/bin/Release/UniversalCompiler.exe" |> CopyFile compilerDir
        "./extra/UniversalCompiler/UniversalCompiler.xml" |> CopyFile compilerDir
        "./tools/pdb2mdb/pdb2mdb.exe" |> CopyFile compilerDir

        let dir = System.IO.DirectoryInfo("./core/IncrementalCompiler/bin/Release/")
        filesInDir dir |> Array.iter (fun f -> f.FullName |> CopyFile compilerDir)
)

Target "Help" <| fun _ -> 
    showUsage solution (fun name -> 
        if name = "package" then Some("Build package", "sign")
        else None)

"Clean"
  ==> "Restore"
  ==> "Build"

"Build" ==> "Export"

RunTargetOrDefault "Help"
