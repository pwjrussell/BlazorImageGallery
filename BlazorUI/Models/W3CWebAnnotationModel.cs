using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorUI.Models
{
    public class W3CWebAnnotationModel
    {
        public W3CWebAnnotationModel()
        {

        }
        public W3CWebAnnotationModel(string text, int x, int y, int width, int height)
        {
            Content = "http://www.w3.org/ns/anno.jsonld";
            SetNewUuid();
            Type = "Annotation";
            Body = new Body[1]
            {
                new Body()
                {
                    Type = "TextualBody",
                    Value = text
                }
            };
            Target = new Target()
            {
                Selector = new Selector()
                {
                    Type = "FragmentSelector",
                    ConformsTo = "http://www.w3.org/TR/media-frags/",
                    Value = $"xywh=pixel:{x},{y},{width},{height}"
                }
            };
        }

        [JsonPropertyName("@content")]
        public string Content { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public Body[] Body { get; set; }
        public Target Target { get; set; }

        public void SetNewUuid()
        {
            Id = $"#{Guid.NewGuid():D}";
        }
    }
    public class Body
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
    public class Target
    {
        public Selector Selector { get; set; }
    }
    public class Selector
    {
        public string Type { get; set; }
        public string ConformsTo { get; set; }
        public string Value { get; set; }
    }
}
