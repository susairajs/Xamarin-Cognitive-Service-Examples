using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using XamarinCognitive.Models;
using Newtonsoft.Json;
namespace XamarinCognitive
{
    public partial class MainPage : ContentPage
    {
        public string subscriptionKey = "26d1b6941e3a422c880639fdcdcf069b";

        public string uriBase = "https://southeastasia.api.cognitive.microsoft.com/face/v1.0/detect";

        public MainPage()
        {
            InitializeComponent();
        }

        async void btnPick_Clicked(object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null) return;
                imgSelected.Source = ImageSource.FromStream(() => {
                    var stream = file.GetStream();
                    return stream;
                });
                MakeAnalysisRequest(file.Path);
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }

        }


        public async void MakeAnalysisRequest(string imageFilePath)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
                "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            string uri = uriBase + "?" + requestParameters;
            HttpResponseMessage response;
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                string contentString = await response.Content.ReadAsStringAsync();

                List<ResponseModel> faceDetails = JsonConvert.DeserializeObject<List<ResponseModel>>(contentString);
                if(faceDetails.Count!=0)
                {
                    lblHappiness.Text = "Happiness : " + faceDetails[0].faceAttributes.emotion.happiness;
                    lblSadness.Text = "Sadness : " + faceDetails[0].faceAttributes.emotion.sadness;
                    lblAnger.Text = "Anger : " + faceDetails[0].faceAttributes.emotion.anger;
                    lblFear.Text = "Fear : " + faceDetails[0].faceAttributes.emotion.fear;
                    lblNeutral.Text = "Neutral : " + faceDetails[0].faceAttributes.emotion.neutral;
                    lblSurprise.Text = "Surprise : " + faceDetails[0].faceAttributes.emotion.surprise;
                    lblDisgust.Text = "Disgust : " + faceDetails[0].faceAttributes.emotion.disgust;
                    lblContempt.Text = "Contempt : " + faceDetails[0].faceAttributes.emotion.contempt;
                }

            }
        }
        public byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
