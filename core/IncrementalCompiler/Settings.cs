﻿using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace IncrementalCompiler
{
    public class Settings
    {
        public PrebuiltOutputReuseType PrebuiltOutputReuse;

        public static Settings Default = new Settings
        {
            PrebuiltOutputReuse = PrebuiltOutputReuseType.WhenNoChange,
        };

        public static Settings? Load()
        {
            var fileName = Path.ChangeExtension(Assembly.GetEntryAssembly()?.Location, ".xml");
            if (fileName == null || File.Exists(fileName) == false)
                return null;

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Load(stream);
            }
        }

        public static Settings Load(Stream stream)
        {
            // To reduce start-up time, do manual parsing instead of using XmlSerializer
            var xdoc = XDocument.Load(stream).Element("Settings");
            return new Settings
            {
                PrebuiltOutputReuse = (PrebuiltOutputReuseType)Enum.Parse(typeof(PrebuiltOutputReuseType), xdoc.Element("PrebuiltOutputReuse").Value),
            };
        }
    }
}
