using System.Collections;

namespace HomeBuilder
{
    public class Assets
    {

        static private Assets instance = null;

        readonly public PrefabAssets prefabs;
        readonly public SpriteAssets sprites;

        private Assets()
        {
            prefabs = new PrefabAssets();
            sprites = new SpriteAssets();
        }

        static public Assets GetInstance()
        {
            if (instance == null)
            {
                instance = new Assets();
            }

            return instance;
        }

        public class PrefabAssets
        {

            public string cube      = "Prefabs/cube_prefab";
            public string note      = "Prefabs/note_prefab";
            public string module    = "Prefabs/module_prefab";

        }

        public class SpriteAssets
        {
            public string styleModern   = "Sprites/style_modern";
            public string styleClassic  = "Sprites/style_classic";
        }

    }
}