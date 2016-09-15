using HomeBuilder.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HomeBuilder.History
{
    public class Note : MonoBehaviour
    {
        public delegate void OnTriggered(Note note);
        public static OnTriggered onTriggered;

        public Image image;
        public Text description;
        public Text square;

        int id;
        Appartment app;

        public void SetId(int id)
        {
            this.id = id;
        }

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetDescription(string text)
        {
            description.text = text;
        }

        public void SetSquare(float number)
        {
            square.text = "" + number;
        }

        public void SetAppartment(Appartment app)
        {
            this.app = app;

            SetSquare(app.GetSquare());
        } 

        public Appartment GetAppartment()
        {
            return app;
        }

        public int GetId()
        {
            return id;
        }

        public Sprite GetImage()
        {
            return image.sprite;
        }

        public string GetDescription()
        {
            return description.text;
        }

        public int GetSquare()
        {
            int res = 0;
            int.TryParse(square.text, out res);
            return res;
        }

        public void OnClick()
        {
            if (onTriggered != null)
            {
                onTriggered(this);
            } 
        }

    }
}
