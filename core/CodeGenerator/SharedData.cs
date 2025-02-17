﻿using System.IO;

public static class SharedData {
  public const string GeneratedFolder = "generated-by-compiler";

  public static string CompileTimesFileName(string assemblyName) {
    return Path.Combine(GeneratedFolder, $"{assemblyName}.compile-times.txt");
  }
}

// fix for C#9 records
// https://stackoverflow.com/questions/62648189/testing-c-sharp-9-0-in-vs2019-cs0518-isexternalinit-is-not-defined-or-imported
namespace System.Runtime.CompilerServices {
  public class IsExternalInit{}
}
