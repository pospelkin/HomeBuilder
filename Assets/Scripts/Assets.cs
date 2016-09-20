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
            readonly public string planRoom  = "Prefabs/plan_room_prefab";

        }

        public class SpriteAssets
        {
            readonly public string styleModern = "Sprites/style_modern";
            readonly public string styleClassic = "Sprites/style_classic";
            readonly public string styleOld = "Sprites/style_old";

            readonly public string kitchenStyleModern = "Sprites/Kitchen/style_modern";
            readonly public string kitchenStyleClassic = "Sprites/Kitchen/style_classic";
            readonly public string kitchenStyleOld = "Sprites/Kitchen/style_old";

            readonly public string bedroomStyleModern = "Sprites/Bedroom/style_modern";
            readonly public string bedroomStyleClassic = "Sprites/Bedroom/style_classic";
            readonly public string bedroomStyleOld = "Sprites/Bedroom/style_old";

            readonly public string bathroomStyleModern = "Sprites/Bathroom/style_modern";
            readonly public string bathroomStyleClassic = "Sprites/Bathroom/style_classic";
            readonly public string bathroomStyleOld = "Sprites/Bathroom/style_old";

            readonly public string hallStyleModern = "Sprites/Hall/style_modern";
            readonly public string hallStyleClassic = "Sprites/Hall/style_classic";
            readonly public string hallStyleOld = "Sprites/Hall/style_old";
        }

    }
}