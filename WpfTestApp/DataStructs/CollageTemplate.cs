using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace WpfTestApp.DataStructs
{

    public struct ContainerData
    {
        public string MaskSource;
        
        // percentage values
        public double Left;
        public double Top;
        public double Width;
        public double Height;
        public double MaskLeft;
        public double MaskTop;
}

    public class CollageTemplate
    {
        public string Name { get; set; }
        public int Images { get; set; }
        public ContainerData[] Containers { get; set; }

        public static CollageTemplate ReadFromYaml(string yamlFilePath)
        {
            string yaml = File.ReadAllText(yamlFilePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            //yml contains a string containing your YAML
            var template = deserializer.Deserialize<CollageTemplate>(yaml);
            return template;
        }
    }
}
