using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using System.Globalization;
//using Microsoft.Office.Interop.Excel;

namespace HINOSystem.Libs
{
    public class NPOIClass
    {

        private readonly IConfiguration _config;


        protected IWorkbook workbook;
        protected DataFormatter dataFormatter;
        protected IFormulaEvaluator formulaEvaluator;

        private int FormatRow = 0;

        public NPOIClass(IConfiguration configuration)
        {
            _config = configuration;
        }


        public ISheet Open(string pFileName = "", dynamic pSheetName = null)
        {

            XSSFWorkbook workBook;
            ISheet _sheet;

            using (FileStream file = new FileStream(pFileName, FileMode.Open, FileAccess.Read))
            {
                workBook = new XSSFWorkbook(file);
                file.Close();
                _sheet = workBook.GetSheetAt(pSheetName);
            }

            return _sheet;
        }


        public ISheet Create(string pFileName = "", string pSheetName = null)
        {

            //XSSFWorkbook workBook;
            ISheet _sheet;

            using (var file = new FileStream(pFileName, FileMode.Open, FileAccess.ReadWrite))
            {
                var workBook = new XSSFWorkbook(file);
                _sheet = workBook.CreateSheet(pSheetName);
                file.Close();
            }

            return _sheet;
        }



        public void Append(ISheet pSheet, string[] pArray = null, int pFormatRow=0)
        {
            this.FormatRow = pFormatRow;

            int _row = (pFormatRow > pSheet.LastRowNum ? pFormatRow : pSheet.LastRowNum + 1);

            if (pFormatRow == 0 && pSheet.LastRowNum == 0) _row = 0;

            if (pFormatRow != 0) pSheet.CopyRow(pFormatRow - 1, _row);



            if (pSheet.GetRow(_row) == null) pSheet.CreateRow(_row);


            for (int c = 0; c < pArray.Length; c++)
            {

                if (pSheet.GetRow(_row).GetCell(c) == null) pSheet.GetRow(_row).CreateCell(c);

                pSheet.GetRow(_row).GetCell(c).SetCellValue(pArray[c]);
            }

        }


        public void Cell(ISheet pSheet, int pRow = 0, int pCell = 0, dynamic pValue = null, dynamic pAlignment = null, string pType = null)
        {
            string _value = "";
            try
            {
                _value = pValue.ToString();
                if (pSheet.GetRow(pRow) == null) pSheet.CreateRow(pRow);
                if (pSheet.GetRow(pRow).GetCell(pCell) == null) pSheet.GetRow(pRow).CreateCell(pCell);


                var _style = pSheet.Workbook.CreateCellStyle();
                //IDataFormat _format = pSheet.Workbook.CreateDataFormat();
                _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                

                if (pType == null)
                {
                    pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_value.ToString());
                }else
                {
                    if (pType.ToString().ToLower() == "numeric")
                    {
                        if (_value == null) _value = "0";
                        if (_value != null)
                        {
                            decimal _dec = decimal.Parse(_value);
                            int _int = (int) _dec;
                            _value = _int.ToString();
                            double _dou = double.Parse(_value);
                            pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_dou);
                        }

                        _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                        _style.DataFormat = pSheet.Workbook.CreateDataFormat().GetFormat("#,##0");

                    }
                    if (pType.ToString().ToLower() == "currency")
                    {
                        if (decimal.TryParse(_value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal value))
                        {
                            _value = value.ToString("C2", CultureInfo.InvariantCulture);
                            _value = _value.Substring(1);
                        }

                        double _dou = double.Parse(_value);
                        pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_dou);
                        _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                        _style.DataFormat = pSheet.Workbook.CreateDataFormat().GetFormat("#,##0.00");

                    }
                }


                if (pAlignment != null)
                {
                    if (pAlignment == "0") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    if (pAlignment == "l") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    if (pAlignment == "left") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                    if (pAlignment == "1") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    if (pAlignment == "c") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    if (pAlignment == "center") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    if (pAlignment == "2") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    if (pAlignment == "r") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    if (pAlignment == "right") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                }

                pSheet.GetRow(pRow).GetCell(pCell).CellStyle = _style;

                //pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_value.ToString());

            }
            catch (Exception e)
            {
                pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_value);
            }

        }
        public void SetCellValue(ISheet pSheet, int pRow = 0, int pCell = 0, dynamic pValue = null, dynamic pAlignment = null, string pType = null)
        {
            this.Cell(pSheet,pRow,pCell,pValue,pAlignment,pType);
        }



        public void CellFormula(ISheet pSheet, int pRow = 0, int pCell = 0, dynamic pValue = null, dynamic pAlignment = null, string pType = null)
        {
            string _value = "";
            try
            {
                _value = pValue.ToString();
                if (pSheet.GetRow(pRow) == null) pSheet.CreateRow(pRow);
                if (pSheet.GetRow(pRow).GetCell(pCell) == null) pSheet.GetRow(pRow).CreateCell(pCell);


                var _style = pSheet.Workbook.CreateCellStyle();
                //IDataFormat _format = pSheet.Workbook.CreateDataFormat();
                _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                pSheet.GetRow(pRow).GetCell(pCell).SetCellFormula(_value.ToString());



                if (pType.ToString().ToLower() == "numeric")
                {
                    if (_value == null) _value = "0";
                    //if (_value != null)
                    //{
                    //    decimal _dec = decimal.Parse(_value);
                    //    int _int = (int)_dec;
                    //    _value = _int.ToString();
                    //    double _dou = double.Parse(_value);
                    //    pSheet.GetRow(pRow).GetCell(pCell).SetCellFormula(_dou.ToString());
                    //}

                    _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    _style.DataFormat = pSheet.Workbook.CreateDataFormat().GetFormat("#,##0");

                }
                if (pType.ToString().ToLower() == "currency")
                {
                    //if (decimal.TryParse(_value, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal value))
                    //{
                    //    _value = value.ToString("C2", CultureInfo.InvariantCulture);
                    //    _value = _value.Substring(1);
                    //}

                    //double _dou = double.Parse(_value);
                    //pSheet.GetRow(pRow).GetCell(pCell).SetCellFormula(_dou.ToString());

                    _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    _style.DataFormat = pSheet.Workbook.CreateDataFormat().GetFormat("#,##0.00");

                }

                if (pAlignment != null)
                {
                    if (pAlignment == "0") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    if (pAlignment == "l") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    if (pAlignment == "left") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

                    if (pAlignment == "1") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    if (pAlignment == "c") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    if (pAlignment == "center") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                    if (pAlignment == "2") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    if (pAlignment == "r") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                    if (pAlignment == "right") _style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;
                }

                pSheet.GetRow(pRow).GetCell(pCell).CellStyle = _style;

                //pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_value.ToString());

            }
            catch (Exception e)
            {
                pSheet.GetRow(pRow).GetCell(pCell).SetCellValue(_value);
            }

        }
        public void SetCellFormula(ISheet pSheet, int pRow = 0, int pCell = 0, dynamic pValue = null, dynamic pAlignment = null, string pType = null)
        {
            this.CellFormula(pSheet, pRow, pCell, pValue, pAlignment, pType);
        }

        public void Save(string pFileName = "", ISheet pSheet = null)
        {
            if (this.FormatRow != 0)
            {
                pSheet.RemoveRow(pSheet.GetRow(this.FormatRow - 1));
                pSheet.ShiftRows(this.FormatRow, pSheet.LastRowNum, -1);
            }

            using (FileStream file = new FileStream(pFileName, FileMode.Create, FileAccess.Write))
            {
                pSheet.Workbook.Write(file, true);
                file.Close();
            }
        }

        //
        // Get value with string type
        //
        public string GetStringValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue.ToString().Trim();
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString().Trim();
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString().Trim();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (cell.CellType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue.ToString().Trim();
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString().Trim();
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString().Trim();
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch
                {
                    // When failed in evaluating the formula, use stored values instead...
                    // and set cell value for reference from formulae in other cells...
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue.ToString().Trim();
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString().Trim();
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString().Trim();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (cell.CellType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue.ToString().Trim();
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString().Trim();
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString().Trim();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }




        //
        // Get formatted value as string from the specified cell
        //
        public string GetFormattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    // Get evaluated and formatted cell value
                    returnValue = this.dataFormatter.FormatCellValue(cell, this.formulaEvaluator);
                }
                catch
                {
                    // When failed in evaluating the formula, use stored values instead...
                    // and set cell value for reference from formulae in other cells...
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = dataFormatter.FormatRawCellContents
                                (cell.NumericCellValue, 0, cell.CellStyle.GetDataFormatString());
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }






        //
        // Get unformatted value as string from the specified cell
        //
        public string GetUnformattedValue(ICell cell)
        {
            string returnValue = string.Empty;
            if (cell != null)
            {
                try
                {
                    // Get evaluated cell value
                    returnValue = (cell.CellType == CellType.Numeric ||
                    (cell.CellType == CellType.Formula &&
                    cell.CachedFormulaResultType == CellType.Numeric)) ?
                        formulaEvaluator.EvaluateInCell(cell).NumericCellValue.ToString() :
                        this.dataFormatter.FormatCellValue(cell, this.formulaEvaluator);
                }
                catch
                {
                    // When failed in evaluating the formula, use stored values instead...
                    // and set cell value for reference from formulae in other cells...
                    if (cell.CellType == CellType.Formula)
                    {
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.String:
                                returnValue = cell.StringCellValue;
                                cell.SetCellValue(cell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                returnValue = cell.NumericCellValue.ToString();
                                cell.SetCellValue(cell.NumericCellValue);
                                break;
                            case CellType.Boolean:
                                returnValue = cell.BooleanCellValue.ToString();
                                cell.SetCellValue(cell.BooleanCellValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return (returnValue ?? string.Empty).Trim();
        }


    }

}