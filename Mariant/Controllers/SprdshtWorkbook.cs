using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Linq;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;

namespace WebAsistida.lib
{
    public class SprdshtWorkbook
    {
        SpreadsheetDocument doc;
        bool useSharedString;
        Workbook wbook;

        SharedStringTablePart sharedStringTablePart;
        WorkbookStylesPart workbookStylesPart;
        

        public static SprdshtWorkbook Create(String fileName, bool useSharedString)
        {
            return new SprdshtWorkbook(fileName, useSharedString);
        }

        public static SprdshtWorkbook Open(String fileName)
        {
            SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, true);
            return new SprdshtWorkbook(doc);
        }

        ///////////////////////////////////////////////////////////////////////////////////
        private SprdshtWorkbook(SpreadsheetDocument p_doc)
        {
            doc = p_doc;
            wbook = p_doc.WorkbookPart.Workbook;

            sharedStringTablePart = doc.WorkbookPart.SharedStringTablePart;
            workbookStylesPart = doc.WorkbookPart.WorkbookStylesPart;

            useSharedString = sharedStringTablePart.SharedStringTable.Count() != 0 ? true:false;

            /*
             --SpreadsheetDocument doc;
        bool useSharedString;
        --Workbook wbook;

        --SharedStringTablePart sharedStringTablePart;
        --WorkbookStylesPart workbookStylesPart;
             */
        }
        ///////////////////////////////////////////////////////////////////////////////////

        private SprdshtWorkbook(String fileName, bool useSharedString)
        {
            
            doc = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook,false);
            // Si existe habria que abrirlo, o borrarlo y recrearlo.
            //wbook = SpreadsheetDocument.Open(fileName, true);

            //Create initial parts               
            // Workbook
            doc.AddWorkbookPart();
            wbook = doc.WorkbookPart.Workbook = new Workbook();
            //doc.WorkbookPart.Workbook.Save();

            // Shared string table
            sharedStringTablePart = doc.WorkbookPart.AddNewPart<SharedStringTablePart>();
            sharedStringTablePart.SharedStringTable = new SharedStringTable();
            sharedStringTablePart.SharedStringTable.Save();

            // Sheets collection
            doc.WorkbookPart.Workbook.Sheets = new Sheets();
            //doc.WorkbookPart.Workbook.Save();

            // Stylesheet
            workbookStylesPart = doc.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = CreateStylesheet();
            workbookStylesPart.Stylesheet.Save();

            //AddBasicStyles();
        }

        public void save()
        {
            wbook.Save();
        }

        public void close()
        {
            doc.Close();
        }

        // Generates content of workbookStylesPart1
        private static Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            fonts1.Append(font1);

            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);

            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };
            fill2.Append(patternFill2);

            // FillId = 2,LIGHT ORANGE
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFF9900" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3,GRAY 25%
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "FFC0C0C0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4,LIGHT GREEN
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFCCFFCC" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)4U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };
          
            stylesheetExtension1.Append(slicerStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);
            return stylesheet1;

        }

        /*
        public bool AddBasicStyles()
        {
            Stylesheet stylesheet = doc.WorkbookPart.WorkbookStylesPart.Stylesheet;

            // Numbering formats (x:numFmts)
            stylesheet.InsertAt<NumberingFormats>(new NumberingFormats(), 0);
            // Currency
            stylesheet.GetFirstChild<NumberingFormats>().InsertAt<NumberingFormat>(
               new NumberingFormat()
               {
                   NumberFormatId = 164,
                   FormatCode = "#,##0.00"
                   + "\\ \"" + System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol + "\""
               }, 0);

            // Fonts (x:fonts)
            stylesheet.InsertAt<Fonts>(new Fonts(), 1);
            stylesheet.GetFirstChild<Fonts>().InsertAt<Font>(
               new Font()
               {
                   FontSize = new FontSize()
                   {
                       Val = 11
                   },
                   FontName = new FontName()
                   {
                       Val = "Calibri"
                   }
               }, 0);

            // Fills (x:fills)
            stylesheet.InsertAt<Fills>(new Fills(), 2);
            stylesheet.GetFirstChild<Fills>().InsertAt<Fill>(
               new Fill()
               {
                   PatternFill = new PatternFill()
                   {
                       PatternType = new EnumValue<PatternValues>()
                       {
                           Value = PatternValues.None
                       }
                   }
               }, 0);

            // Borders (x:borders)
            stylesheet.InsertAt<Borders>(new Borders(), 3);
            stylesheet.GetFirstChild<Borders>().InsertAt<Border>(
               new DocumentFormat.OpenXml.Spreadsheet.Border()
               {
                   LeftBorder = new LeftBorder(),
                   RightBorder = new RightBorder(),
                   TopBorder = new TopBorder(),
                   BottomBorder = new BottomBorder(),
                   DiagonalBorder = new DiagonalBorder()
               }, 0);

            // Cell style formats (x:CellStyleXfs)
            stylesheet.InsertAt<CellStyleFormats>(new CellStyleFormats(), 4);
            stylesheet.GetFirstChild<CellStyleFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   NumberFormatId = 0,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               }, 0);

            // Cell formats (x:CellXfs)
            stylesheet.InsertAt<CellFormats>(new CellFormats(), 5);
            // General text
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   FormatId = 0,
                   NumberFormatId = 0
               }, 0);
            // Date
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 22,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  1);
            // Currency
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 164,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  2);
            // Percentage
            stylesheet.GetFirstChild<CellFormats>().InsertAt<CellFormat>(
               new CellFormat()
               {
                   ApplyNumberFormat = true,
                   FormatId = 0,
                   NumberFormatId = 10,
                   FontId = 0,
                   FillId = 0,
                   BorderId = 0
               },
                  3);

            stylesheet.Save();

            return true;
        }
        */

        public SprdShtWorkSheet addSheet(String sheetName )
        {
            //return new SprdShtWorkSheet( wbook.Sheets.AddNewPart<WorksheetPart>(sheetName) );

            // Add the worksheetpart
            WorksheetPart worksheetPart = doc.WorkbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            //worksheetPart.Worksheet.Save();

            // Add the sheet and make relation to workbook
            Sheet sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet() {
                Id = doc.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = (uint)(doc.WorkbookPart.Workbook.Sheets.Count() + 1),
                Name = sheetName
            };
            // Agrega la nueva pagina al conjunto
            Sheets sheets = doc.WorkbookPart.Workbook.GetFirstChild<Sheets>();

            sheets.Append(sheet);

            //doc.WorkbookPart.Workbook.Save();

            OpenXmlWriter writer = OpenXmlWriter.Create(worksheetPart);
            writer.WriteStartElement(new Worksheet());
            writer.WriteStartElement(new SheetData());
            
            return new SprdShtWorkSheet(doc, worksheetPart.Worksheet, writer, useSharedString);
            
            /*
            return new SprdShtWorkSheet(doc, worksheetPart.Worksheet, useSharedString);
            */
        }

        public SprdShtWorkSheet findSheet(String sheetName)
        {
            SprdShtWorkSheet retSheet = null;
            /*
            foreach (WorksheetPart worksheetpart in doc.WorkbookPart.WorksheetParts)
            {
                foreach (OpenXmlAttribute attr in worksheetpart.Worksheet.GetAttributes())
                {
                    Console.WriteLine("{0}: {1}", attr.LocalName, attr.Value);
                }
                
                if (sheetName.Equals(worksheetpart.Worksheet.GetAttribute("Name", "").Value, StringComparison.CurrentCultureIgnoreCase))
                {
                    retSheet = new SprdShtWorkSheet(doc, worksheetpart.Worksheet, useSharedString);
                }
                 
            }*/
            // Iterate Sheets; Get Name and xref WorksheetPart (container for Worksheet)
            foreach (Sheet sheet in doc.WorkbookPart.Workbook.Sheets)
            {
                string sName = sheet.Name;
                string sID = sheet.Id;

                if (sheetName.Equals(sName, StringComparison.CurrentCultureIgnoreCase))
                {
                    WorksheetPart part = (WorksheetPart)doc.WorkbookPart.GetPartById(sID);
                    //Worksheet actualSheet = part.Worksheet;

                    OpenXmlReader reader = OpenXmlReader.Create(part);
                    retSheet = new SprdShtWorkSheet(doc, part.Worksheet, reader, useSharedString);
                    return retSheet;
                }
            }
            return retSheet;
        }

/*
        private WorksheetPart
             GetWorksheetPartByName( string sheetName)
        {
            IEnumerable<Sheet> sheets =
               doc.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                // The specified worksheet does not exist.

                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 doc.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }
 */

        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts a column number to column name (i.e. A, B, C..., AA, AB...)
        /// </summary>
        /// <param name="columnIndex">Index of the column</param>
        /// <returns>Column name</returns>
        public static string ColumnNameFromIndex(uint columnIndex)
        {
            uint remainder;
            string columnName = "";

            while (columnIndex > 0)
            {
                remainder = (columnIndex - 1) % 26;
                columnName = System.Convert.ToChar(65 + remainder).ToString() + columnName;
                columnIndex = (uint)((columnIndex - remainder) / 26);
            }

            return columnName;
        }  
        ///////////////////////////////////////////////////////////////////////////////////

        public class SprdShtWorkSheet
        {
            SpreadsheetDocument doc;
            Worksheet wSheet;
            SheetData sheetData;
            uint currentRowCount;
            OpenXmlWriter writer;
            OpenXmlReader reader;


            internal SprdShtWorkSheet(SpreadsheetDocument p_doc, Worksheet p_sheet, OpenXmlWriter p_writer, bool useSharedString)
            {
                doc = p_doc;
                wSheet = p_sheet;
                sheetData = p_sheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
                writer = p_writer;
            }

            internal SprdShtWorkSheet(SpreadsheetDocument p_doc, Worksheet p_sheet, OpenXmlReader p_reader, bool useSharedString)
            {
                doc = p_doc;
                wSheet = p_sheet;
                sheetData = p_sheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
                reader = p_reader;
            }
            public void close()
            {
                if (writer != null)
                {
                    writer.WriteEndElement(); //end of SheetData
                    writer.WriteEndElement(); //end of worksheet
                    writer.Close();
                }
            }

            /*
            internal SprdShtWorkSheet(SpreadsheetDocument p_doc, Worksheet p_sheet, bool useSharedString)
            {
                doc = p_doc;
                wSheet = p_sheet;
            }
            */
            ///////////////////////////////////////////////////////////////////////////////////

            public bool addRow(String[] strArr, String[] colorCol, String[] type = null)
            {
                if (writer == null)
                {
                    return false;
                }
                uint rowIndex = currentRowCount + 1;
                Row theRow = new Row() { RowIndex = rowIndex };
                writer.WriteStartElement(theRow);
                for (uint columnIndex = 0; columnIndex < strArr.Length; columnIndex++)
                {
                    string cellReference = SprdshtWorkbook.ColumnNameFromIndex(columnIndex + 1) + rowIndex;
                    
                    if (colorCol[columnIndex] == "GREEN")
                    {
                        Cell theCell = new Cell()
                        {
                            CellReference = cellReference,
                            CellValue = new CellValue(strArr[columnIndex]),
                            StyleIndex = (UInt32Value)3U,
                            DataType = CellValues.String
                        };
                        writer.WriteElement(theCell);
                    }
                    else if (colorCol[columnIndex] == "ORANGE")
                    {
                        Cell theCell = new Cell()
                        {
                            CellReference = cellReference,
                            CellValue = new CellValue(strArr[columnIndex]),
                            StyleIndex = (UInt32Value)1U,
                            DataType = CellValues.String
                        };
                        writer.WriteElement(theCell);
                    }
                    else if (colorCol[columnIndex] == "GREY")
                    {
                        Cell theCell = new Cell()
                        {
                            CellReference = cellReference,
                            CellValue = new CellValue(strArr[columnIndex]),
                            StyleIndex = (UInt32Value)2U,
                            DataType = CellValues.String
                        };
                        writer.WriteElement(theCell);
                    }
                    else
                    {
                        Cell theCell = new Cell()
                        {
                            CellReference = cellReference,
                            CellValue = new CellValue(strArr[columnIndex]),
                            DataType = CellValues.String
                        };
                        writer.WriteElement(theCell);
                    }
                }
                currentRowCount++;
                writer.WriteEndElement(); //end of Row
                return true;
            }
            
                /*
                SheetData sheetData = wSheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();

                uint rowIndex = currentRowCount + 1;

                var theRow = new Row { RowIndex = rowIndex };
                sheetData.Append(theRow);
                currentRowCount = rowIndex;

                for (uint columnIndex = 0; columnIndex < strArr.Length; columnIndex++)
                {
                    CellValues valType;
                    if (type == null)
                    {
                        valType = CellValues.String;
                    }
                    else
                    {
                        switch (type[columnIndex].ToUpper())
                        {
                            case "VARCHAR":
                                valType = CellValues.String;
                                break;
                            // Faltan otros tipos.
                            default:
                                valType = CellValues.String;
                                break;

                        }
                    }

                    string cellReference = SprdshtWorkbook.ColumnNameFromIndex(columnIndex + 1) + rowIndex;
                    Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
                    CellValue cellValue = new CellValue();
                    cellValue.Text = strArr[columnIndex];
                    cell.Append(cellValue);
                    theRow.Append(cell);
                }
                wSheet.Save();
                return true;
                */

            ///////////////////////////////////////////////////////////////////////////////////

            public String[] getRow( uint rowIndex, uint columnCount = 0 )
            {
                //SheetData sheetData = wSheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
                // Check if the row exists.
                if (sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() == 0)
                {
                    return null;
                }

                uint colCnt;
                if (columnCount == 0) // Hay que determinar cantidad de columnas disponibles.
                {
                    uint columnIndex = 1;
                    while (GetCellValue(columnIndex, rowIndex) != null)
                    {
                        columnIndex++;  // Cuenta colunmas con datos.
                    }

                    if (columnIndex == 1) // Si la fila esta vacia, no hay datos.
                    {
                        return null;
                    }
                    colCnt = columnIndex - 1; // Descuento el que fallo.
                }
                else
                {
                    colCnt = columnCount;
                }

                String[] ret = new String[colCnt];

                for (uint colIdx = 0; colIdx < colCnt; colIdx++)
                {
                    ret[colIdx] = GetCellValue(colIdx+1, rowIndex);
                }
                return ret;
            }

            ///////////////////////////////////////////////////////////////////////////////////

            public Cell SetCellValue(
                uint columnIndex,
                uint rowIndex,
                CellValues valueType,
                string value,
                uint? styleIndex,
                bool save = true)
            {
                //SheetData sheetData = wSheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
                Row row;
                Row previousRow = null;
                Cell cell;
                Cell previousCell = null;
                Columns columns;
                Column previousColumn = null;
                string cellAddress = SprdshtWorkbook.ColumnNameFromIndex(columnIndex) + rowIndex;

                // Check if the row exists, create if necessary
                if (sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() != 0)
                {
                    row = sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).First();
                }
                else
                {
                    row = new Row() { RowIndex = rowIndex };
                    //sheetData.Append(row);
                    for (uint counter = rowIndex - 1; counter > 0; counter--)
                    {
                        previousRow = sheetData.Elements<Row>().Where(item => item.RowIndex == counter).FirstOrDefault();
                        if (previousRow != null)
                        {
                            break;
                        }
                    }
                    sheetData.InsertAfter(row, previousRow);
                }

                // Check if the cell exists, create if necessary
                if (row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).Count() > 0)
                {
                    cell = row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).First();
                }
                else
                {
                    // Find the previous existing cell in the row
                    for (uint counter = columnIndex - 1; counter > 0; counter--)
                    {
                        previousCell = row.Elements<Cell>().Where(item => item.CellReference.Value == SprdshtWorkbook.ColumnNameFromIndex(counter) + rowIndex).FirstOrDefault();
                        if (previousCell != null)
                        {
                            break;
                        }
                    }
                    cell = new Cell() { CellReference = cellAddress };
                    row.InsertAfter(cell, previousCell);
                }

                // Check if the column collection exists
                columns = wSheet.Elements<Columns>().FirstOrDefault();
                if (columns == null)
                {
                    columns = wSheet.InsertAt(new Columns(), 0);
                }
                // Check if the column exists
                if (columns.Elements<Column>().Where(item => item.Min == columnIndex).Count() == 0)
                {
                    // Find the previous existing column in the columns
                    for (uint counter = columnIndex - 1; counter > 0; counter--)
                    {
                        previousColumn = columns.Elements<Column>().Where(item => item.Min == counter).FirstOrDefault();
                        if (previousColumn != null)
                        {
                            break;
                        }
                    }
                    columns.InsertAfter(
                       new Column()
                       {
                           Min = columnIndex,
                           Max = columnIndex,
                           CustomWidth = true,
                           Width = 9
                       }, previousColumn);
                }

                // Add the value
                cell.CellValue = new CellValue(value);
                if (styleIndex != null)
                {
                    cell.StyleIndex = styleIndex;
                }
                if (valueType != CellValues.Date)
                {
                    cell.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(valueType);
                }

                if (save)
                {
                    wSheet.Save();
                }

                return cell;
            }

            public String GetCellValue(uint columnIndex, uint rowIndex)
            {
                Cell cell = null;
                Row row;
                //Column col;
                string cellAddress = SprdshtWorkbook.ColumnNameFromIndex(columnIndex) + rowIndex;

                // Check if the row exists, create if necessary
                if (sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() == 0)
                {
                    return null;
                }
                row = sheetData.Elements<Row>().Where(item => item.RowIndex == rowIndex).First();

                /*
                // Check if the row exists, create if necessary
                if (wSheet.GetFirstChild<SheetData>().Elements<Row>().Where(item => item.RowIndex == rowIndex).Count() == 0)
                {
                    return null;
                }
                row = wSheet.GetFirstChild<SheetData>().Elements<Row>().Where(item => item.RowIndex == rowIndex).First();
                */

                //se comenta ya que no se utiliza para el excel con formato y con campos en español
                /*
                // Check if the column collection exists
                Columns columns = wSheet.Elements<Columns>().FirstOrDefault();
                if (columns == null)
                {
                    return null;
                }

                // Check if the column exists
                if (columns.Elements<Column>().Where(item => item.Min == columnIndex).Count() == 0)
                {
                    return null;
                }
                
                col = columns.Elements<Column>().Where(item => item.Min == columnIndex).First();
                 */

                // Check if the cell exists.
                if (row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).Count() != 0)
                {
                    cell = row.Elements<Cell>().Where(item => item.CellReference.Value == cellAddress).First();
                }

                if (cell == null)
                {
                    return null;
                }

                if (cell.DataType == null)
                    return cell.InnerText;

                string value = cell.InnerText;
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                        // For shared strings, look up the value in the shared strings table.
                        // Get worksheet from cell
                        OpenXmlElement parent = cell.Parent;
                        while (parent.Parent != null && parent.Parent != parent
                                && string.Compare(parent.LocalName, "worksheet", true) != 0)
                        {
                            parent = parent.Parent;
                        }
                        if (string.Compare(parent.LocalName, "worksheet", true) != 0)
                        {
                            throw new Exception("Unable to find parent worksheet.");
                        }

                        //Worksheet ws = parent as Worksheet;
                        //SpreadsheetDocument ssDoc = ws.WorksheetPart.OpenXmlPackage as SpreadsheetDocument;
                        SharedStringTablePart sstPart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

                        // lookup value in shared string table
                        if (sstPart != null && sstPart.SharedStringTable != null)
                        {
                            value = sstPart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;

                            if (value.IndexOf(",") != -1)
                            {
                                value = value.Replace(",",".");
                            }
                        }
                        break;

                    //this case within a case is copied from msdn. 
                    case CellValues.Boolean:
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                        break;
                }
                return value;
            }
        }
    }
}