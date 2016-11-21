using UnityEngine;
using System.Collections;
using HomeBuilder.Core;
using HomeBuilder.Designing;
using sharpPDF;
using sharpPDF.Enumerators;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HomeBuilder.Share
{

    public class PDFComposer : MonoBehaviour
    {

        public Camera modelCam;
        public Camera planCam;
        public RawImage result;
        public GameObject textToHide;

        public GCameraController cameraController;
        public EventSystem eventSystem;
        public Constructor constructor;

        public string filepath;

        public void DownloadPDF(Appartment app)
        {
            StartCoroutine(CreatePDF(app));
        }

        public IEnumerator CreatePDF(Appartment app)
        {
            cameraController.enabled = false;
            eventSystem.enabled      = false;

            constructor.GetCurrentEditor().ClearAllSigns();

            GetComponent<CanvasGroup>().alpha = 0;

            yield return new WaitForSeconds(1f);

            pdfDocument myDoc = new pdfDocument(app.GetName(), "HomeBuilder", false);
            pdfPage page = myDoc.addPage();

            page.addText(app.GetName(), 10, 730, predefinedFont.csTimesBold, 30, new pdfColor(predefinedColor.csBlack));

            yield return StartCoroutine(PrepareModel());
            yield return StartCoroutine(AddScreenshot(page, 612 - 160, 792 - 10, 150, planCam));

            page.addText(
                "An appartment " + Format(app.GetSquare()) + "m2" + (app.GetFloors() == 2 ? " with 2 floors." : "."),
                5, 700, predefinedFont.csTimes, 12, new pdfColor(predefinedColor.csBlack));

            int yPos = 630;
            /*Add Columns to a grid*/
            for (int i = 0; i < app.GetFloors(); i++)
            {
                ModuleInfo[] modules = app.GetModules(i);

                if (yPos - 210 < 0)
                {
                    page = myDoc.addPage();
                    yPos = 730;
                }

                page.addText(
                    "Floor " + i,
                    5, yPos, predefinedFont.csTimesBold, 20, new pdfColor(predefinedColor.csBlack));

                yPos -= 10;
                
                yield return StartCoroutine(PrepareFloor(i));
                yield return StartCoroutine(AddScreenshot(page, 10, yPos, 200, modelCam, true));

                yPos -= 200;

                /*Table's creation*/
                pdfTable myTable = new pdfTable();
                //Set table's border
                myTable.borderSize = 1;
                myTable.borderColor = new pdfColor(predefinedColor.csBlack);

                myTable.tableHeader.addColumn(new pdfTableColumn("Name", predefinedAlignment.csCenter, 120));
                myTable.tableHeader.addColumn(new pdfTableColumn("Style", predefinedAlignment.csCenter, 120));
                myTable.tableHeader.addColumn(new pdfTableColumn("Square", predefinedAlignment.csCenter, 120));

                for (int j = 0; j < modules.Length; j++)
                {
                    pdfTableRow myRow = myTable.createRow();
                    myRow[0].columnValue = modules[j].GetName();
                    myRow[1].columnValue = modules[j].GetStyle();
                    myRow[2].columnValue = "" + Format(modules[j].GetSquare()) + "m2";

                    myTable.addRow(myRow);
                }

                /*Set Header's Style*/
                myTable.tableHeaderStyle = new pdfTableRowStyle(predefinedFont.csCourierBoldOblique, 12, new pdfColor(predefinedColor.csWhite), new pdfColor(predefinedColor.csGray));
                /*Set Row's Style*/
                myTable.rowStyle = new pdfTableRowStyle(predefinedFont.csTimes, 10, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csWhite));
                /*Set Alternate Row's Style*/
                myTable.alternateRowStyle = new pdfTableRowStyle(predefinedFont.csTimes, 10, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csLightGray));
                /*Set Cellpadding*/
                myTable.cellpadding = 10;
                yPos -= 20;

                if (yPos - (modules.Length + 1) * 50 < 0)
                {
                    page = myDoc.addPage();
                    yPos = 730;
                }

                /*Put the table on the page object*/
                page.addTable(myTable, 5, yPos);

                yPos -= (modules.Length + 1) * 50;

            }

            //StartCoroutine(PostScreenshot(planCam));

            Debug.Log("WRITE PDF");
            filepath = myDoc.createPDF(app.GetName() + ".pdf");
            Debug.Log("PDF SAVED");

            yield return StartCoroutine(PrepareEnd());

            GetComponent<CanvasGroup>().alpha = 1;

            cameraController.enabled = true;
            eventSystem.enabled      = true;
        }

        string Format(float value)
        {
            float newValue = Mathf.RoundToInt(value*10f)/10f;
            return "" + newValue;
        }

        IEnumerator PrepareModel()
        {
            ((SphereCamera)cameraController.gCamera).SetPhi(-35);
            ((SphereCamera)cameraController.gCamera).SetTita(65);
            ((SphereCamera)cameraController.gCamera).SetRadius(30);
            ((SphereCamera)cameraController.gCamera).UpdatePosition();

            yield return new WaitForEndOfFrame();
        }

        IEnumerator PrepareEnd()
        {
            textToHide.SetActive(true);

            ((SphereCamera)cameraController.gCamera).SetPhi(-35);
            ((SphereCamera)cameraController.gCamera).SetTita(65);
            ((SphereCamera)cameraController.gCamera).UpdatePosition();

            constructor.OpenModel();

            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator PrepareFloor(int i)
        {
            textToHide.SetActive(false);

            constructor.OpenPlan();
            constructor.OpenFloor(i);

            yield return new WaitForSeconds(0.5f);
        }

        IEnumerator AddScreenshot(pdfPage page, int x, int y, int size, Camera camera, bool flag = false)
        {
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            camera.targetTexture = rt;


            int height = Mathf.RoundToInt(rt.height * 0.75f),
                width  = rt.width - 20;
            if (!flag)
            {
                height = Mathf.RoundToInt(rt.height * 0.75f);
                width  = height;
            }

            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            RenderTexture.active = rt;

            yield return new WaitForEndOfFrame();

            camera.Render();
            screenShot.ReadPixels(new Rect(rt.width / 2 - width / 2, rt.height / 2 - height / 2, width, height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);

            screenShot.Apply();

            Texture2D scaled = ScaleTexture(screenShot,
                Mathf.RoundToInt(screenShot.width*(size / (float) screenShot.height)), size);

            page.addImage(scaled.EncodeToJPG(), x, y - scaled.height, scaled.width, scaled.height);
        }

        IEnumerator PostScreenshot(Camera camera)
        {
            Debug.Log("Post Screenshot");
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
            camera.targetTexture = rt;

            int width  = Mathf.RoundToInt(rt.width * 0.75f),
                height = Mathf.RoundToInt(rt.height * 0.75f);

            Texture2D screenShot = new Texture2D(width, height);
            Debug.Log("Textures Created");
            RenderTexture.active = rt;

            yield return new WaitForEndOfFrame();

            camera.Render();

            Debug.Log("Rendered");
            screenShot.ReadPixels(new Rect(rt.width/2 - width/2, rt.height/2 - height/2, width, height), 0, 0);
            Debug.Log("Pixels Read");
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            Debug.Log("Destroyed RT");

            screenShot.Apply();

            result.texture = ScaleTexture(screenShot, Mathf.RoundToInt(screenShot.width * (500 / (float) screenShot.height)), 500);
            result.SetNativeSize();
            Debug.Log("Texture assigned.");
        }

        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    float x = j / (float) result.width * (float) source.width;
                    float y = i / (float) result.height * (float) source.height;
                    Color newColor = source.GetPixel(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
                    result.SetPixel(j, i, newColor);
                }
            }
            result.Apply();
            return result;
        }

    }

}
