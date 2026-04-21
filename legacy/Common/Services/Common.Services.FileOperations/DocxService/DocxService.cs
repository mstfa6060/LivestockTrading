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
using Common.Definitions.Base.Enums;

namespace Common.Services.FileOperations;

public class DocxService
{
    public DocxService()
    {
        ComponentInfo.SetLicense("DN-2024Jan08-wMMlyE2pW1c4VIBQA0EJ2bSfv05Hb9dMplpG4H4fgyIMV/FhdRM8v3EubxD/SkiArTLzUTfxHtt1OhExeqBXl7q55Zg==A");

        // ComponentInfo.SetLicense("DN-2023Dec22-2024Jan22-pUSuoodqTN29ZbVbgW4ez492B0QIY6eI2pgSPHHKBd/MWA/xnAbYP9A2GgJNKyJQ1HdKKzWqUIbMlcljVWBtmhxIbXg==B");


        // Set license key to use GemBox.Document in a Free mode.
        // ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        // Continue to use the component in a Trial mode when free limit is reached.
        // ComponentInfo.FreeLimitReached += (sender, e) => e.FreeLimitReachedAction = FreeLimitReachedAction.ContinueAsTrial;
    }

    public byte[] ReplaceDocxContent(byte[] defaultDocxBytes, List<ReplacementItem> replacementData, byte[] companyDocxBytes, CorrespondanceTypes correspondanceType)
    {
        var isAttachmentsInAnotherPage = false;
        // var isDistributionsInAnotherPage = false;
        var isDocumentHaveDistibutions = replacementData.Any(a => (a.Key == TemplateConstant.DISTRIBUTION_PROPERLY && a.Values.Count > 0) || (a.Key == TemplateConstant.DISTRIBUTION_INFORMATION && a.Values.Count > 0));

        Stream stream = new MemoryStream(defaultDocxBytes);
        DocumentModel documentModel = DocumentModel.Load(stream, GemBox.Document.LoadOptions.DocxDefault);

        Stream stream2 = new MemoryStream(companyDocxBytes);
        DocumentModel companyDocumentModel = DocumentModel.Load(stream2, GemBox.Document.LoadOptions.DocxDefault);

        // var documentModel = templateDocumentModel;

        var mapping = new ImportMapping(companyDocumentModel, documentModel, true);

        // Import section from source document to destination document.
        this.SetHeaderFooter(companyDocumentModel, documentModel, mapping);

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
                    case TemplateConstant.X_Creator_Organization:
                        break;

                    case TemplateConstant.DISTRIBUTION_PROPERLY:
                    case TemplateConstant.DISTRIBUTION_INFORMATION:
                    case TemplateConstant.DOCUMENT_NO:
                    case TemplateConstant.DOCUMENT_DATE:
                    case TemplateConstant.SUBJECT:
                    case TemplateConstant.ESIGNED_LABEL:
                    case TemplateConstant.ACCESS_CODE:
                    case TemplateConstant.AUTHOR_NAMESURNAME:
                    case TemplateConstant.AUTHOR_TITLE:
                    case TemplateConstant.INTERLOCUTOR:
                    case TemplateConstant.ATTACHMENT:
                    case TemplateConstant.PRIORITY:
                    case TemplateConstant.VALIDATION_URL:
                        value = replacementItem.GetValue();
                        this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
                        break;

                    case TemplateConstant.WRITING:
                        value = replacementItem.GetValue();
                        var cleanEditorContent = this.CleanEditorContent(value);
                        this.ChangeText(replacementContentRange, cleanEditorContent, replacementItem.ContentType);
                        break;

                    case TemplateConstant.REFERENCE:
                        if (replacementItem.Values.Count == 0)
                        {
                            documentModel.GetChildElements(true, ElementType.TableRow)
                                                   .Where(r => r.Content.ToString().Contains(TemplateConstant.REFERENCE))
                                                   .ToList()
                                                   .ForEach(r => r.Content.Delete());
                        }
                        else
                        {
                            value = replacementItem.GetValue();
                            this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
                        }
                        break;

                    case TemplateConstant.APPROVER_1:
                    case TemplateConstant.APPROVER_3:
                    case TemplateConstant.APPROVER_2:
                        value = replacementItem.GetValue();
                        this.ChangeText(replacementContentRange, value, replacementItem.ContentType);
                        break;

                    default:
                        throw new Exception($"Replacement Data Not Handled. Key: {replacementItem.Key}");
                }
            }
        }

        #region Approver

        ContentRange correspondanceStart = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.CORRESPONDANCE_START)).FirstOrDefault();
        ContentRange correspondanceEnd = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.APPROVE_START)).FirstOrDefault();
        ContentRange approveStart = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.APPROVE_START)).FirstOrDefault();
        ContentRange approveEnd = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.APPROVE_END)).FirstOrDefault();

        switch (correspondanceType)
        {
            case CorrespondanceTypes.OLUR:

                new ContentRange(correspondanceStart.Start, approveStart.Start).Delete();

                documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.APPROVE_START))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.APPROVE_END))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                break;

            case CorrespondanceTypes.RESMI:
            default:

                new ContentRange(correspondanceEnd.Start, approveEnd.Start).Delete();

                documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.CORRESPONDANCE_START))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.APPROVE_END))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                break;
        }

        #endregion


        #region Attachment

        ContentRange attachmentStart = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.ATTACHMENT_START)).FirstOrDefault();
        ContentRange attachmentEnd = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.ATTACHMENT_END)).FirstOrDefault();
        ContentRange attachmentLabel = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.ATTACHMENT_LABEL)).FirstOrDefault();

        ContentRange distributionStart = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.DISTRIBUTION_START)).FirstOrDefault();

        bool isAttachmentNotExist = false;

        if (attachmentLabel != null && attachmentStart != null && attachmentEnd != null)
        {
            var attachmentItem = replacementData.Where(d => d.Key == TemplateConstant.ATTACHMENT).FirstOrDefault();

            var attachmentDeliveryStringList = attachmentItem.Values.ToList();
            int attachmentCount = attachmentDeliveryStringList.Count;

            if (attachmentCount == 0)
            {
                isAttachmentNotExist = true;
                new ContentRange(attachmentLabel.Start, attachmentEnd.Start).Delete();

                documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.ATTACHMENT_END))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
            }
            else if (attachmentCount == 1)
            {
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.ATTACHMENT_START))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.ATTACHMENT_END))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
            }
            else
            {
                if (GetPageNumber(attachmentStart.Start) == GetPageNumber(attachmentEnd.End))
                {
                    documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.ATTACHMENT_START))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
                    documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.ATTACHMENT_END))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
                }
                else
                {
                    isAttachmentsInAnotherPage = true;

                    new ContentRange(attachmentStart.Start, distributionStart.Start).Delete();

                    for (int i = 0; i < attachmentDeliveryStringList.Count; i++)
                    {
                        var attachmentName = attachmentDeliveryStringList[i].Split($"{i + 1})");
                        attachmentDeliveryStringList[i] = $"{i + 1}){attachmentName[1]}";
                    }

                    AddDocAddListAnotherPage(documentModel, attachmentDeliveryStringList);

                    if (attachmentLabel != null)
                        this.ChangeText(attachmentLabel, $"Ek: Ek Listesi ({attachmentCount} Adet)", ReplacementItemContentTypes.Text);
                    else
                        System.Console.WriteLine("Attachment label is null");
                }
            }
        }
        #endregion

        #region Distribution

        ContentRange distributionEnd = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.DISTRIBUTION_END)).FirstOrDefault();
        ContentRange distributionLabel = documentModel.Content.Find(TemplateConstant.GetSearchKey(TemplateConstant.DISTRIBUTION_LABEL)).FirstOrDefault();

        var distributionProperly = replacementData.Where(d => d.Key == TemplateConstant.DISTRIBUTION_PROPERLY).FirstOrDefault();
        var distributionInformation = replacementData.Where(d => d.Key == TemplateConstant.DISTRIBUTION_INFORMATION).FirstOrDefault();
        var splittedDistibutionProperlyList = distributionProperly.Values.ToList();
        var splittedDistributionInformationList = distributionInformation.Values.ToList();
        int totalDistributionCount = splittedDistibutionProperlyList.Count + splittedDistributionInformationList.Count;

        switch (correspondanceType)
        {
            case CorrespondanceTypes.OLUR:
            case CorrespondanceTypes.SATINALMA:
            case CorrespondanceTypes.KOMISYON:
            case CorrespondanceTypes.DISIPLIN:
                System.Console.WriteLine($"Distribution Start is null: {distributionStart == null}");
                System.Console.WriteLine($"Distribution End is null: {distributionEnd == null}");
                new ContentRange(distributionStart.Start, distributionEnd.Start).Delete();

                documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_LABEL))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
                documentModel.GetChildElements(true, ElementType.TableRow)
                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                        .ToList()
                                                        .ForEach(r => r.Content.Delete());
                break;

            case CorrespondanceTypes.RESMI:
                if (splittedDistibutionProperlyList.Count > 0)
                    // For first value is "Dağıtım"
                    splittedDistibutionProperlyList.Remove(splittedDistibutionProperlyList[0]);

                if (totalDistributionCount < 2)
                {
                    new ContentRange(distributionStart.Start, distributionEnd.Start).Delete();

                    documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_LABEL))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                    documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                }
                else
                {
                    if (distributionLabel != null && distributionStart != null && distributionEnd != null)
                    {
                        if (GetPageNumber(distributionStart.Start) == GetPageNumber(distributionEnd.End))
                        {
                            if (isAttachmentNotExist || isAttachmentsInAnotherPage)
                            {
                                documentModel.GetChildElements(true, ElementType.TableRow)
                                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_LABEL))
                                                                        .ToList()
                                                                        .ForEach(r => r.Content.Delete());
                                documentModel.GetChildElements(true, ElementType.TableRow)
                                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_START))
                                                                        .ToList()
                                                                        .ForEach(r => r.Content.Delete());
                                documentModel.GetChildElements(true, ElementType.TableRow)
                                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                                        .ToList()
                                                                        .ForEach(r => r.Content.Delete());
                            }
                        }
                        else
                        {
                            new ContentRange(distributionStart.Start, distributionEnd.Start).Delete();
                            documentModel.GetChildElements(true, ElementType.TableRow)
                                                                       .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                                       .ToList()
                                                                       .ForEach(r => r.Content.Delete());

                            if (splittedDistributionInformationList.Count > 0)
                            {
                                splittedDistributionInformationList.Remove(splittedDistributionInformationList[0]);
                            }
                            // if (splittedDistibutionProperlyList.Count > 0)
                            // {
                            //     splittedDistibutionProperlyList.Remove(splittedDistibutionProperlyList[0]);
                            // }

                            if (splittedDistributionInformationList.Count > 0 && splittedDistibutionProperlyList.Count > 0)
                            {
                                splittedDistibutionProperlyList.Remove(splittedDistibutionProperlyList[0]);
                                splittedDistributionInformationList.Remove(splittedDistributionInformationList[0]);
                            }

                            AddDeliveryListAnotherPage(documentModel, splittedDistibutionProperlyList, splittedDistributionInformationList);
                            totalDistributionCount = splittedDistibutionProperlyList.Count + splittedDistributionInformationList.Count;

                            if (distributionLabel != null)
                            {
                                if (!isAttachmentsInAnotherPage && !isAttachmentNotExist)
                                    distributionLabel.LoadText("\nDağıtım:\nDağıtım Listesi (" + totalDistributionCount + " Muhatap)");
                                else
                                    distributionLabel.LoadText("Dağıtım:\nDağıtım Listesi (" + totalDistributionCount + " Muhatap)");
                            }
                            else
                                System.Console.WriteLine("Distribution label is null");
                        }
                    }
                }
                break;

            case CorrespondanceTypes.ADK:
            case CorrespondanceTypes.EXGRATIA:
            case CorrespondanceTypes.HASAR:
            case CorrespondanceTypes.HASARPARTNER:
            case CorrespondanceTypes.IHALEKOMITESI:
            case CorrespondanceTypes.KALITE:
            case CorrespondanceTypes.KAMPANYA:
            case CorrespondanceTypes.MUNFESIH:
            case CorrespondanceTypes.TEMINAT:
            case CorrespondanceTypes.URUN:
            case CorrespondanceTypes.WEBSERVIS:
            case CorrespondanceTypes.HASARFESIHNAME:
            default:
                if (splittedDistibutionProperlyList.Count > 0)
                    // For first value is "Dağıtım"
                    splittedDistibutionProperlyList.Remove(splittedDistibutionProperlyList[0]);


                if (totalDistributionCount < 1)
                {
                    new ContentRange(distributionStart.Start, distributionEnd.Start).Delete();

                    documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_LABEL))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                    documentModel.GetChildElements(true, ElementType.TableRow)
                                                            .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                            .ToList()
                                                            .ForEach(r => r.Content.Delete());
                }
                else
                {
                    if (distributionLabel != null && distributionStart != null && distributionEnd != null)
                    {
                        if (GetPageNumber(distributionStart.Start) == GetPageNumber(distributionEnd.End))
                        {
                            if (isAttachmentNotExist || isAttachmentsInAnotherPage)
                            {
                                documentModel.GetChildElements(true, ElementType.TableRow)
                                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_LABEL))
                                                                        .ToList()
                                                                        .ForEach(r => r.Content.Delete());
                                documentModel.GetChildElements(true, ElementType.TableRow)
                                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_START))
                                                                        .ToList()
                                                                        .ForEach(r => r.Content.Delete());
                                documentModel.GetChildElements(true, ElementType.TableRow)
                                                                        .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                                        .ToList()
                                                                        .ForEach(r => r.Content.Delete());
                            }
                        }
                        else
                        {
                            new ContentRange(distributionStart.Start, distributionEnd.Start).Delete();
                            documentModel.GetChildElements(true, ElementType.TableRow)
                                                                       .Where(r => r.Content.ToString().Contains(TemplateConstant.DISTRIBUTION_END))
                                                                       .ToList()
                                                                       .ForEach(r => r.Content.Delete());

                            if (splittedDistributionInformationList.Count > 0)
                            {
                                splittedDistributionInformationList.Remove(splittedDistributionInformationList[0]);
                            }
                            // if (splittedDistibutionProperlyList.Count > 0)
                            // {
                            //     splittedDistibutionProperlyList.Remove(splittedDistibutionProperlyList[0]);
                            // }

                            if (splittedDistributionInformationList.Count > 0 && splittedDistibutionProperlyList.Count > 0)
                            {
                                splittedDistibutionProperlyList.Remove(splittedDistibutionProperlyList[0]);
                                splittedDistributionInformationList.Remove(splittedDistributionInformationList[0]);
                            }

                            AddDeliveryListAnotherPage(documentModel, splittedDistibutionProperlyList, splittedDistributionInformationList);
                            totalDistributionCount = splittedDistibutionProperlyList.Count + splittedDistributionInformationList.Count;

                            if (distributionLabel != null)
                            {
                                if (!isAttachmentsInAnotherPage && !isAttachmentNotExist)
                                    distributionLabel.LoadText("\nDağıtım:\nDağıtım Listesi (" + totalDistributionCount + " Muhatap)");
                                else
                                    distributionLabel.LoadText("Dağıtım:\nDağıtım Listesi (" + totalDistributionCount + " Muhatap)");
                            }
                            else
                                System.Console.WriteLine("Distribution label is null");
                        }
                    }
                }
                break;

        }


        #endregion

        // clear all other "{...}" strings
        for (int i = 0; i < 10; i++) // if exceeded ~10 there may be some errors/bugs 
        {
            bool found = false;

            foreach (ContentRange contentRange in documentModel.Content.Find(new Regex(TemplateConstant.STAR.WildcardToRegex())).Reverse())
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

        MemoryStream outputDocx = new MemoryStream();
        documentModel.Save(outputDocx, GemBox.Document.SaveOptions.DocxDefault);

        byte[] newDocxBytes = new byte[outputDocx.Length];
        outputDocx.Read(newDocxBytes, 0, (int)outputDocx.Length);

        return newDocxBytes;
    }

    private void SetHeaderFooter(DocumentModel companyDocumentModel, DocumentModel documentModel, ImportMapping mapping)
    {
        var targetSection = companyDocumentModel.Sections.FirstOrDefault();
        var sourceSection = documentModel.Sections.FirstOrDefault();
        var copyTargetSection = documentModel.Import(targetSection, true, mapping);

        if (sourceSection.HeadersFooters.Count > 0)
            sourceSection.HeadersFooters.RemoveAt(0);


        GemBox.Document.HeaderFooter headerClone = copyTargetSection.HeadersFooters.Where(x => x.HeaderFooterType == HeaderFooterType.HeaderDefault).FirstOrDefault();
        if (headerClone != null)
        {
            GemBox.Document.HeaderFooter header = new GemBox.Document.HeaderFooter(documentModel, HeaderFooterType.HeaderDefault);
            for (int i = 0; i < headerClone.Blocks.Count(); i++)
            {
                header.Blocks.Add(headerClone.Blocks[i].Clone(true));
            }
            sourceSection.HeadersFooters.Add(header);
        }

        // Section firstSection = documentModel.Sections.FirstOrDefault();
        GemBox.Document.HeaderFooter footerClone = copyTargetSection.HeadersFooters.Where(x => x.HeaderFooterType == HeaderFooterType.FooterDefault).FirstOrDefault();
        if (footerClone != null)
        {
            GemBox.Document.HeaderFooter footer = new GemBox.Document.HeaderFooter(documentModel, HeaderFooterType.FooterDefault);
            for (int i = 0; i < footerClone.Blocks.Count(); i++)
            {
                footer.Blocks.Add(footerClone.Blocks[i].Clone(true));
            }
            sourceSection.HeadersFooters.Add(footer);
        }
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

        // System.Console.WriteLine("------------------------------==========================");
        // System.Console.WriteLine(newEditorContent);
        // System.Console.WriteLine("------------------------------");

        return OldStringExtensions.HtmlPart1 + newEditorContent + OldStringExtensions.HtmlPart2;
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

    private void AddDocAddListAnotherPage(DocumentModel documentModel, List<string> docDeliveryStringList)
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

    private void AddDeliveryListAnotherPage(DocumentModel documentModel, List<string> splittedPrimaryList, List<string> splittedSecondaryList)
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

public class NVParser
{
    public string Value { get; set; }
    public NVParser()
        : this(null)
    {
    }
    public NVParser(string value)
    {
        this.Value = value ?? string.Empty;
    }
    public string GetValue(string paramName)
    {
        return GetValue(paramName, "");
    }
    public string GetValue(string paramName, string defaultValue)
    {
        string value = "";
        string baseOriginal = " " + Value + " dummy=";
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
    public List<string> GetParamList()
    {
        List<string> list = new List<string>();
        int i = 0;
        int inside = 0;
        char[] stops = "\'\r\n\t=<>!,(){} ".ToCharArray();

        while (i < Value.Length)
        {
            char c = Value[i];
            if (c == '{') inside++;
            if (c == '}') inside--;

            if (c == '=' && inside == 0)
            {
                for (int j = i - 1; j >= -1; j--)
                {
                    bool startFound = j == -1 ? true : Value.Substring(j, 1).IndexOfAny(stops) >= 0;

                    if (startFound)
                    {
                        string paramName = Value.Substring(j + 1, i - j - 1);
                        list.Add(paramName);
                        break;
                    }
                }
            }

            i++;
        }

        return list;
    }
    public NVParser SetParam(string paramName, string value)
    {
        SetParam(paramName, value, " ");
        return this;
    }
    public NVParser SetParam(string paramName, string value, string delimiter)
    {
        List<string> list = GetParamList();

        if (list.Find(x => x.Equals(paramName, StringComparison.InvariantCultureIgnoreCase)) == null)
            list.Add(paramName);

        string newValue = string.Empty;

        foreach (string name in list)
        {
            string v = (name.Equals(paramName, StringComparison.InvariantCultureIgnoreCase) ? (value ?? string.Empty) : GetValue(name));
            newValue = newValue.Trim() + (newValue.IsNullOrEmpty() ? string.Empty : delimiter) + name + "={" + v + "}";
        }

        Value = newValue;
        return this;
    }
}


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////String Extensions (That will be remove feature)///////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static class OldStringExtensions
{
    public static string WildcardToRegex(this string value)
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

    public static string Mid(this string value, int startIndex)
    {
        if (value == null)
            return null;

        return value.Mid(startIndex, value.Length);
    }

    public static string Mid(this string value, int startIndex, int length)
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

    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static bool IsNotNullAndEmpty(this string value)
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