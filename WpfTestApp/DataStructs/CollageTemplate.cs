using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTestApp.DataStructs
{

    struct ContainerData
    {
        public string MaskSource;
        public int X;
        public int Y;
    }

    internal class CollageTemplate
    {
        public int Images;
        public ContainerData[] Containers;

    }
}
