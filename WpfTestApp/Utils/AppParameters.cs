using WpfTestApp.DataStructs;

namespace WpfTestApp.Utils
{
    internal class AppParameters
    {
        private AppParameters()
        {
            EditorParameters = new EditorParameters();
        }

        public static AppParameters Instance => _instance ??= new AppParameters();

        private static AppParameters? _instance;

        public EditorParameters EditorParameters { get; private set; }
        public const string TEMPLATES_FOLDER = ".\\Templates";
        public const string MASKS_FOLDER = ".\\Masks";
    }
}
