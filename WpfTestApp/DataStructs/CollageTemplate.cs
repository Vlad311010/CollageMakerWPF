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

    struct ContainerData
    {
        public string MaskSource;
        public double Left;
        public double Top;
        public double Width;
        public double Height;
    }

    internal class CollageTemplate
    {
        public int Images;
        public ContainerData[] Containers;

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
