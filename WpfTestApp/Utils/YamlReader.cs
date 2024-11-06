using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using WpfTestApp.DataStructs;

namespace WpfTestApp.Utils
{
    internal class YamlReader
    {

        public static void Foo()
        {

            string file = "C:\\Users\\Vlad\\Desktop\\testApp\\WpfTestApp\\WpfTestApp\\Templates\\test.yaml";
            string yaml = File.ReadAllText(file);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  
                .Build();

            //yml contains a string containing your YAML
            var p = deserializer.Deserialize<CollageTemplate>(yaml);
            Console.WriteLine($"{p.Images}");
        }
    }

}
