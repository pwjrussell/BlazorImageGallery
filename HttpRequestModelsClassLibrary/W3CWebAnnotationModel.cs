using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HttpRequestModelsClassLibrary
{
    public class W3CWebAnnotationModel
    {
        public W3CWebAnnotationModel()
        {

        }
        public W3CWebAnnotationModel(string text, int x, int y, int width, int height)
        {
            @context = "http://www.w3.org/ns/anno.jsonld";
            SetNewUuid();
            type = "Annotation";
            body = new Body[1]
            {
                new Body()
                {
                    type = "TextualBody",
                    value = text,
                    purpose = "commenting"
                }
            };
            target = new Target()
            {
                selector = new Selector()
                {
                    type = "FragmentSelector",
                    conformsTo = "http://www.w3.org/TR/media-frags/",
                    value = $"xywh=pixel:{x},{y},{width},{height}"
                }
            };
        }

        public string @context { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public Body[] body { get; set; }
        public Target target { get; set; }

        public void SetNewUuid()
        {
            id = $"#{Guid.NewGuid():D}";
        }
    }
    public class Body
    {
        public string type { get; set; }
        public string value { get; set; }
        public string purpose { get; set; }
    }
    public class Target
    {
        public Selector selector { get; set; }
    }
    public class Selector
    {
        public string type { get; set; }
        public string conformsTo { get; set; }
        public string value { get; set; }
    }
}
