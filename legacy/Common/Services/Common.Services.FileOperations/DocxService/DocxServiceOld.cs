using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Common.Contracts.Queue.Models;
using Common.Services.FileOperations.Constant;
using Microsoft.AspNetCore.Http;
using GemBox.Document;
using GemBox.Document.Tables;
using iTextSharp.LGPLv2.Core.System.Drawing;
using iTextSharp.LGPLv2.Core.System.Encodings;
using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace Common.Services.FileOperations;

public class DocxServiceOld
{
    public DocxServiceOld()
    {
        ComponentInfo.SetLicense("DN-2024Jan08-wMMlyE2pW1c4VIBQA0EJ2bSfv05Hb9dMplpG4H4fgyIMV/FhdRM8v3EubxD/SkiArTLzUTfxHtt1OhExeqBXl7q55Zg==A");

        // ComponentInfo.SetLicense("DN-2023Dec22-2024Jan22-pUSuoodqTN29ZbVbgW4ez492B0QIY6eI2pgSPHHKBd/MWA/xnAbYP9A2GgJNKyJQ1HdKKzWqUIbMlcljVWBtmhxIbXg==B");


        // Set license key to use GemBox.Document in a Free mode.
        // ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        // Continue to use the component in a Trial mode when free limit is reached.
        // ComponentInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
    }

    public byte[] ReplaceDocxContent(byte[] docxBytes, List<ReplacementItem> replacementData)
    {
        var isAttachmentsInAnotherPage = false;
        var isDistributionsInAnotherPage = false;
        var isDocumentHaveDistibutions = replacementData.Any(a => (a.Key == TemplateConstantOld.DLV_P && a.Values.Count > 0) || (a.Key == TemplateConstantOld.DLV_S && a.Values.Count > 0));

        Stream stream = new MemoryStream(docxBytes);
        DocumentModel documentModel = DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);

        foreach (var replacementItem in replacementData)
        {
            var searchKey = $"{{{replacementItem.Key}}}";
            var matchedContentRanges = documentModel.Content.Find(searchKey).Reverse();

            foreach (ContentRange replacementContentRange in matchedContentRanges) // workaround: Reverse() is proposed by GemBox support, otherwise gives error, because LoadText changes Content in iteration
            {
                var value = replacementItem.Values.FirstOrDefault();
                switch (replacementItem.Key)
                {
                    // Not created for pdf
                    case TemplateConstantOld.X_Creator_Organization:
                        break;

                    case TemplateConstantOld.DLV_P:
                    case TemplateConstantOld.DLV_S:
                    case TemplateConstantOld.DOCUMENT_NUMBER:
                    case TemplateConstantOld.DOCUMENT_DATE:
                    case TemplateConstantOld.SUBJECT:
                    case TemplateConstantOld.ESIGNED_LABEL:
                    case TemplateConstantOld.ACCESS_CODE:
                    case TemplateConstantOld.AUTHOR_NAMESURNAME:
                    case TemplateConstantOld.AUTHOR_TITLE:
                    case TemplateConstantOld.MUHATAP:
                    case TemplateConstantOld.DOCADD:
                    case TemplateConstantOld.PRIORITY:
                    case TemplateConstantOld.VALIDATION_URL:
                        value = replacementItem.GetValue();
                        this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
                        break;

                    case TemplateConstantOld.WRITING:
                        value = replacementItem.GetValue();
                        var cleanEditorContent = this.CleanEditorContent(value);
                        this.ChangeText(replacementContentRange, cleanEditorContent, replacementItem.ContentType);
                        break;

                    case TemplateConstantOld.DOCREF:
                        if (replacementItem.Values.Count == 0)
                        {
                            documentModel.GetChildElements(true, ElementType.TableRow)
                                                   .Where(r => r.Content.ToString().Contains(TemplateConstantOld.DOCREF))
                                                   .ToList()
                                                   .ForEach(r => r.Content.Delete());

                            // foreach (Table table in documentModel.GetChildElements(true, ElementType.Table))
                            // {
                            //     foreach (TableRow row in table.Rows)
                            //     {
                            //         foreach (ContentRange contentRange in row.Content.Find(new Regex(TemplateConstant.DOCREF.WildcardToRegex())).Reverse()) // {h:x} hide row if x has value, {h:!x} hide row if x is empty
                            //         {
                            //             System.Console.WriteLine(contentRange.ToString().StartsWith(TemplateConstant.DOCREF));
                            //             // row.RowFormat.Hidden = true;
                            //             contentRange.Delete();
                            //         }
                            //     }
                            // }
                        }
                        else
                        {
                            value = replacementItem.GetValue();
                            System.Console.WriteLine($"value: {value}");
                            this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
                        }
                        break;

                    case TemplateConstantOld.ONAY_A:
                    case TemplateConstantOld.ONAY_C:
                    case TemplateConstantOld.ONAY_B:
                        value = replacementItem.GetValue();
                        this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
                        break;
                    default:
                        throw new Exception($"Replacement Data Not Handled {replacementItem.Key}");
                }
            }
        }


        #region Attachment

        ContentRange DOCADDSTART = documentModel.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.DOCADD_START)).FirstOrDefault();
        ContentRange DOCADDEND = documentModel.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.DOCADD_END)).FirstOrDefault();
        ContentRange docAddLabelRange = documentModel.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.DOCADD_LABEL)).FirstOrDefault();
        bool isAttachmentNotExist = false;

        if (docAddLabelRange != null && DOCADDSTART != null && DOCADDEND != null)
        {
            var docadd = replacementData.Where(d => d.Key == TemplateConstantOld.DOCADD).FirstOrDefault();

            string[] docDeliveryStringList = docadd.Values.ToArray();
            int docAddRowNumber = docDeliveryStringList.Length;
            // this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
            if (docAddRowNumber == 0)
            {
                isAttachmentNotExist = true;
                new ContentRange(DOCADDSTART.Start, DOCADDEND.End).Delete();
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                   .Where(r => r.Content.ToString().Contains(TemplateConstantOld.DOCADD_LABEL))
                                                   .ToList()
                                                   .ForEach(r => r.Content.Delete());
            }
            else if (docAddRowNumber > 1)
            {
                if (GetPageNumber(DOCADDSTART.Start) != GetPageNumber(DOCADDEND.End))
                {
                    isAttachmentsInAnotherPage = true;
                    new ContentRange(DOCADDSTART.Start, DOCADDEND.End).Delete();

                    AddDocAddListAnotherPage(documentModel, docDeliveryStringList);

                    if (docAddLabelRange != null)
                        this.ChangeText(docAddLabelRange, $"EK: Ek Listesi ({docAddRowNumber} Adet)", ReplacementItemContentTypes.Text);
                    else
                        System.Console.WriteLine("label is null");
                }
            }
        }
        #endregion

        #region Distribution

        ContentRange DLVSTART = documentModel.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.DLV_START)).FirstOrDefault();
        ContentRange DLVEND = documentModel.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.DLV_END)).FirstOrDefault();
        ContentRange dlvLabelRange = documentModel.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.DLV_LABEL)).FirstOrDefault();
        // ContentRange dlv = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.DLV)).FirstOrDefault();

        var dlv_p = replacementData.Where(d => d.Key == TemplateConstantOld.DLV_P).FirstOrDefault();
        var dlv_s = replacementData.Where(d => d.Key == TemplateConstantOld.DLV_S).FirstOrDefault();
        var splittedPrimaryList = dlv_p.Values.ToArray();
        var splittedSecondaryList = dlv_s.Values.ToArray();
        int totalDistributionCount = splittedPrimaryList.Length + splittedSecondaryList.Length;

        if (totalDistributionCount == 0)
        {
            if (DLVSTART != null && DLVEND != null)
            {
                new ContentRange(DLVSTART.Start, DLVEND.End).Delete();
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                .Where(r => r.Content.ToString().Contains(TemplateConstantOld.DLV_LABEL))
                                                .ToList()
                                                .ForEach(r => r.Content.Delete());
            }
        }
        else
        {
            if (dlvLabelRange != null && DLVSTART != null && DLVEND != null)
            {
                System.Console.WriteLine($"start: {GetPageNumber(DLVSTART.Start)}");
                System.Console.WriteLine($"end: {GetPageNumber(DLVEND.End)}");
                if (GetPageNumber(DLVSTART.Start) != GetPageNumber(DLVEND.End))
                {
                    isDistributionsInAnotherPage = true;
                    new ContentRange(DLVSTART.Start, DLVEND.End).Delete();
                    AddDeliveryListAnotherPage(documentModel, splittedPrimaryList, splittedSecondaryList);

                    if (dlvLabelRange != null)
                        dlvLabelRange.LoadText("DAĞITIM: Dağıtım Listesi (" + totalDistributionCount + " Muhatap)");
                }
                else
                {
                    if (isAttachmentNotExist || isAttachmentsInAnotherPage)
                    {
                        documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstantOld.DLV_LABEL))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
                    }
                }
            }
        }

        #endregion

        List<TableRow> singlePageHiddenRows = new List<TableRow>();
        foreach (Table table in documentModel.GetChildElements(true, ElementType.Table))
        {
            foreach (TableRow row in table.Rows)
            {
                foreach (ContentRange contentRange in row.Content.Find(new Regex(TemplateConstantOld.STAR.WildcardToRegexOld())).Reverse()) // {h:x} hide row if x has value, {h:!x} hide row if x is empty
                {
                    if (contentRange.ToString().StartsWith(TemplateConstantOld.H))
                    {
                        // System.Console.WriteLine($"ContentRange: {contentRange.ToString()}");
                        string contentVal = contentRange.ToString().Replace("{", "").Replace("}", "").Trim();
                        // System.Console.WriteLine($"ContentVal: {contentVal}");
                        bool not = contentVal.StartsWith(TemplateConstantOld.NOT_H);
                        // string search = "{" + contentVal.Mid(not ? 3 : 2).Trim() + "}";
                        string search = contentVal.MidOld(not ? 3 : 2).Trim();
                        // System.Console.WriteLine($"Search: {search}");
                        ReplacementItem item = replacementData.Find(x => x.Key == search);
                        // System.Console.WriteLine($"Matched Item: {item?.Key}:{item?.Values?.Count}");

                        switch (search)
                        {
                            case TemplateConstantOld.DOCADD_LABEL:
                                if (isAttachmentsInAnotherPage)
                                    continue;
                                break;

                            case TemplateConstantOld.DLV:
                                if (isDocumentHaveDistibutions)
                                    continue;
                                break;

                            case TemplateConstantOld.DLV_LABEL:
                                if (isDistributionsInAnotherPage)
                                    continue;
                                break;

                            default:
                                if (item == null)
                                {
                                    if (not)// {h:!x}
                                        row.RowFormat.Hidden = true;
                                }
                                else
                                {
                                    var itemValue = item.Values != null && item.Values.Count > 0 ? item.Values[0] : "";
                                    if ((!not && !itemValue.IsNullOrEmptyOld()) // {h:x}
                                        || (not && itemValue.IsNullOrEmptyOld())) // {h:!x}
                                        row.RowFormat.Hidden = true;
                                }
                                break;
                        }
                    }
                    // Burayi acinca ilgi ve ekler siliniyor. kapali oldugunda da ilgi olmazsa ilgi satiri silinmiyor.
                    // var docRef = replacementData.Where(d => d.Key == TemplateConstant.DOCREF).FirstOrDefault();
                    // if (docRef.Values.Count == 0)
                    //     row.RowFormat.Hidden = true;
                }

                foreach (ContentRange contentRange in row.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.HE)).Reverse()) // hide row if cell is empty
                {
                    // contentRange.LoadText(string.Empty, TxtLoadOptions.TxtDefault);

                    Element tableCell = FindClosestElement(contentRange.Start.Parent, ElementType.TableCell);

                    // System.Console.WriteLine($"HEE:{tableCell.Content.ToString()}");

                    if (string.IsNullOrWhiteSpace(tableCell.Content.ToString()))
                        row.RowFormat.Hidden = true;
                }

                foreach (ContentRange contentRange in row.Content.Find(TemplateConstantOld.GetSearchKey(TemplateConstantOld.HS)).Reverse()) // hide row if single page
                {
                    singlePageHiddenRows.Add(row);
                }

                foreach (ContentRange contentRange in row.Content.Find(new Regex(TemplateConstantOld.GetSearchKey(TemplateConstantOld.STAR_START).WildcardToRegexOld())).Reverse()) // hide row if single page
                    row.RowFormat.Hidden = true;

                foreach (ContentRange contentRange in row.Content.Find(new Regex(TemplateConstantOld.GetSearchKey(TemplateConstantOld.STAR_END).WildcardToRegexOld())).Reverse()) // hide row if single page
                    row.RowFormat.Hidden = true;
            }
        }

        // clear all other "{...}" strings
        for (int i = 0; i < 10; i++) // if exceeded ~10 there may be some errors/bugs 
        {
            bool found = false;

            foreach (ContentRange contentRange in documentModel.Content.Find(new Regex(TemplateConstantOld.STAR.WildcardToRegexOld())).Reverse())
            {
                contentRange.LoadText(string.Empty, TxtLoadOptions.TxtDefault);
                found = true;
            }

            if (!found)
                break;
        }

        int pageCount = 0;

        DocumentModelPaginator paginator = documentModel.GetPaginator();
        pageCount = paginator.Pages.Count;

        System.Console.WriteLine($"Total Page Count: {pageCount}");

        if (pageCount == 1)
        {
            foreach (TableRow row in singlePageHiddenRows)
                row.RowFormat.Hidden = true;
        }


        MemoryStream outputDocx = new MemoryStream();
        documentModel.Save(outputDocx, GemBox.Document.SaveOptions.DocxDefault);

        byte[] newDocxBytes = new byte[outputDocx.Length];
        outputDocx.Read(newDocxBytes, 0, (int)outputDocx.Length);

        return newDocxBytes;
    }

    private bool ChangeText(ContentRange contentRange, string value, ReplacementItemContentTypes itemType)
    {
        try
        {
            switch (itemType)
            {
                case ReplacementItemContentTypes.Text:
                    contentRange.LoadText(value, TxtLoadOptions.TxtDefault);
                    break;
                case ReplacementItemContentTypes.Html:
                    contentRange.LoadText(value, TxtLoadOptions.HtmlDefault);
                    break;
                default:
                    throw new Exception($"Item Type Not Handled. Item Type: {itemType}");
            }

            return true;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            System.Console.WriteLine(ex.InnerException?.Message);

            return false;
        }

    }


    public string CleanEditorContent(string editorContent)
    {
        // Remove font families
        var searchFontFamily = @"font-family\s*:\s*([^;]+);";
        var newEditorContent = Regex.Replace(editorContent, searchFontFamily, "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        // TODO: Make this dictionary
        // Fix table border bug
        var searchTableBorderVariant1 = "<table style=\"";
        var replaceTextVariant1 = "<table border=\"1\" style=\"border-collapse: collapse; border-color: black;";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant1, replaceTextVariant1);

        var searchTableBorderVariant2 = "<table class=\"ck-table-resized\" style=\"";
        var replaceTextVariant2 = "<table border=\"1\" class=\"ck-table-resized\" style=\"border-collapse: collapse;border-color: black;";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant2, replaceTextVariant2);

        var searchTableBorderVariant3 = "<table class=\"ck-table-resized\">";
        var replaceTextVariant3 = "<table border=\"1\" class=\"ck-table-resized\" style=\"border-collapse: collapse;border-color: black;";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant3, replaceTextVariant3);

        var searchTableBorderVariant4 = "<table>";
        var replaceTextVariant4 = "<table border=\"1\" style=\"border-collapse: collapse;border-color: black;\"";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant4, replaceTextVariant4);

        var searchTableBorderVariant5 = "<figure";
        var replaceTextVariant5 = "<div";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant5, replaceTextVariant5);
        var searchTableBorderVariant6 = "</figure>";
        var replaceTextVariant6 = "</div>";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant6, replaceTextVariant6);

        var searchTableBorderVariant7 = "width:38.61%;\"><table";
        // var replaceTextVariant7 = "float:right;\"><table";
        var replaceTextVariant7 = "width:38.61%; margin:.9px auto;\"><table";
        newEditorContent = newEditorContent.Replace(searchTableBorderVariant7, replaceTextVariant7);
        // System.Console.WriteLine("------------------------------");
        // System.Console.WriteLine(newEditorContent);
        // System.Console.WriteLine("------------------------------");


        return OldStringExtensionsOld.HtmlPart1 + newEditorContent + OldStringExtensionsOld.HtmlPart2;
    }

    public byte[] DocxToPdf(byte[] docxBytes)
    {

        Stream stream = new MemoryStream(docxBytes);
        DocumentModel documentModel = DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);

        /// Writing.....
        MemoryStream outputPdf = new MemoryStream();
        documentModel.Save(outputPdf, GemBox.Document.SaveOptions.PdfDefault);

        byte[] bytes = new byte[outputPdf.Length];
        outputPdf.Read(bytes, 0, (int)outputPdf.Length);

        return bytes;
    }


    ///////////////////////////////////////////////////////

    private Element FindClosestElement(Element referenceElement, ElementType elementType)
    {
        Element element = referenceElement;

        while (element != null)
        {
            if (element.ElementType == elementType)
                return element;

            element = element.Parent;
        }

        return element;
    }

    private int GetPageNumber(ContentPosition position)
    {
        DocumentModel document = position.Parent.Document;

        Field pageField = new Field(document, FieldType.Page);
        Field importedPageField = position.InsertRange(pageField.Content).Parent as Field;

        document.GetPaginator(new PaginatorOptions() { UpdateFields = true });

        int pageNumber = int.Parse(importedPageField.Content.ToString());
        importedPageField.Content.Delete();

        return pageNumber;
    }

    private void AddDocAddListAnotherPage(DocumentModel documentModel, string[] docDeliveryStringList)
    {
        Paragraph paragraph = new Paragraph(documentModel);
        Section section = new Section(documentModel, paragraph);
        paragraph.Inlines.Add(new Run(documentModel, "EK LİSTESİ") { CharacterFormat = { Bold = true } });
        paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));
        foreach (var row in docDeliveryStringList)
        {
            paragraph.Inlines.Add(new Run(documentModel, row));
            paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));
        }

        AddDefaultFooter(documentModel, section);
        documentModel.Sections.Add(section);
    }

    private void AddDeliveryListAnotherPage(DocumentModel documentModel, string[] splittedPrimaryList, string[] splittedSecondaryList)
    {
        Paragraph paragraph = new Paragraph(documentModel);
        Section section = new Section(documentModel, paragraph);

        paragraph.Inlines.Add(new Run(documentModel, "DAĞITIM LİSTESİ") { CharacterFormat = { Bold = true } });
        paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));

        // Bilgi yoksa Geregi yazmamasi icin eklendi
        if (splittedSecondaryList.Any())
        {
            paragraph.Inlines.Add(new Run(documentModel, "GEREĞİ:") { CharacterFormat = { Bold = true } });
            paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));
        }

        foreach (string row in splittedPrimaryList)
        {
            paragraph.Inlines.Add(new Run(documentModel, row));
            paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));
        }

        paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.ColumnBreak));

        if (splittedSecondaryList.Any())
        {
            paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));
            paragraph.Inlines.Add(new Run(documentModel, "BİLGİ:") { CharacterFormat = { Bold = true } });
            paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));

            foreach (string row in splittedSecondaryList)
            {
                paragraph.Inlines.Add(new Run(documentModel, row));
                paragraph.Inlines.Add(new SpecialCharacter(documentModel, SpecialCharacterType.LineBreak));
            }
        }

        section.PageSetup.TextColumns = new TextColumnCollection(2);
        AddDefaultFooter(documentModel, section);
        documentModel.Sections.Add(section);
    }

    private void AddDefaultFooter(DocumentModel documentModel, Section section)
    {
        Section firstSection = documentModel.Sections.FirstOrDefault();
        GemBox.Document.HeaderFooter footerClone = firstSection.HeadersFooters.Where(x => x.HeaderFooterType == HeaderFooterType.FooterDefault).FirstOrDefault();
        if (footerClone != null)
        {
            GemBox.Document.HeaderFooter footer = new GemBox.Document.HeaderFooter(documentModel, HeaderFooterType.FooterDefault);
            for (int i = 0; i < footerClone.Blocks.Count(); i++)
            {
                footer.Blocks.Add(footerClone.Blocks[i].Clone(true));
            }
            section.HeadersFooters.Add(footer);
        }
    }
}

public class NVParserOld
{
    public string ValueOld { get; set; }
    public NVParserOld()
        : this(null)
    {
    }
    public NVParserOld(string value)
    {
        this.ValueOld = value ?? string.Empty;
    }
    public string GetValueOld(string paramName)
    {
        return GetValueOld(paramName, "");
    }
    public string GetValueOld(string paramName, string defaultValue)
    {
        string value = "";
        string baseOriginal = " " + ValueOld + " dummy=";
        string baseSearch = baseOriginal.Replace("\r\n", "  ").Replace("\r", " ").Replace("\n", " ");

        int startIndex = baseSearch.IndexOf(" " + paramName.Trim() + "=", StringComparison.InvariantCultureIgnoreCase);

        if (startIndex >= 0)
        {
            string dataPart = baseOriginal.Substring(startIndex + (" " + paramName.Trim() + "=").Length);

            if (dataPart.Substring(0, 1) == "{")
            {
                dataPart = dataPart.Substring(1);

                int i = 0;
                int s = 1;

                for (int k = 0; k < dataPart.Length; k++)
                {
                    string c = dataPart.Substring(k, 1);
                    s = s + (c == "{" ? 1 : 0);
                    s = s + (c == "}" ? -1 : 0);

                    if (s == 0)
                    {
                        i = k;
                        break;
                    }
                }


                dataPart = dataPart.Substring(0, i);
            }
            else
            {
                dataPart = dataPart.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                int i = dataPart.IndexOf("=");

                while (i > 0 && dataPart.Substring(i, 1) != " ")
                    i = i - 1;

                dataPart = dataPart.Substring(0, i);
            }

            value = dataPart;
        }

        value = value.Trim();
        value = value != "" ? value : defaultValue;

        return value;
    }
    public List<string> GetParamListOld()
    {
        List<string> list = new List<string>();
        int i = 0;
        int inside = 0;
        char[] stops = "\'\r\n\t=<>!,(){} ".ToCharArray();

        while (i < ValueOld.Length)
        {
            char c = ValueOld[i];
            if (c == '{') inside++;
            if (c == '}') inside--;

            if (c == '=' && inside == 0)
            {
                for (int j = i - 1; j >= -1; j--)
                {
                    bool startFound = j == -1 ? true : ValueOld.Substring(j, 1).IndexOfAny(stops) >= 0;

                    if (startFound)
                    {
                        string paramName = ValueOld.Substring(j + 1, i - j - 1);
                        list.Add(paramName);
                        break;
                    }
                }
            }

            i++;
        }

        return list;
    }
    public NVParserOld SetParamOld(string paramName, string value)
    {
        SetParamOld(paramName, value, " ");
        return this;
    }
    public NVParserOld SetParamOld(string paramName, string value, string delimiter)
    {
        List<string> list = GetParamListOld();

        if (list.Find(x => x.Equals(paramName, StringComparison.InvariantCultureIgnoreCase)) == null)
            list.Add(paramName);

        string newValue = string.Empty;

        foreach (string name in list)
        {
            string v = (name.Equals(paramName, StringComparison.InvariantCultureIgnoreCase) ? (value ?? string.Empty) : GetValueOld(name));
            newValue = newValue.Trim() + (newValue.IsNullOrEmptyOld() ? string.Empty : delimiter) + name + "={" + v + "}";
        }

        ValueOld = newValue;
        return this;
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////String Extensions (That will be remove feature)///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static class OldStringExtensionsOld
{
    public static string WildcardToRegexOld(this string value)
    {
        string result = Regex.Escape(value).
            Replace(@"\*", ".+?").
            Replace(@"\?", ".");

        if (result.EndsWith(".+?"))
        {
            result = result.Remove(result.Length - 3, 3);
            result += ".*";
        }

        return result;
    }

    public static string MidOld(this string value, int startIndex)
    {
        if (value == null)
            return null;

        return value.MidOld(startIndex, value.Length);
    }

    public static string MidOld(this string value, int startIndex, int length)
    {
        if (value == null)
            return null;

        if (startIndex >= 0 && startIndex < value.Length)
        {
            if ((startIndex + length - 1) < value.Length)
                return value.Substring(startIndex, length);
            else
                return value.Substring(startIndex, value.Length - startIndex);
        }
        else
            return null;
    }

    public static bool IsNullOrEmptyOld(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNotNullAndEmptyOld(this string value)
    {
        return !string.IsNullOrEmpty(value);
    }

    public static string HtmlPart1 = @"
<html xmlns='http://www.w3.org/1999/xhtml' xml:lang='en' lang='en'><head> <!--style_start-->

    <style type='text/css' media='all'>
          @page {
          size: 3in 11in;
            margin: 0.1in 0 0.1in 0;
  
          background:   #ffffff;
  
   
  
        @bottom-center {
  
            content:element(footer)
  
          }
  
   
  
          @top-center {
  
            content: element(header)
  
          }
  
        }  

        table {
            width: 100%;
            border: 1px solid black;
            border-spacing: 0;
            border-collapse: collapse;
        }
 
        table, td, th {
            border: 1px solid black;
        }
 
       .ck-table-resized {
            margin: auto auto;
        }
 
    </style>
  
     </head>
  
        <body> <!--style_end-->
	";

    public static string HtmlPart2 = @"
	  </body></html>
	";
}