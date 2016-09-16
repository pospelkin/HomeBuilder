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

            readonly public string cube      = "Prefabs/cube_prefab";
            readonly public string note      = "Prefabs/note_prefab";
            readonly public string module    = "Prefabs/module_prefab";
            readonly public string moduleS   = "Prefabs/module_small_prefab"; 

        }

        public class SpriteAssets
        {
            readonly public string styleModern   = "Sprites/style_modern";
            readonly public string styleClassic  = "Sprites/style_classic";
            readonly public string styleOld      = "Sprites/style_old";
        }

    }
}