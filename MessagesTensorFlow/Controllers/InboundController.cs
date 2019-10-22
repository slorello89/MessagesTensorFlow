using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace MessagesTensorFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InboundController : ControllerBase
    {
        public static Dictionary<string, string> _pendingTrainLabels = new Dictionary<string, string>();
        public IConfiguration Configuration { get; set; }
        public InboundController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpPost]
        public HttpStatusCode Post([FromBody]InboundMessage message)
        {
            const string TRAIN = "train";
            try
            {
                Debug.WriteLine(JsonConvert.SerializeObject(message));
                if (!string.IsNullOrEmpty(message.message.content.text))
                {
                    var split = message.message.content.text.Split(new[] { ' ' }, 2);
                    if (split.Length > 1)
                    {
                        if (split[0].ToLower() == TRAIN)
                        {
                            var label = split[1];
                            var requestor = message.from.id;
                            if (!_pendingTrainLabels.ContainsKey(requestor))
                            {
                                _pendingTrainLabels.Add(requestor, label);
                            }
                            else
                            {
                                _pendingTrainLabels[requestor] = label;
                            }
                        }
                    }
                }
                if (_pendingTrainLabels.ContainsKey(message.from.id) && message.message.content?.image?.url != null)
                {
                    ThreadPool.QueueUserWorkItem(ClassificationHandler.AddTrainingData, new ClassificationHandler.TrainRequest()
                    {
                        toId = message.to.id,
                        fromid = message.from.id,
                        imageUrl = message.message.content.image.url,
                        Label = _pendingTrainLabels[message.from.id],
                        Configuration = Configuration
                    });
                    _pendingTrainLabels.Remove(message.from.id);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(ClassificationHandler.ClassifyAndRespond,
                    new ClassificationHandler.ClassifyRequest()
                    {
                        toId = message.to.id,
                        fromid = message.from.id,
                        imageUrl = message.message.content.image.url,
                        Configuration = Configuration
                    });
                }

                return HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.NoContent;
            }
        }
    }
}