using Spire.Doc;
using Spire.Doc.Documents;
using System.Drawing;

namespace RentalCheckIn.Services.Core;

public class PDFService : IPDFService
{
    private readonly IWebHostEnvironment environment;
    private readonly ILogger<PDFService> logger;

    public PDFService(IWebHostEnvironment environment, ILogger<PDFService> logger)
    {
        this.environment = environment;
        this.logger = logger;
    }

    public MemoryStream FillCheckInFormAsync(CheckInReservationDTO model, string culture)
    {
        var template = "CheckInForm-English.docx";

        if (culture != null) 
        { 
            if (culture == "nl-NL")
            {
                template = "CheckInForm-Dutch.docx";
            }
        }

        string templatePath = Path.Combine(environment.WebRootPath, "templates", template);
        string outputDir = Path.Combine(environment.WebRootPath, "output");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Generate a unique filename using a GUID
        string uniqueFileName = $"GeneratedCheckInForm_{Guid.NewGuid()}.docx";
        string outputPath = Path.Combine(outputDir, uniqueFileName);
        // Load the template document
        Document doc = new Document();
        doc.LoadFromFile(templatePath);
        // Data for Mail Merge
        Dictionary<string, string> mergeData = new Dictionary<string, string>
        {
            { "Country", model.CountryISO2 },
            { "Language", model.LanguageName },
            { "GuestFirstName", model.GuestFirstName },
            { "CheckInDate", model.CheckInDate.ToString("MM/dd/yyyy") },
            { "CheckOutDate", model.CheckOutDate.ToString("MM/dd/yyyy") },
            { "PassportNr", model.PassportNr },
            { "MailAddress", model.MailAddress },
            { "Mobile", model.Mobile },
            { "NumberOfNights", model.NumberOfNights.ToString() },
            { "NumberOfGuests", model.NumberOfGuests.ToString() },
            { "ApartmentName", model.ApartmentName },
            { "ApartmentFee", model.ApartmentFee.ToString("F2") },
            { "SecurityDeposit", model.SecurityDeposit.ToString("F2") },
            { "TotalPrice", model.TotalPrice.ToString() },
            { "KwhAtCheckIn", model.KwhAtCheckIn.ToString() },
            { "GuestFullName", model.GuestFullName },
            { "SignatureDataUrl", "placeholderpath" }
        };

        // Specify the field names and values from the dictionary
        string[] fieldNames = new string[mergeData.Count];
        string[] fieldValues = new string[mergeData.Count];
        int index = 0;

        foreach (var kvp in mergeData)
        {
            fieldNames[index] = kvp.Key;
            fieldValues[index] = kvp.Value;
            index++;
        }

        Image? signatureImage = null;

        // Insert the signature image if available
        if (!string.IsNullOrEmpty(model.SignatureDataUrl))
        {

            // Insert the signature image if available
            if (!string.IsNullOrEmpty(model.SignatureDataUrl))
            {
                try
                {

                    //Decode Base64 string into an Image
                    string base64String = model.SignatureDataUrl.Contains(",")
                        ? model.SignatureDataUrl.Split(',')[1] // Remove the "data:image/png;base64," prefix
                        : model.SignatureDataUrl;
                    // Step 1: Convert Base64 string to byte array
                    byte[] utf8Bytes = Convert.FromBase64String(base64String);
                    // Step 2: Decode UTF8 byte array to string
                    string decodedString = Encoding.UTF8.GetString(utf8Bytes);
                    string base64String2 = decodedString.Contains(",")
                        ? decodedString.Split(',')[1] // Remove the "data:image/png;base64," prefix
                        : decodedString;
                    byte[] imageBytes = Convert.FromBase64String(base64String2);

                    using (var ms = new MemoryStream(imageBytes))
                    {
                        signatureImage = Image.FromStream(ms);
                    }

                    // Register event for handling the image field
                    doc.MailMerge.MergeImageField += (sender, field) =>
                    {
                        field.Image = signatureImage;
                    };

                    // Mapping of apartment names to aliases
                    Dictionary<string, string> hotelAliasMapping = new Dictionary<string, string>
                        {
                            { "Snowy (street view)", "Snowy" },
                            { "Massi (garden view)", "Massi" }
                        };

                    // Get the alias for the selected apartment
                    if (hotelAliasMapping.TryGetValue(model.ApartmentName, out string? targetAlias))
                    {
                        // Check the checkbox corresponding to the alias
                        ChooseApartment(doc, targetAlias);
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An unexpected error occurred in PDFService while trying to fill Check-In Form .docx template.");
                }
            }

            // Perform the mail merge
            doc.MailMerge.Execute(fieldNames, fieldValues);

        }

        // Save to a MemoryStream as PDF
        var pdfStream = new MemoryStream();
        doc.SaveToStream(pdfStream, FileFormat.PDF);
        pdfStream.Position = 0;

        return pdfStream;
    }

    private void ChooseApartment(Document document, string targetAlias)
    {
        try
        {
            // Traverse sections, tables, and cells to find and update checkboxes
            foreach (Section section in document.Sections)
            {
                foreach (DocumentObject obj in section.Body.ChildObjects)
                {
                    if (obj is Table table)
                    {
                        foreach (TableRow row in table.Rows)
                        {
                            foreach (TableCell cell in row.Cells)
                            {
                                foreach (Paragraph paragraph in cell.Paragraphs)
                                {
                                    foreach (DocumentObject pobj in paragraph.ChildObjects)
                                    {
                                        if (pobj is StructureDocumentTagInline sdt &&
                                            sdt.SDTProperties.SDTType == SdtType.CheckBox)
                                        {
                                            // Check the alias
                                            if (sdt.SDTProperties.Alias == targetAlias)
                                            {
                                                // Update checkbox state
                                                SdtCheckBox checkBox = sdt.SDTProperties.ControlProperties as SdtCheckBox;
                                                if (checkBox != null)
                                                {
                                                    // Set checkbox to checked
                                                    checkBox.Checked = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred while trying to Choose ");
        }
    }

}
