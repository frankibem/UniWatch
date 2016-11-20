using Microsoft.ProjectOxford.Face;
using System.Web.Configuration;

namespace UniWatch.Services
{
    public class RecognitionService
    {
        /// <summary>
        /// Creates and returns a new FaceServiceClient
        /// </summary>
        public static IFaceServiceClient GetFaceClient()
        {
            var subscriptionKey = WebConfigurationManager.AppSettings["CognitiveSubscriptionKey"];
            return new FaceServiceClient(subscriptionKey);
        }
    }
}