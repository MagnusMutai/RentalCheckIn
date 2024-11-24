using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Doc.Reporting;
using System.Drawing;



namespace RentalCheckIn.Services.Core;

public class PDFService : IPDFService
{
    private readonly IWebHostEnvironment environment;

    public PDFService(IWebHostEnvironment environment) 
    {
        this.environment = environment;
    }

    public void FillCheckInFormAsync(CheckInReservationDTO model, byte[] sigImage)
    {
        string templatePath = Path.Combine(environment.WebRootPath, "templates", "CheckInForm-English.docx");
        string outputPath = Path.Combine(environment.WebRootPath, "output", "GeneratedCheckInForm.docx");


        // Load the template document
        Document doc = new Document();
        doc.LoadFromFile(templatePath);

        // Data for Mail Merge
        Dictionary<string, string> mergeData = new Dictionary<string, string>
        {
            { "GuestFirstName", model.GuestFirstName },
            { "CheckInDate", model.CheckInDate.ToString("MM/dd/yyyy") },
            { "CheckOutDate", model.CheckOutDate.ToString("MM/dd/yyyy") },
            { "SelectedHotel", "Massi" }, // Assuming SelectedHotel is a property in the model
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
            { "SignatureDataUrl", "C:\\Users\\Magnu\\source\\repos\\RentalCheckIn\\RentalCheckIn\\wwwroot\\images\\snowylodge.jpg" }
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

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error inserting signature: {ex.Message}");
                        throw;
                    }
                }



                // Register an event which occurs when merging the image filed
                doc.MailMerge.MergeImageField += new MergeImageFieldEventHandler(MailMerge_MergeImageField);




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

        Console.WriteLine($"Generated document saved at: {outputPath}");
    }

    // Fill an image field with a picture
    private static void MailMerge_MergeImageField(object sender, MergeImageFieldEventArgs field)
    {
        string filePath = field.FieldValue as string;

        if (!string.IsNullOrEmpty(filePath))
        {
            field.Image = Image.FromFile(filePath);
        }
    }

    public class SignatureHelper
    {
        public Image ConvertBase64ToImage(string base64String)
        {
            try
            {
                // Remove the data:image/png;base64, prefix if present
                string base64Data = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

                // Convert Base64 string to an image
                byte[] imageBytes = Convert.FromBase64String(base64Data);
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting Base64 string to image: {ex.Message}");
                return null;
            }
        }
    }

}
