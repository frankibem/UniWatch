using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /// <summary>
        /// Train the recognizer for the given class
        /// </summary>
        /// <param name="groupId">The id for the group to train</param>        
        /// <remarks>Returns when the training is compolete</remarks>
        public async Task TrainRecognizer(string groupId)
        {
            var faceClient = GetFaceClient();
            await faceClient.TrainPersonGroupAsync(groupId);

            TrainingStatus trainingStatus = null;
            while(true)
            {
                trainingStatus = await faceClient.GetPersonGroupTrainingStatusAsync(groupId);
                if(trainingStatus.Status != Status.Running)
                    break;

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Detect the students in the given images
        /// </summary>
        /// <param name="groupId">The id for the group to detect faces in</param>
        /// <param name="images">The images to detect faces in</param>
        /// <returns>A list of ids for detected students</returns>
        public IEnumerable<Guid> DetectStudents(string groupId, IEnumerable<Models.UploadedImage> images)
        {
            HashSet<Guid> detectedStudents = new HashSet<Guid>();
            var faceClient = GetFaceClient();

            foreach(var image in images)
            {
                var detectedFaces = Task.Run(() => faceClient.DetectAsync(image.Url)).Result;

                var faceIds = detectedFaces.Select(f => f.FaceId).ToArray();
                var identifyResults = Task.Run(() => faceClient.IdentifyAsync(groupId, faceIds)).Result;

                foreach(var result in identifyResults)
                {
                    if(result.Candidates.Length == 0)
                        continue;

                    detectedStudents.Add(result.Candidates[0].PersonId);
                }
            }

            return detectedStudents;
        }

        /// <summary>
        /// Clears all data stored in the service
        /// </summary>
        public static void ClearAll()
        {
            var faceClient = GetFaceClient();
            var personGroups = Task.Run(() => faceClient.ListPersonGroupsAsync()).Result;

            foreach(var group in personGroups)
            {
                Task.Run(() => faceClient.DeletePersonGroupAsync(group.PersonGroupId)).Wait();
            }
        }
    }
}