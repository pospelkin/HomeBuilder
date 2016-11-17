using UnityEngine;
using System.Collections;
using HomeBuilder.Core;
using sharpPDF;
using sharpPDF.Enumerators;

namespace HomeBuilder.Share
{

    public class PDFComposer : MonoBehaviour
    {

        public string DownloadPDF(Appartment app)
        {
            return CreatePDF(app);
        }

        string CreatePDF(Appartment app)
        {
            pdfDocument myDoc = new pdfDocument(app.GetName(), "HomeBuilder", false);
            pdfPage myFirstPage = myDoc.addPage();

            myFirstPage.addText(app.GetName(), 10, 730, predefinedFont.csTimesBold, 30, new pdfColor(predefinedColor.csBlack));

            myFirstPage.addText(
                "An appartment " + Format(app.GetSquare()) + "m2" + (app.GetFloors() == 2 ? " with 2 floors." : "."),
                5, 700, predefinedFont.csTimes, 12, new pdfColor(predefinedColor.csBlack));

            int yPos = 660;
            /*Add Columns to a grid*/
            for (int i = 0; i < app.GetFloors(); i++)
            {
                ModuleInfo[] modules = app.GetModules(i);

                myFirstPage.addText(
                    "Floor " + i,
                    5, yPos, predefinedFont.csTimesBold, 20, new pdfColor(predefinedColor.csBlack));
                /*Table's creation*/
                pdfTable myTable = new pdfTable();
                //Set table's border
                myTable.borderSize = 1;
                myTable.borderColor = new pdfColor(predefinedColor.csBlack);

                myTable.tableHeader.addColumn(new pdfTableColumn("Name",   predefinedAlignment.csCenter, 120));
                myTable.tableHeader.addColumn(new pdfTableColumn("Style",  predefinedAlignment.csCenter, 120));
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
                /*Put the table on the page object*/
                myFirstPage.addTable(myTable, 5, yPos);

                yPos -= (modules.Length + 1) * 50;

            }

            Debug.Log("WRITE PDF");
            string path = myDoc.createPDF(app.GetName() + ".pdf");
            Debug.Log("PDF SAVED");

            return path;
        }

        string Format(float value)
        {
            float newValue = Mathf.RoundToInt(value*10f)/10f;
            return "" + newValue;
        }

    }

}
