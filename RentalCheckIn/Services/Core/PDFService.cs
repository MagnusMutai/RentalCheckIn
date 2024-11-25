using Spire.Doc;
using Spire.Doc.Documents;
using System.Drawing;

namespace RentalCheckIn.Services.Core;

public class PDFService : IPDFService
{
    private readonly IWebHostEnvironment environment;

    public PDFService(IWebHostEnvironment environment) 
    {
        this.environment = environment;
    }

    public string FillCheckInFormAsync(CheckInReservationDTO model)
    {
        string templatePath = Path.Combine(environment.WebRootPath, "templates", "CheckInForm-English.docx");
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
        Image signatureImage = null;
        Image resizedImage = null;

        // Insert the signature image if available
        if (!string.IsNullOrEmpty(model.SignatureDataUrl))
        {
            try
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


                        //byte[] photoBytes = File.ReadAllBytes("wwwroot/images/snowylodge.jpg");
                        // Save the decoded bytes to a file for debugging
                        File.WriteAllBytes("test_image.png", imageBytes);
                        Console.WriteLine("Image saved successfully for debugging.");


                        using (var ms = new MemoryStream(imageBytes))
                        {
                            signatureImage = Image.FromStream(ms);

                            // Resize the image
                            int targetWidth = 120; // Specify the desired width
                            int targetHeight = 120; // Specify the desired height
                            resizedImage = ResizeImage(signatureImage, targetWidth, targetHeight);
                        }

                        // Register event for handling the image field
                        doc.MailMerge.MergeImageField += (sender, field) =>
                        {
                            field.Image = resizedImage;
                        };

                        // Mapping of hotel view names to aliases
                        Dictionary<string, string> hotelAliasMapping = new Dictionary<string, string>
                        {
                            { "Snowy (street view)", "Snowy" },
                            { "Massi (garden view)", "Massi" }
                        };

                        // Get the alias for the selected view of the lodge
                        if (hotelAliasMapping.TryGetValue(model.ApartmentName, out string? targetAlias))
                        {
                            // Check the checkbox corresponding to the alias
                            ChooseViewOfLodge(doc, targetAlias);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error inserting signature: {ex.Message}");
                        throw;
                    }
                }

                // Perform the mail merge
                doc.MailMerge.Execute(fieldNames, fieldValues);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting signature: {ex.Message}");
                throw;
            }
        }
        // Save the generated document
        doc.SaveToFile(outputPath, FileFormat.Docx);
        return uniqueFileName;
    }


    // Method to resize an image
    private static Image ResizeImage(Image originalImage, int width, int height)
    {
        Bitmap resizedImage = new Bitmap(width, height);
        using (Graphics graphics = Graphics.FromImage(resizedImage))
        {
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.DrawImage(originalImage, 0, 0, width, height);
        }
        return resizedImage;
    }

    static void ChooseViewOfLodge(Document document, string targetAlias)
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
        catch(Exception ex) 
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

}
