using System;
using System.IO;
using Plugin.Media;
using Xamarin.Forms;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using Image = Syncfusion.Drawing.Image;
using SignaturePad.Forms;

namespace eZeahler5{

    public partial class MainPage : ContentPage
    {
        //Übergabevariablen
        private string  zaehlerArt = "Hallo Welt";
        private string zaehlernummer = "";
        Plugin.Media.Abstractions.MediaFile theImage;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void TakePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            theImage = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                SaveToAlbum= true,
                Name = "test.jpg"
            });

            MyImage.Source = ImageSource.FromStream(() =>
            {
                var stream = theImage.GetStream();
                return stream;
            });
        }

        private async void PickPhoto_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No upload", "Picking a photo is not supported.", "Ok");
                return;
            }
            theImage = await CrossMedia.Current.PickPhotoAsync();
            if (theImage == null)
                return;
            MyImage.Source=ImageSource.FromStream(() => theImage.GetStream());
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            zaehlerArt = eZeahlerMethods.Items[eZeahlerMethods.SelectedIndex];
        }

        //private void Entry_Completed(object sender, EventArgs e)
        //{
        //    var value = ((Entry)sender).Text;
        //}

        private void Entry_Completed_Stand(object sender, EventArgs e)
        {
            var valueStand = ZählerStand.Text;
        }

        private void Entry_Completed_Nummer(object sender, EventArgs e)
        {
            zaehlernummer = ZählerNummer.Text;
        }
        private async System.Threading.Tasks.Task PdfButtonClicked(object sender, EventArgs e)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();

            //Add a page to the document
            PdfPage page = document.Pages.Add();

            //Create PDF graphics for the page
            PdfGraphics graphics = page.Graphics;

            //Set the standard font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

            //Draw the text
            graphics.DrawString("Zähler Art: " + zaehlerArt, font, PdfBrushes.Black, new PointF(0, 20));
            graphics.DrawString("Zähler Stände: " + ZählerStand.Text, font, PdfBrushes.Black, new PointF(0, 50));
            graphics.DrawString("Zähler Nummer: " + zaehlernummer, font, PdfBrushes.Black, new PointF(0, 85));
            if ( theImage != null)
            {
                Stream imageStream = theImage.GetStream();

                PdfBitmap imageForPdf = new PdfBitmap(imageStream);

                graphics.DrawImage(imageForPdf, 0, 150, 200, 200);
            }
            
            Stream signatureStream = await signatureView.GetImageStreamAsync(SignatureImageFormat.Jpeg);

            PdfBitmap imageForsignature = new PdfBitmap(signatureStream);

            graphics.DrawImage(imageForsignature, 250, 150, 200, 200);

            //Save the document to the stream
            MemoryStream stream = new MemoryStream();
            document.Save(stream);

            //Close the document
            document.Close(true);

            //Save the stream as a file in the device and invoke it for viewing
            DependencyService.Get<ISave>().SaveAndView("Output.pdf", "application / pdf", stream);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    } 
}
